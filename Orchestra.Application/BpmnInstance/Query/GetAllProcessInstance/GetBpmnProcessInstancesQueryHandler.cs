using MediatR;
using Orchestra.Domain.Services;
using Orchestra.Dtos;
using Orchestra.Handler.BpmnInstance.Query.GetAllProcessInstance;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Services;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.BpmnInstance.Query.GetProcessInstance
{
    public class GetBpmnProcessInstancesQueryHandler : IRequestHandler<GetBpmnProcessInstancesQuery, IEnumerable<GetBpmnProcessInstancesQueryResult>>
    {
        private readonly IBpmnProcessInstanceService _service;

        public GetBpmnProcessInstancesQueryHandler(IBpmnProcessInstanceService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<GetBpmnProcessInstancesQueryResult>> Handle(GetBpmnProcessInstancesQuery request, CancellationToken cancellationToken)
        {
            var instances = await _service.GetAllWithUserAsync(cancellationToken);

            var results = instances.Select(instance => new GetBpmnProcessInstancesQueryResult
            {
                id = instance.Id,
                Name = instance.Name,
                XmlContent = instance.XmlContent,
                CreatedAt = instance.CreatedAt,
                BpmnProcessBaselineId = instance.BpmnProcessBaselineId,
                Version = instance.version,
                Description = instance.Description,
                CreatedBy = instance.CreatedBy.FullName
            });

            return results;
        }
    }
}


