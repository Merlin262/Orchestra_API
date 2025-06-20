using MediatR;
using Orchestra.Data.Context;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.Tasks.Command.UpdateTaskStatus
{
    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, bool>
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly ITaskService _taskService;

        public UpdateTaskStatusCommandHandler(ITasksRepository tasksRepository, ITaskService taskService)
        {
            _tasksRepository = tasksRepository;
            _taskService = taskService;
        }

        public async Task<bool> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            return await _taskService.UpdateTaskStatusAsync(request.TaskId, request.StatusId, cancellationToken);
        }
    }
}
