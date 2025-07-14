using MediatR;

namespace Orchestra.Handler.Tasks.Command.UnassignUser
{
    public class UnassignUserFromTaskCommand : IRequest<bool>
    {
        public Guid TaskId { get; }

        public UnassignUserFromTaskCommand(Guid taskId)
        {
            TaskId = taskId;
        }
    }
}
