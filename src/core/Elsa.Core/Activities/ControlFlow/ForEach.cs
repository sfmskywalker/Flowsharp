using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Models;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using ScheduledActivity = Elsa.Services.Models.ScheduledActivity;

namespace Elsa.Activities.ControlFlow
{
    [ActivityDefinition(Category = "Control Flow", Description = "Iterate over a collection.", Icon = "far fa-circle")]
    public class ForEach : Activity
    {
        [ActivityProperty(Hint = "Enter an expression that evaluates to a collection of items to iterate over.")]
        public IWorkflowExpression<ICollection<object>> Collection
        {
            get => GetState<IWorkflowExpression<ICollection<object>>>();
            set => SetState(value);
        }
        
        private int? CurrentIndex
        {
            get => GetState<int?>();
            set => SetState(value);
        }

        protected override async Task<IActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext workflowExecutionContext, ActivityExecutionContext activityExecutionContext, CancellationToken cancellationToken)
        {
            var collection = (await workflowExecutionContext.EvaluateAsync(Collection, activityExecutionContext, cancellationToken))?.ToArray() ?? new object[0];
            var currentIndex = CurrentIndex ?? 0;

            if (currentIndex < collection.Length)
            {
                var input = collection[currentIndex];
                CurrentIndex = currentIndex + 1;
                return Combine(Schedule(this), Done(OutcomeNames.Iterate, Variable.From(input)));
            }

            CurrentIndex = null;
            return Done();
        }
    }
}