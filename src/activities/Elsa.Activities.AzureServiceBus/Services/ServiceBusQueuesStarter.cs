﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Persistence;
using Elsa.Persistence.Specifications.WorkflowInstances;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Elsa.Activities.AzureServiceBus.Services
{
    // TODO: Look for a way to merge ServiceBusQueuesStarter with ServiceBusTopicsStarter - there's a lot of overlap.
    public class ServiceBusQueuesStarter : IServiceBusQueuesStarter
    {
        private readonly IQueueMessageReceiverClientFactory _messageReceiverClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly ICollection<QueueWorker> _workers;

        public ServiceBusQueuesStarter(
            IQueueMessageReceiverClientFactory messageReceiverClientFactory,
            IServiceScopeFactory scopeFactory,
            IServiceProvider serviceProvider,
            ILogger<ServiceBusQueuesStarter> logger)
        {
            _messageReceiverClientFactory = messageReceiverClientFactory;
            _scopeFactory = scopeFactory;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _workers = new List<QueueWorker>();
        }

        public async Task CreateWorkersAsync(CancellationToken cancellationToken = default)
        {
            await DisposeExistingWorkersAsync();
            var queueNames = (await GetQueueNamesAsync(cancellationToken).ToListAsync(cancellationToken)).Distinct();

            foreach (var queueName in queueNames)
            {
                var receiver = await _messageReceiverClientFactory.GetReceiverAsync(queueName, cancellationToken);
                var worker = ActivatorUtilities.CreateInstance<QueueWorker>(_serviceProvider, receiver, (Func<IReceiverClient, Task>) DisposeReceiverAsync);
                _workers.Add(worker);
            }
        }

        private async Task DisposeExistingWorkersAsync()
        {
            foreach (var worker in _workers.ToList())
            {
                await worker.DisposeAsync();
                _workers.Remove(worker);
            }
        }

        private async Task DisposeReceiverAsync(IReceiverClient messageReceiver) => await _messageReceiverClientFactory.DisposeReceiverAsync(messageReceiver);

        private async IAsyncEnumerable<string> GetQueueNamesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var workflowRegistry = scope.ServiceProvider.GetRequiredService<IWorkflowRegistry>();
            var workflowBlueprintReflector = scope.ServiceProvider.GetRequiredService<IWorkflowBlueprintReflector>();
            var workflowInstanceStore = scope.ServiceProvider.GetRequiredService<IWorkflowInstanceStore>();
            var workflows = await workflowRegistry.ListAsync(cancellationToken);

            var query =
                from workflow in workflows
                from activity in workflow.Activities
                where activity.Type == nameof(AzureServiceBusQueueMessageReceived)
                select workflow;

            foreach (var workflow in query)
            {
                // If a workflow is not published, only consider it for processing if it has at least one non-ended workflow instance.
                if (!workflow.IsPublished && !await WorkflowHasNonFinishedWorkflowsAsync(workflow, workflowInstanceStore, cancellationToken))
                    continue;

                var workflowBlueprintWrapper = await workflowBlueprintReflector.ReflectAsync(scope.ServiceProvider, workflow, cancellationToken);

                foreach (var activity in workflowBlueprintWrapper.Filter<AzureServiceBusQueueMessageReceived>())
                {
                    var queueName = await activity.GetPropertyValueAsync(x => x.QueueName, cancellationToken);

                    if (string.IsNullOrWhiteSpace(queueName))
                    {
                        _logger.LogWarning(
                            "Encountered a queue name that is null or empty in activity {ActivityType}:{ActivityId} in workflow {WorkflowDefinitionId}:v{WorkflowDefinitionVersion}",
                            activity.ActivityBlueprint.Type,
                            activity.ActivityBlueprint.Id,
                            workflow.Id,
                            workflow.Version);

                        continue;
                    }

                    yield return queueName!;
                }
            }
        }

        private static async Task<bool> WorkflowHasNonFinishedWorkflowsAsync(IWorkflowBlueprint workflowBlueprint, IWorkflowInstanceStore workflowInstanceStore, CancellationToken cancellationToken)
        {
            var count = await workflowInstanceStore.CountAsync(new NonFinalizedWorkflowSpecification().WithWorkflowDefinition(workflowBlueprint.Id), cancellationToken);
            return count > 0;
        }
    }
}