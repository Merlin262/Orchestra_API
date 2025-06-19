using Orchestra.Models;

namespace Orchestra.Repoitories
{
    public interface IProcessStepRepository
    {
        Task AddRangeAsync(IEnumerable<ProcessStep> steps);
    }
}
