using System.Threading.Tasks;
using Elsa.Activities.AzureServiceBus.Results;
using Elsa.Activities.AzureServiceBus.Services;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Serialization;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Activities.AzureServiceBus
{
    [Action(Category = "Azure Service Bus", DisplayName = "Send Service Bus Topic Message", Description = "Sends a message to the specified topic", Outcomes = new[] { OutcomeNames.Done })]
    public class SendAzureServiceBusTopicMessage : Activity
    {
        private readonly ITopicMessageSenderFactory _messageSenderFactory;
        private readonly IContentSerializer _serializer;

        public SendAzureServiceBusTopicMessage(ITopicMessageSenderFactory messageSenderFactory, IContentSerializer serializer)
        {
            _messageSenderFactory = messageSenderFactory;
            _serializer = serializer;
        }

        [ActivityInput] public string TopicName { get; set; } = default!;
        [ActivityInput] public object Message { get; set; } = default!;
        
        [ActivityInput(DefaultValue = true, Hint = "Allow the activity to send immediatly the message if true or wait for the first suspend. Usefull for RequestResponse pattern using Topic Message Received " )] 
        public bool FireAndForget { get; set; } = true;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var sender = await _messageSenderFactory.GetTopicSenderAsync(TopicName, context.CancellationToken);

            var message = Extensions.MessageBodyExtensions.CreateMessage(_serializer,Message);

            if (!string.IsNullOrWhiteSpace(context.WorkflowExecutionContext.CorrelationId))
                message.CorrelationId = context.WorkflowExecutionContext.CorrelationId;

            if (!FireAndForget)
                return Combine(Done(), new ServiceBusActionResult(sender, message));
            
            await sender.SendAsync(message);
            return Done();
        }
    }
}