using Orchestra.Models;

namespace Orchestra.Repoitories.Interfaces
{
    public interface ITasksRepository : IGenericRepository<Tasks>
    {
        Task AddRangeAsync(IEnumerable<Tasks> tasks);
        Task<List<Tasks>> GetByProcessInstanceIdAsync(int processInstanceId, CancellationToken cancellationToken = default);
        Task<List<Tasks>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Status?> GetStatusByIdAsync(int statusId, CancellationToken cancellationToken = default);
        Task<Tasks?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
