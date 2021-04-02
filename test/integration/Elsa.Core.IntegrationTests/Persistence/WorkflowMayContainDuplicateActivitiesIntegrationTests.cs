using Xunit;
using System.Threading.Tasks;
using Elsa.Core.IntegrationTests.Autofixture;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Elsa.Services;
using Elsa.Core.IntegrationTests.Workflows;
using Elsa.Persistence;

namespace Elsa.Core.IntegrationTests.Persistence
{
    public class WorkflowMayContainDuplicateActivitiesIntegrationTests
    {
        /* Please note that these tests might not represent _actually desired behaviour_.
         * The tests do prove that issue #683 is no longer a problem, but it still does not seem logical
         * that Elsa should want to allow duplicate activity IDs in a Workflow (definition or instance).
         *
         * It would be reasonable to remove these tests if they begin to "get in the way".
         */

        [Theory(DisplayName = "A workflow that contains duplicate activities may be run & persisted to an in-memory store"), AutoMoqData]
        public async Task ADuplicateActivitiesWorkflowInstanceShouldBeRoundTrippableInMemory([HostBuilderWithDuplicateActivitiesWorkflow] IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((ctx, services) => {
                services.AddHostedService<HostedWorkflowRunner<DuplicateActivitiesWorkflow>>();
            });
            var host = await hostBuilder.StartAsync();
        }

        [Theory(DisplayName = "A workflow that contains duplicate activities may be run & persisted to an EF Sqlite store"), AutoMoqData]
        public async Task ADuplicateActivitiesWorkflowInstanceShouldBeRoundTrippableWithEntityFramework([HostBuilderWithDuplicateActivitiesWorkflowAndEntityFramework] IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((ctx, services) => {
                services.AddHostedService<HostedWorkflowRunner<DuplicateActivitiesWorkflow>>();
            });
            var host = await hostBuilder.StartAsync();
        }

        [Theory(DisplayName = "A workflow that contains duplicate activities may be run & persisted to a MongoDb store"), AutoMoqData]
        public async Task ADuplicateActivitiesWorkflowInstanceShouldBeRoundTrippableWithMongoDb([HostBuilderWithDuplicateActivitiesWorkflowAndMongoDb] IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((ctx, services) => {
                services.AddHostedService<HostedWorkflowRunner<DuplicateActivitiesWorkflow>>();
            });
            var host = await hostBuilder.StartAsync();
        }

        [Theory(DisplayName = "A workflow that contains duplicate activities may be run & persisted to a YesSQL store"), AutoMoqData]
        public async Task ADuplicateActivitiesWorkflowInstanceShouldBeRoundTrippableWithYesSql([HostBuilderWithDuplicateActivitiesWorkflowAndYesSql] IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((ctx, services) => {
                services.AddHostedService<HostedWorkflowRunner<DuplicateActivitiesWorkflow>>();
            });
            var host = await hostBuilder.StartAsync();
        }

        private class HostedWorkflowRunner<TWorkflow> : IHostedService where TWorkflow : DuplicateActivitiesWorkflow
        {
            private readonly IBuildsAndStartsWorkflow _workflowRunner;
            private readonly IWorkflowInstanceStore _instanceStore;

            public async Task StartAsync(CancellationToken cancellationToken)
            {
                var instance = await _workflowRunner.BuildAndStartWorkflowAsync<TWorkflow>(cancellationToken: cancellationToken);
                var retrievedInstance = await _instanceStore.FindByIdAsync(instance.Id, cancellationToken);

                Assert.NotNull(retrievedInstance);
            }

            public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            public HostedWorkflowRunner(IBuildsAndStartsWorkflow workflowRunner, IWorkflowInstanceStore instanceStore)
            {
                _workflowRunner = workflowRunner ?? throw new System.ArgumentNullException(nameof(workflowRunner));
                _instanceStore = instanceStore ?? throw new System.ArgumentNullException(nameof(instanceStore));
            }
        }
    }
}