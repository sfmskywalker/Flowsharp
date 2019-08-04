using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Elsa.Models;
using Elsa.Persistence.YesSql.Documents;
using Elsa.Persistence.YesSql.Extensions;
using Elsa.Persistence.YesSql.Indexes;
using YesSql;

namespace Elsa.Persistence.YesSql.Services
{
    public class YesSqlWorkflowDefinitionStore : IWorkflowDefinitionStore
    {
        private readonly ISession session;
        private readonly IMapper mapper;

        public YesSqlWorkflowDefinitionStore(ISession session, IMapper mapper)
        {
            this.session = session;
            this.mapper = mapper;
        }

        public Task AddAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
        {
            var document = mapper.Map<WorkflowDefinitionDocument>(definition);

            session.Save(document);

            return Task.CompletedTask;
        }

        public async Task<WorkflowDefinition> GetByIdAsync(string id, VersionOptions version, CancellationToken cancellationToken = default)
        {
            var query = session.Query<WorkflowDefinitionDocument, WorkflowDefinitionIndex>().WithVersion(version);
            var document = await query.FirstOrDefaultAsync();

            return mapper.Map<WorkflowDefinition>(document);
        }

        public async Task<IEnumerable<WorkflowDefinition>> ListAsync(VersionOptions version, CancellationToken cancellationToken = default)
        {
            var query = session.Query<WorkflowDefinitionDocument, WorkflowDefinitionIndex>().WithVersion(version);
            var documents = await query.ListAsync();

            return mapper.Map<IEnumerable<WorkflowDefinition>>(documents);
        }

        public Task<WorkflowDefinition> UpdateAsync(WorkflowDefinition definition, CancellationToken cancellationToken)
        {
            session.Save(definition);
            return Task.FromResult(definition);
        }
    }
}