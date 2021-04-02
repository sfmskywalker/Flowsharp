using System;
using AutoFixture.Xunit2;
using Elsa.Builders;
using Elsa.Models;
using Elsa.Testing.Shared.AutoFixture.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Elsa.Services
{
    public class WorkflowBuilderTests
    {
        [Theory(DisplayName = "The Build method should return a workflow blueprint with the WorkflowBurst persistence behaviour if no behaviour was specified"), AutoMoqData]
        public void BuildShouldReturnWorkflowBlueprintWithWorkflowBurstPersistenceBehaviourIfNoBehaviourSpecified(
            [AutofixtureServiceProvider] IServiceProvider serviceProvider,
            IIdGenerator idGenerator,
            IGetsStartActivitiesForCompositeActivityBlueprint getsStartActivitiesForCompositeActivityBlueprint,
            string idPrefix)
        {
            var workflow = new NoOpWorkflow();
            var sut = new WorkflowBuilder(idGenerator, serviceProvider, getsStartActivitiesForCompositeActivityBlueprint);
            var blueprint = sut.Build(workflow, idPrefix);

            Assert.Equal(WorkflowPersistenceBehavior.WorkflowBurst, blueprint.PersistenceBehavior);
        }

        class NoOpWorkflow : IWorkflow
        {
            public void Build(IWorkflowBuilder builder)
            {
            }
        }
    }
}