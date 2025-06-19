using MediatR;

namespace Orchestra.Handler.BpmnInstance.Command.DeleteInstance
{
    public class DeleteBpmnProcessInstanceCommand : IRequest<bool>
    {
        public int Id { get; }

        public DeleteBpmnProcessInstanceCommand(int id)
        {
            Id = id;
        }
    }
}
