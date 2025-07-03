using MediatR;

namespace Orchestra.Handler.Tasks.Command.UpdateTaskStatus
{
    public class UpdateTaskStatusCommand : IRequest<UpdateTaskStatusResultDto>
    {
        public Guid TaskId { get; set; }
        public int StatusId { get; set; }

        public UpdateTaskStatusCommand(Guid taskId, int statusId)
        {
            TaskId = taskId;
            StatusId = statusId;
        }
    }
}
