using Orchestra.Dtos;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Serviecs.Intefaces
{
    public interface ITaskService
    {
        Task<List<TaskWithUserDto>> GetTasksForProcessInstanceAsync(int processInstanceId, CancellationToken cancellationToken = default);
        Task<List<Tasks>> GetUserTasksAsync(string userId, CancellationToken cancellationToken);
        Task<List<BpmnProcessInstance>> GetProcessInstancesByIdsAsync(List<int> processInstanceIds, CancellationToken cancellationToken);
        List<ProcessInstanceWithTasksDto> MapProcessInstancesWithTasks(List<BpmnProcessInstance> processInstances, List<Tasks> userTasks);
        Task<bool> UpdateTaskStatusAsync(Guid taskId, int statusId, CancellationToken cancellationToken = default);
        Task<bool> AssignUserToTaskAsync(int bpmnProcessInstanceId, string xmlTaskId, string userId, CancellationToken cancellationToken = default);
        Task<bool> UnassignUserFromTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    }
}
