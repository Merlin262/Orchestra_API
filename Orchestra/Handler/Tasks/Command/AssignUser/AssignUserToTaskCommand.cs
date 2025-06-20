using MediatR;

namespace Orchestra.Handler.Tasks.Command.AssignUser
{
    public class AssignUserToTaskCommand : IRequest<bool>
    {
        public string TaskId { get; }
        public string UserId { get; }

        public AssignUserToTaskCommand(string taskId, string userId)
        {
            TaskId = taskId;
            UserId = userId;
        }
    }
}
