using MediatR;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Services;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.BpmnInstance.Command.CreateBpmnProcessInstanceCommand
{
    public class CreateBpmnProcessInstanceCommandHandler : IRequestHandler<CreateBpmnProcessInstanceCommand, BpmnProcessInstance>
    {
        private readonly IBpmnProcessInstanceService _service;

        public CreateBpmnProcessInstanceCommandHandler(IBpmnProcessInstanceService service)
        {
            _service = service;
        }

        public async Task<BpmnProcessInstance> Handle(CreateBpmnProcessInstanceCommand request, CancellationToken cancellationToken)
        {
            await _service.AddAsync(request.Instance, cancellationToken);
            return request.Instance;
        }

    }
}
