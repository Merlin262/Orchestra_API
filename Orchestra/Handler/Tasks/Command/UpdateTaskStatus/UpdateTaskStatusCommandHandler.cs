using MediatR;
using Orchestra.Dtos;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.Tasks.Command.UpdateTaskStatus
{
    public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, UpdateTaskStatusResultDto>
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly ITaskService _taskService;

        public UpdateTaskStatusCommandHandler(ITasksRepository tasksRepository, ITaskService taskService)
        {
            _tasksRepository = tasksRepository;
            _taskService = taskService;
        }

        public async Task<UpdateTaskStatusResultDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var success = await _taskService.UpdateTaskStatusAsync(request.TaskId, request.StatusId, cancellationToken);

            var task = await _tasksRepository.GetByIdAsync(request.TaskId, cancellationToken);

            return new UpdateTaskStatusResultDto
            {
                Success = success,
                XmlTaskId = task?.XmlTaskId
            };
        }
    }
}