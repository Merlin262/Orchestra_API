using MediatR;

namespace Orchestra.Handler.Tasks.Command.AssignUser
{
    public class AssignUserToTaskCommand : IRequest<bool>
    {
        public string TaskId { get; }
        public string UserId { get; }
        public int ProcessInstanceId { get; set; }

        public AssignUserToTaskCommand(string taskId, string userId, int processInstanceId)
        {
            TaskId = taskId;
            UserId = userId;
            ProcessInstanceId = processInstanceId;
        }
    }
}
