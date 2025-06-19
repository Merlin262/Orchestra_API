using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Services;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.BpmnInstance.Query.GetProcessInstance
{
    public class GetBpmnProcessInstancesQueryHandler : IRequestHandler<GetBpmnProcessInstancesQuery, IEnumerable<BpmnProcessInstance>>
    {
        private readonly IBpmnProcessInstanceService _service;

        public GetBpmnProcessInstancesQueryHandler(IBpmnProcessInstanceService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<BpmnProcessInstance>> Handle(GetBpmnProcessInstancesQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(cancellationToken);
        }
    }
}
