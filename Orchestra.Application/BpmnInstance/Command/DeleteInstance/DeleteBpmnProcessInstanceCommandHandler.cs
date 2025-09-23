using MediatR;
using Orchestra.Domain.Services;
using Orchestra.Services;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.BpmnInstance.Command.DeleteInstance
{
    public class DeleteBpmnProcessInstanceCommandHandler : IRequestHandler<DeleteBpmnProcessInstanceCommand, bool>
    {
        private readonly IBpmnProcessInstanceService _service;

        public DeleteBpmnProcessInstanceCommandHandler(IBpmnProcessInstanceService service)
        {
            _service = service;
        }

        public async Task<bool> Handle(DeleteBpmnProcessInstanceCommand request, CancellationToken cancellationToken)
        {
            var instance = await _service.GetByIdAsync(request.Id, cancellationToken);
            if (instance == null)
                return false;

            await _service.DeleteAsync(instance, cancellationToken);
            return true;
        }
    }
}
