﻿using System.Threading;
using System.Threading.Tasks;
using Elsa.Client.Models;
using Refit;

namespace Elsa.Client.Services
{
    public interface IWorkflowDefinitionsApi
    {
        [Get("/v1/workflow-definitions/{workflowDefinitionId}/{versionOptions}")]
        Task<WorkflowDefinition> GetByIdAsync(string workflowDefinitionId, VersionOptions versionOptions, CancellationToken cancellationToken = default);
        
        [Get("/v1/workflow-definitions/{workflowDefinitionVersionId}")]
        Task<WorkflowDefinition> GetByVersionIdAsync(string workflowDefinitionVersionId, CancellationToken cancellationToken = default);

        [Get("/v1/workflow-definitions")]
        Task<PagedList<WorkflowDefinition>> ListAsync(int? page = default, int? pageSize = default, VersionOptions? versionOptions = default, CancellationToken cancellationToken = default);

        [Post("/v1/workflow-definitions")]
        Task<WorkflowDefinition> SaveAsync([Body] SaveWorkflowDefinitionRequest request, CancellationToken cancellationToken = default);

        [Delete("/v1/workflow-definitions/{workflowDefinitionId}")]
        Task DeleteAsync(string workflowDefinitionId, CancellationToken cancellationToken = default);
    }
}