using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Http.Extensions;
using Elsa.Activities.Http.Models;
using Elsa.Activities.Http.Services;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Extensions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Elsa.Activities.Http.Activities
{
    [ActivityDefinition(
        Category = "HTTP",
        DisplayName = "Receive HTTP Request",
        Description = "Receive an incoming HTTP request.",
        RuntimeDescription = "x => !!x.state.path ? `Handle <strong>${ x.state.method } ${ x.state.path }</strong>.` : x.definition.description",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class ReceiveHttpRequest : Activity
    {
        public static Uri GetPath(JObject state)
        {
            return state.GetState<Uri>(nameof(Path));
        }

        public static string GetMethod(JObject state)
        {
            return state.GetState<string>(nameof(Method));
        }

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IEnumerable<IContentFormatter> contentFormatters;

        public ReceiveHttpRequest(
            IHttpContextAccessor httpContextAccessor,
            IEnumerable<IContentFormatter> contentFormatters)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.contentFormatters = contentFormatters;
        }

        /// <summary>
        /// The path that triggers this activity. 
        /// </summary>
        [ActivityProperty(Hint = "The relative path that triggers this activity.")]
        public Uri Path
        {
            get => GetState<Uri>();
            set => SetState(value);
        }

        /// <summary>
        /// The HTTP method that triggers this activity.
        /// </summary>
        [ActivityProperty(
            Type = ActivityPropertyTypes.Select,
            Hint = "The HTTP method that triggers this activity."
        )]
        [SelectOptions("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS", "HEAD")]
        public string Method
        {
            get => GetState<string>();
            set => SetState(value);
        }

        /// <summary>
        /// A value indicating whether the HTTP request content body should be read and stored as part of the HTTP request model.
        /// The stored format depends on the content-type header.
        /// </summary>
        [ActivityProperty(
            Hint =
                "A value indicating whether the HTTP request content body should be read and stored as part of the HTTP request model. The stored format depends on the content-type header."
        )]
        public bool ReadContent
        {
            get => GetState<bool>();
            set => SetState(value);
        }

        /// <summary>
        /// Variable name for model body - Read model body to named variable. 
        /// </summary>
        [ActivityProperty(Hint = "Variable name for content model - Read content model to named variable.")]
        public string OutputVariable
        {
            get => GetState<string>(null, "OutputVariable");
            set => SetState(value, "OutputVariable");
        }

        protected override ActivityExecutionResult OnExecute(WorkflowExecutionContext workflowContext)
        {
            return Halt(true);
        }

        protected override async Task<ActivityExecutionResult> OnResumeAsync(
            WorkflowExecutionContext workflowContext,
            CancellationToken cancellationToken)
        {
            
            var request = httpContextAccessor.HttpContext.Request;
            var model = new HttpRequestModel
            {
                Path = new Uri(request.Path.ToString(), UriKind.Relative),
                QueryString = request.Query.ToDictionary(x => x.Key, x => new StringValuesModel(x.Value)),
                Headers = request.Headers.ToDictionary(x => x.Key, x => new StringValuesModel(x.Value)),
                Method = request.Method
            };

            if (ReadContent)
            {
                if (request.HasFormContentType)
                {
                    model.Form = (await request.ReadFormAsync(cancellationToken)).ToDictionary(
                        x => x.Key,
                        x => new StringValuesModel(x.Value)
                    );
                }

                var parser = SelectContentParser(request.ContentType);
                var content = await request.ReadContentAsBytesAsync(cancellationToken);
                model.Body = await parser.ParseAsync(content, request.ContentType);
            }

            Output["Content"] = model;
            workflowContext.CurrentScope.LastResult = model;
            if(OutputVariable != null && OutputVariable != "")
            {
                workflowContext.SetVariable(OutputVariable, model.Body);
            }

            return Done();
        }

        private IContentFormatter SelectContentParser(string contentType)
        {
            var formatters = contentFormatters.OrderByDescending(x => x.Priority).ToList();
            return formatters.FirstOrDefault(
                       x => x.SupportedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase)
                   ) ?? formatters.Last();
        }
    }
}