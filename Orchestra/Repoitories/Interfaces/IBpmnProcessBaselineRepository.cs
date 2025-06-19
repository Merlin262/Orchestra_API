using Orchestra.Models;

namespace Orchestra.Repoitories.Interfaces
{
    public interface IBpmnProcessBaselineRepository
    {
        Task<BpmnProcessBaseline?> GetByIdAsync(int id);
    }
}
