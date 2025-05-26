using MediatR;

namespace Orchestra.Handler.Command.DeleteBpmnProcessBaselineCommand
{
    public class DeleteBpmnProcessBaselineCommand : IRequest<bool>
    {
        public int Id { get; }

        public DeleteBpmnProcessBaselineCommand(int id)
        {
            Id = id;
        }
    }
}
