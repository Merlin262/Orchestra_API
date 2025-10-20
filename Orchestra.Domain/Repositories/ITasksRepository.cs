using Orchestra.Enums;
using Orchestra.Models;

namespace Orchestra.Repoitories.Interfaces
{
    public interface ITasksRepository : IGenericRepository<Tasks>
    {
        Task AddRangeAsync(IEnumerable<Tasks> tasks);
        Task<List<Tasks>> GetByProcessInstanceIdAsync(int processInstanceId, CancellationToken cancellationToken = default);
        Task<List<Tasks>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<StatusEnum?> GetStatusByIdAsync(int statusId, CancellationToken cancellationToken = default);
        Task<Tasks?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Tasks>> GetTasksWithSubProcessIdAsync(Guid subProcessId, int bpmnProcessId, CancellationToken cancellationToken = default);
    }
}
