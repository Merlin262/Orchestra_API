using MediatR;

namespace Orchestra.Handler.BpmnBaseline.Command.DeleteBpmnProcessBaselineCommand
{
    public class DeleteBpmnProcessBaselineCommand : IRequest<DeleteBpmnProcessBaselineCommandResult>
    {
        public int Id { get; }

        public DeleteBpmnProcessBaselineCommand(int id)
        {
            Id = id;
        }
    }
}
