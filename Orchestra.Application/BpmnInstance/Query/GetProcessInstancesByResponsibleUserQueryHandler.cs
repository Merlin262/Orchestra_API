using MediatR;
using Orchestra.Domain.Services;
using Orchestra.Dtos;
using Orchestra.Serviecs.Intefaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestra.Handler.BpmnInstance.Query
{
    public class GetProcessInstancesByResponsibleUserQueryHandler : IRequestHandler<GetProcessInstancesByResponsibleUserQuery, List<ProcessInstanceWithTasksDto>>
    {
        private readonly IBpmnProcessInstanceService _service;
        public GetProcessInstancesByResponsibleUserQueryHandler(IBpmnProcessInstanceService service)
        {
            _service = service;
        }
        public async Task<List<ProcessInstanceWithTasksDto>> Handle(GetProcessInstancesByResponsibleUserQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetProcessInstancesByResponsibleUserAsync(request.UserId, cancellationToken);
        }
    }
}
