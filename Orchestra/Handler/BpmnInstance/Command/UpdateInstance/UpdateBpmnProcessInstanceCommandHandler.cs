using MediatR;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.BpmnInstance.Command.UpdateInstance
{
    public class UpdateBpmnProcessInstanceCommandHandler : IRequestHandler<UpdateBpmnProcessInstanceCommand, BpmnProcessInstance?>
    {
        private readonly IBpmnProcessInstanceService _service;

        public UpdateBpmnProcessInstanceCommandHandler(IBpmnProcessInstanceService service)
        {
            _service = service;
        }

        public async Task<BpmnProcessInstance?> Handle(UpdateBpmnProcessInstanceCommand request, CancellationToken cancellationToken)
        {
            var instance = await _service.GetByIdAsync(request.Id, cancellationToken);
            if (instance == null)
                return null;

            if (request.Name != null)
                instance.Name = request.Name;
            if (request.Description != null)
                instance.Description = request.Description;

            await _service.UpdateAsync(instance, cancellationToken);
            return instance;
        }
    }
}
