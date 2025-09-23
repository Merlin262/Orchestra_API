using MediatR;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Handler.BpmnInstance.Command.CreateBpmnProcessInstanceCommand
{
    public class CreateBpmnProcessInstanceCommand : IRequest<BpmnProcessInstance>
    {
        public BpmnProcessInstance Instance { get; }

        public CreateBpmnProcessInstanceCommand(BpmnProcessInstance instance)
        {
            Instance = instance;
        }
    }
}
