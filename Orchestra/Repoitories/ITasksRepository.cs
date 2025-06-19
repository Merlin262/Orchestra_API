using Orchestra.Models;

namespace Orchestra.Repoitories
{
    public interface ITasksRepository
    {
        Task AddRangeAsync(IEnumerable<Tasks> tasks);
    }
}
