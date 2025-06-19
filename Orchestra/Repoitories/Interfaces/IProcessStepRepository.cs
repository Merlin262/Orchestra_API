using Orchestra.Models;

namespace Orchestra.Repoitories.Interfaces
{
    public interface IProcessStepRepository
    {
        Task AddRangeAsync(IEnumerable<ProcessStep> steps);
    }
}
