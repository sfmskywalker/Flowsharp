﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elsa.Activities.Telnyx.Client.Models;
using Elsa.Activities.Telnyx.Client.Services;
using Elsa.Activities.Telnyx.Extensions;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Builders;
using Elsa.Design;
using Elsa.Exceptions;
using Elsa.Services;
using Elsa.Services.Models;
using Refit;

namespace Elsa.Activities.Telnyx.Activities
{
    [Action(
        Category = Constants.Category,
        Description = "Dial a number or SIP URI from a given connection.",
        Outcomes = new[] { OutcomeNames.Done },
        DisplayName = "Dial"
    )]
    public class Dial : Activity
    {
        private readonly ITelnyxClient _telnyxClient;

        public Dial(ITelnyxClient telnyxClient)
        {
            _telnyxClient = telnyxClient;
        }

        [ActivityProperty(Label = "Call Control ID", Hint = "The ID of the Call Control App (formerly ID of the connection) to be used when dialing the destination.", Category = PropertyCategories.Advanced)]
        public string CallControlId { get; set; } = default!;

        [ActivityProperty(Label = "To", Hint = "The DID or SIP URI to dial out and bridge to the given call.")]
        public string To { get; set; } = default!;

        [ActivityProperty(Label = "From",
            Hint = "The 'from' number to be used as the caller id presented to the destination ('To' number). The number should be in +E164 format. This attribute will default to the 'From' number of the original call if omitted.")]
        public string? From { get; set; }

        [ActivityProperty(Label = "From Display Name",
            Hint =
                "The string to be used as the caller id name (SIP From Display Name) presented to the destination ('To' number). The string should have a maximum of 128 characters, containing only letters, numbers, spaces, and -_~!.+ special characters. If omitted, the display name will be the same as the number in the 'From' field.")]
        public string? FromDisplayName { get; set; }

        [ActivityProperty(Label = "Answering Machine Detection", Hint = "Enables Answering Machine Detection.", UIHint = ActivityPropertyUIHints.Dropdown,
            Options = new[] { "disabled", "detect", "detect_beep", "detect_words", "greeting_end" })]
        public string? AnsweringMachineDetection { get; set; }

        [ActivityProperty(Label = "Answering Machine Detection Configuration", Hint = "Optional configuration parameters to modify answering machine detection performance.", Category = PropertyCategories.Advanced)]
        public AnsweringMachineConfig? AnsweringMachineDetectionConfig { get; set; }

        [ActivityProperty(Label = "Command ID", Hint = "Use this field to avoid duplicate commands. Telnyx will ignore commands with the same Command ID.", Category = PropertyCategories.Advanced)]
        public string? CommandId { get; set; }

        [ActivityProperty(Label = "Client State", Hint = "Use this field to add state to every subsequent webhook. It must be a valid Base-64 encoded string.", Category = PropertyCategories.Advanced)]
        public string? ClientState { get; set; }

        [ActivityProperty(Label = "Custom Headers", Hint = "Custom headers to be added to the SIP INVITE.", Category = PropertyCategories.Advanced)]
        public IList<Header>? CustomHeaders { get; set; }

        [ActivityProperty(Label = "SIP Authentication Username", Hint = "SIP Authentication username used for SIP challenges.", Category = "SIP Authentication")]
        public string? SipAuthUsername { get; set; }

        [ActivityProperty(Label = "SIP Authentication Password", Hint = "SIP Authentication password used for SIP challenges.", Category = "SIP Authentication")]
        public string? SipAuthPassword { get; set; }

        [ActivityProperty(Label = "Time Limit", Hint = "Sets the maximum duration of a Call Control Leg in seconds.", Category = PropertyCategories.Advanced)]
        public int? TimeLimitSecs { get; set; }

        [ActivityProperty(Label = "Timeout", Hint = "The number of seconds that Telnyx will wait for the call to be answered by the destination to which it is being transferred.", Category = PropertyCategories.Advanced)]
        public int? TimeoutSecs { get; set; }

        [ActivityProperty(Label = "Webhook URL", Hint = "Use this field to override the URL for which Telnyx will send subsequent webhooks to for this call.", Category = PropertyCategories.Advanced)]
        public string? WebhookUrl { get; set; }

        [ActivityProperty(Label = "Webhook URL Method", Hint = "HTTP request type used for Webhook URL", UIHint = ActivityPropertyUIHints.Dropdown, Options = new[] { "GET", "POST" }, Category = PropertyCategories.Advanced)]
        public string? WebhookUrlMethod { get; set; }
        
        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var response = await DialAsync(context);
            return Done(response);
        }

