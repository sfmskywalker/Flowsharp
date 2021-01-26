﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.Specifications.WorkflowTriggers;
using Elsa.Serialization;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Open.Linq.AsyncExtensions;
using Rebus.Extensions;

namespace Elsa.Triggers
{
    public class WorkflowTriggerIndexer : IWorkflowTriggerIndexer
    {
        private readonly IWorkflowRegistry _workflowRegistry;
        private readonly IWorkflowTriggerStore _workflowTriggerStore;
        private readonly IWorkflowContextManager _workflowContextManager;
        private readonly IEnumerable<IWorkflowTriggerProvider> _providers;
        private readonly IIdGenerator _idGenerator;
        private readonly IContentSerializer _contentSerializer;
        private readonly IWorkflowTriggerHasher _hasher;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private Stopwatch _stopwatch = new();

        public WorkflowTriggerIndexer(
            IWorkflowRegistry workflowRegistry,
            IWorkflowTriggerStore workflowTriggerStore,
            IWorkflowContextManager workflowContextManager,
            IEnumerable<IWorkflowTriggerProvider> providers,
            IIdGenerator idGenerator,
            IContentSerializer contentSerializer,
            IServiceProvider serviceProvider,
            IWorkflowTriggerHasher hasher,
            ILogger<WorkflowTriggerIndexer> logger)
        {
            _workflowRegistry = workflowRegistry;
            _workflowTriggerStore = workflowTriggerStore;
            _workflowContextManager = workflowContextManager;
            _providers = providers;
            _idGenerator = idGenerator;
            _contentSerializer = contentSerializer;
            _serviceProvider = serviceProvider;
            _hasher = hasher;
            _logger = logger;
        }

        public async Task IndexTriggersAsync(WorkflowInstance workflowInstance, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Indexing triggers for workflow {WorkflowInstanceId}", workflowInstance.Id);
            _stopwatch.Restart();

            await DeleteTriggersAsync(workflowInstance.Id, cancellationToken);
            var workflowBlueprint = await _workflowRegistry.GetWorkflowAsync(workflowInstance.DefinitionId, VersionOptions.SpecificVersion(workflowInstance.Version), cancellationToken);

            if (workflowBlueprint == null)
            {
                _logger.LogWarning("Could not find workflow definition for workflow {WorkflowInstanceId}", workflowInstance.Id);
                return;
            }

            var blockingActivities = workflowBlueprint.GetBlockingActivities(workflowInstance!);
            var triggerDescriptors = await ExtractTriggersAsync(workflowBlueprint, workflowInstance, blockingActivities, true, cancellationToken).ToList();
            await PersistTriggersAsync(triggerDescriptors, workflowInstance, cancellationToken);

            _stopwatch.Stop();
            _logger.LogInformation("Indexed {TriggerCount} triggers for workflow {WorkflowInstanceId} in {ElapsedTime}", triggerDescriptors.Count, workflowInstance.Id, _stopwatch.Elapsed);
        }

        public async Task DeleteTriggersAsync(IEnumerable<string> workflowInstanceIds, CancellationToken cancellationToken = default)
        {
            var specification = new WorkflowInstanceIdsSpecification(workflowInstanceIds);
            await _workflowTriggerStore.DeleteManyAsync(specification, cancellationToken);
        }

        public async Task DeleteTriggersAsync(string workflowInstanceId, CancellationToken cancellationToken = default)
        {
            var specification = new WorkflowInstanceIdSpecification(workflowInstanceId);
            var count = await _workflowTriggerStore.DeleteManyAsync(specification, cancellationToken);
            
            _logger.LogDebug("Deleted {DeletedTriggerCount} triggers for workflow {WorkflowInstanceId}", count, workflowInstanceId);
        }

        private async Task PersistTriggersAsync(IEnumerable<TriggerDescriptor> triggerDescriptors, WorkflowInstance workflowInstance, CancellationToken cancellationToken)
        {
            foreach (var triggerDescriptor in triggerDescriptors)
            {
                var records = triggerDescriptor.Triggers.Select(x => new WorkflowTrigger
                {
                    Id = _idGenerator.Generate(),
                    TenantId = workflowInstance.TenantId,
                    ActivityType = triggerDescriptor.ActivityType,
                    ActivityId = triggerDescriptor.ActivityId,
                    WorkflowInstanceId = workflowInstance.Id,
                    Hash = _hasher.Hash(x),
                    Model = _contentSerializer.Serialize(x),
                    TypeName = x.GetType().GetSimpleAssemblyQualifiedName()
                });

                await _workflowTriggerStore.AddManyAsync(records, cancellationToken);
            }
        }

        private async Task<IEnumerable<TriggerDescriptor>> ExtractTriggersAsync(
            IWorkflowBlueprint workflowBlueprint,
            WorkflowInstance workflowInstance,
            IEnumerable<IActivityBlueprint> blockingActivities,
            bool loadContext,
            CancellationToken cancellationToken)
        {
            // Setup workflow execution context
            var scope = _serviceProvider.CreateScope();
            var workflowExecutionContext = new WorkflowExecutionContext(scope, workflowBlueprint, workflowInstance);

            // Load workflow context.
            workflowExecutionContext.WorkflowContext =
                loadContext &&
                workflowBlueprint.ContextOptions != null &&
                !string.IsNullOrWhiteSpace(workflowInstance.ContextId)
                    ? await _workflowContextManager.LoadContext(new LoadWorkflowContext(workflowExecutionContext), cancellationToken)
                    : default;

            // Extract triggers for each blocking activity.
            var triggerDescriptors = new List<TriggerDescriptor>();

            foreach (var blockingActivity in blockingActivities)
            {
                var activityExecutionContext = new ActivityExecutionContext(scope, workflowExecutionContext, blockingActivity, null, cancellationToken);
                var providerContext = new TriggerProviderContext(activityExecutionContext);
                var providers = _providers.Where(x => x.ForActivityType == blockingActivity.Type);

                foreach (var provider in providers)
                {
                    var triggers = (await provider.GetTriggersAsync(providerContext, cancellationToken)).ToList();

                    var triggerDescriptor = new TriggerDescriptor
                    {
                        WorkflowBlueprint = workflowBlueprint,
                        WorkflowInstanceId = workflowInstance.Id,
                        ActivityType = blockingActivity.Type,
                        ActivityId = blockingActivity.Id,
                        Triggers = triggers
                    };

                    triggerDescriptors.Add(triggerDescriptor);
                }
            }

            return triggerDescriptors;
        }
    }
}