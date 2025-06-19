using Orchestra.Dtos;

namespace Orchestra.Serviecs.Intefaces
{
    public interface ITaskService
    {
        Task<List<TaskWithUserDto>> GetTasksForProcessInstanceAsync(int processInstanceId, CancellationToken cancellationToken = default);
    }
}
