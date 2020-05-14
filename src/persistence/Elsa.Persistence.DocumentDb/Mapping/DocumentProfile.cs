using Elsa.Models;
using Elsa.Persistence.DocumentDb.Documents;
using Elsa.Services;

namespace Elsa.Persistence.DocumentDb.Mapping
{
    public sealed class DocumentProfile : MapperProfile
    {
        public DocumentProfile()
        {
            CreateMap<WorkflowDefinitionVersion, WorkflowDefinitionVersionDocument>().ReverseMap();
            CreateMap<WorkflowInstance, WorkflowInstanceDocument>().ReverseMap();
        }
    }
}
