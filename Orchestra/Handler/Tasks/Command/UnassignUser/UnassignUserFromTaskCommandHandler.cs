using MediatR;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.Tasks.Command.UnassignUser
{
    public class UnassignUserFromTaskCommandHandler : IRequestHandler<UnassignUserFromTaskCommand, bool>
    {
        private readonly ITaskService _taskService;

        public UnassignUserFromTaskCommandHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<bool> Handle(UnassignUserFromTaskCommand request, CancellationToken cancellationToken)
        {
            return await _taskService.UnassignUserFromTaskAsync(request.TaskId, cancellationToken);
        }
    }
}