        private async Task<DialResponse> DialAsync(ActivityExecutionContext context)
        {
            var callControlId = context.GetCallControlId(CallControlId);
            
            var request = new DialRequest(
                callControlId,
                To,
                From,
                FromDisplayName,
                AnsweringMachineDetection,
                AnsweringMachineDetectionConfig,
                ClientState,
                CommandId,
                CustomHeaders,
                SipAuthUsername,
                SipAuthPassword,
                TimeLimitSecs,
                TimeoutSecs,
                WebhookUrl,
                WebhookUrlMethod
            );

            try
            {
                var response = await _telnyxClient.Calls.DialAsync(request, context.CancellationToken);
                return response.Data;
            }
            catch (ApiException e)
            {
                throw new WorkflowException(e.Content ?? e.Message, e);
            }
        }
    }
    
    public static class DialExtensions
    {
        public static ISetupActivity<Dial> WithCallControlId(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.CallControlId, value);
        public static ISetupActivity<Dial> WithCallControlId(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.CallControlId, value);
        public static ISetupActivity<Dial> WithCallControlId(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.CallControlId, value);
        public static ISetupActivity<Dial> WithCallControlId(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.CallControlId, value);
        public static ISetupActivity<Dial> WithCallControlId(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.CallControlId, value);
        
        public static ISetupActivity<Dial> WithTo(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.To, value);
        public static ISetupActivity<Dial> WithTo(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.To, value);
        public static ISetupActivity<Dial> WithTo(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.To, value);
        public static ISetupActivity<Dial> WithTo(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.To, value);
        public static ISetupActivity<Dial> WithTo(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.To, value);
        
        public static ISetupActivity<Dial> WithFrom(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.From, value);
        public static ISetupActivity<Dial> WithFrom(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.From, value);
        public static ISetupActivity<Dial> WithFrom(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.From, value);
        public static ISetupActivity<Dial> WithFrom(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.From, value);
        public static ISetupActivity<Dial> WithFrom(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.From, value);
        
        public static ISetupActivity<Dial> WithFromDisplayName(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.FromDisplayName, value);
        public static ISetupActivity<Dial> WithFromDisplayName(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.FromDisplayName, value);
        public static ISetupActivity<Dial> WithFromDisplayName(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.FromDisplayName, value);
        public static ISetupActivity<Dial> WithFromDisplayName(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.FromDisplayName, value);
        public static ISetupActivity<Dial> WithFromDisplayName(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.FromDisplayName, value);
        
        public static ISetupActivity<Dial> WithAnsweringMachineDetection(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.AnsweringMachineDetection, value);
        public static ISetupActivity<Dial> WithAnsweringMachineDetection(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.AnsweringMachineDetection, value);
        public static ISetupActivity<Dial> WithAnsweringMachineDetection(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.AnsweringMachineDetection, value);
        public static ISetupActivity<Dial> WithAnsweringMachineDetection(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.AnsweringMachineDetection, value);
        public static ISetupActivity<Dial> WithAnsweringMachineDetection(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.AnsweringMachineDetection, value);
        
        public static ISetupActivity<Dial> WithAnsweringMachineDetectionConfig(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<AnsweringMachineConfig?>> value) => setup.Set(x => x.AnsweringMachineDetectionConfig, value);
        public static ISetupActivity<Dial> WithAnsweringMachineDetectionConfig(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, AnsweringMachineConfig?> value) => setup.Set(x => x.AnsweringMachineDetectionConfig, value);
        public static ISetupActivity<Dial> WithAnsweringMachineDetectionConfig(this ISetupActivity<Dial> setup, Func<ValueTask<AnsweringMachineConfig?>> value) => setup.Set(x => x.AnsweringMachineDetectionConfig, value);
        public static ISetupActivity<Dial> WithAnsweringMachineDetectionConfig(this ISetupActivity<Dial> setup, Func<AnsweringMachineConfig?> value) => setup.Set(x => x.AnsweringMachineDetectionConfig, value);
        public static ISetupActivity<Dial> WithAnsweringMachineDetectionConfig(this ISetupActivity<Dial> setup, AnsweringMachineConfig? value) => setup.Set(x => x.AnsweringMachineDetectionConfig, value);
        
        public static ISetupActivity<Dial> WithCommandId(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.CommandId, value);
        public static ISetupActivity<Dial> WithCommandId(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.CommandId, value);
        public static ISetupActivity<Dial> WithCommandId(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.CommandId, value);
        public static ISetupActivity<Dial> WithCommandId(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.CommandId, value);
        public static ISetupActivity<Dial> WithCommandId(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.CommandId, value);
        
        public static ISetupActivity<Dial> WithClientState(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.ClientState, value);
        public static ISetupActivity<Dial> WithClientState(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.ClientState, value);
        public static ISetupActivity<Dial> WithClientState(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.ClientState, value);
        public static ISetupActivity<Dial> WithClientState(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.ClientState, value);
        public static ISetupActivity<Dial> WithClientState(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.ClientState, value);
        
        public static ISetupActivity<Dial> WithCustomHeaders(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<IList<Header>?>> value) => setup.Set(x => x.CustomHeaders, value);
        public static ISetupActivity<Dial> WithCustomHeaders(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, IList<Header>?> value) => setup.Set(x => x.CustomHeaders, value);
        public static ISetupActivity<Dial> WithCustomHeaders(this ISetupActivity<Dial> setup, Func<ValueTask<IList<Header>?>> value) => setup.Set(x => x.CustomHeaders, value);
        public static ISetupActivity<Dial> WithCustomHeaders(this ISetupActivity<Dial> setup, Func<IList<Header>?> value) => setup.Set(x => x.CustomHeaders, value);
        public static ISetupActivity<Dial> WithCustomHeaders(this ISetupActivity<Dial> setup, IList<Header>? value) => setup.Set(x => x.CustomHeaders, value);
        
        public static ISetupActivity<Dial> WithSipAuthUsername(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.SipAuthUsername, value);
        public static ISetupActivity<Dial> WithSipAuthUsername(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.SipAuthUsername, value);
        public static ISetupActivity<Dial> WithSipAuthUsername(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.SipAuthUsername, value);
        public static ISetupActivity<Dial> WithSipAuthUsername(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.SipAuthUsername, value);
        public static ISetupActivity<Dial> WithSipAuthUsername(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.SipAuthUsername, value);
        
        public static ISetupActivity<Dial> WithSipAuthPassword(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.SipAuthPassword, value);
        public static ISetupActivity<Dial> WithSipAuthPassword(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.SipAuthPassword, value);
        public static ISetupActivity<Dial> WithSipAuthPassword(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.SipAuthPassword, value);
        public static ISetupActivity<Dial> WithSipAuthPassword(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.SipAuthPassword, value);
        public static ISetupActivity<Dial> WithSipAuthPassword(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.SipAuthPassword, value);
        
        public static ISetupActivity<Dial> WithTimeLimitSecs(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<int?>> value) => setup.Set(x => x.TimeLimitSecs, value);
        public static ISetupActivity<Dial> WithTimeLimitSecs(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, int?> value) => setup.Set(x => x.TimeLimitSecs, value);
        public static ISetupActivity<Dial> WithTimeLimitSecs(this ISetupActivity<Dial> setup, Func<ValueTask<int?>> value) => setup.Set(x => x.TimeLimitSecs, value);
        public static ISetupActivity<Dial> WithTimeLimitSecs(this ISetupActivity<Dial> setup, Func<int?> value) => setup.Set(x => x.TimeLimitSecs, value);
        public static ISetupActivity<Dial> WithTimeLimitSecs(this ISetupActivity<Dial> setup, int? value) => setup.Set(x => x.TimeLimitSecs, value);
        
        public static ISetupActivity<Dial> WithTimeoutSecs(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<int?>> value) => setup.Set(x => x.TimeoutSecs, value);
        public static ISetupActivity<Dial> WithTimeoutSecs(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, int?> value) => setup.Set(x => x.TimeoutSecs, value);
        public static ISetupActivity<Dial> WithTimeoutSecs(this ISetupActivity<Dial> setup, Func<ValueTask<int?>> value) => setup.Set(x => x.TimeoutSecs, value);
        public static ISetupActivity<Dial> WithTimeoutSecs(this ISetupActivity<Dial> setup, Func<int?> value) => setup.Set(x => x.TimeoutSecs, value);
        public static ISetupActivity<Dial> WithTimeoutSecs(this ISetupActivity<Dial> setup, int? value) => setup.Set(x => x.TimeoutSecs, value);
        
        public static ISetupActivity<Dial> WithWebhookUrl(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.WebhookUrl, value);
        public static ISetupActivity<Dial> WithWebhookUrl(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.WebhookUrl, value);
        public static ISetupActivity<Dial> WithWebhookUrl(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.WebhookUrl, value);
        public static ISetupActivity<Dial> WithWebhookUrl(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.WebhookUrl, value);
        public static ISetupActivity<Dial> WithWebhookUrl(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.WebhookUrl, value);
        
        public static ISetupActivity<Dial> WithWebhookUrlMethod(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.WebhookUrlMethod, value);
        public static ISetupActivity<Dial> WithWebhookUrlMethod(this ISetupActivity<Dial> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.WebhookUrlMethod, value);
        public static ISetupActivity<Dial> WithWebhookUrlMethod(this ISetupActivity<Dial> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.WebhookUrlMethod, value);
        public static ISetupActivity<Dial> WithWebhookUrlMethod(this ISetupActivity<Dial> setup, Func<string?> value) => setup.Set(x => x.WebhookUrlMethod, value);
        public static ISetupActivity<Dial> WithWebhookUrlMethod(this ISetupActivity<Dial> setup, string? value) => setup.Set(x => x.WebhookUrlMethod, value);
    }
}