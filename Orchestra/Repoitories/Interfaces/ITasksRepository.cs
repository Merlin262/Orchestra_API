using Orchestra.Models;

namespace Orchestra.Repoitories.Interfaces
{
    public interface ITasksRepository
    {
        Task AddRangeAsync(IEnumerable<Tasks> tasks);
        Task<List<Tasks>> GetByProcessInstanceIdAsync(int processInstanceId, CancellationToken cancellationToken = default);

    }
}
