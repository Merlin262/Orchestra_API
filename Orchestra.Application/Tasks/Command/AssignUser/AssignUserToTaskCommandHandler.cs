using MediatR;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.Tasks.Command.AssignUser
{
    public class AssignUserToTaskCommandHandler : IRequestHandler<AssignUserToTaskCommand, bool>
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;

        public AssignUserToTaskCommandHandler(ITaskService taskService, IUserService userService)
        {
            _taskService = taskService;
            _userService = userService;
        }

        public async Task<bool> Handle(AssignUserToTaskCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return false;

            var result = await _taskService.AssignUserToTaskAsync(request.ProcessInstanceId, request.TaskId, request.UserId, cancellationToken);
            return result;
        }
    }
}
