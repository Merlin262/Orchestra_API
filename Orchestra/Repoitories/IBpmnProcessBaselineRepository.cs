using Orchestra.Models;

namespace Orchestra.Repoitories
{
    public interface IBpmnProcessBaselineRepository
    {
        Task<BpmnProcessBaseline?> GetByIdAsync(int id);
    }
}
