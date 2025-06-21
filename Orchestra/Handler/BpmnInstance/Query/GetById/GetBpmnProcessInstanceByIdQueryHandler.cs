using MediatR;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Services;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.BpmnInstance.Query.GetById
{
    public class GetBpmnProcessInstanceByIdQueryHandler : IRequestHandler<GetBpmnProcessInstanceByIdQuery, BpmnProcessInstance?>
    {
        private readonly IBpmnProcessInstanceService _service;

        public GetBpmnProcessInstanceByIdQueryHandler(IBpmnProcessInstanceService service)
        {
            _service = service;
        }

        public async Task<BpmnProcessInstance?> Handle(GetBpmnProcessInstanceByIdQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
