using Orchestra.Dtos;
using Orchestra.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestra.Serviecs.Intefaces
{
    public interface IBpmnBaselineService
    {
        List<string> ExtractPoolNames(string xmlContent);
        string FixDataObjectToDataObjectReference(string xmlContent);
        Task<bool> HasRelatedInstancesAsync(int baselineId, CancellationToken cancellationToken);
        Task<BpmnProcessBaseline?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task DeleteAsync(BpmnProcessBaseline baseline, CancellationToken cancellationToken);
        Task<BpmnProcessBaseline> AddBaselineAsync(BpmnProcessBaseline baseline, CancellationToken cancellationToken);
        Task<BpmnProcessBaseline?> GetByBaselineIdAndVersionAsync(int baselineId, double version, CancellationToken cancellationToken);
        Task<BpmnProcessBaseline> UpdateBaselineAsync(BpmnProcessBaseline baseline, CancellationToken cancellationToken);
        Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
        Task AddBaselineHistoryAsync(BaselineHistory history, CancellationToken cancellationToken);
        Task AddProcessStepsAsync(IEnumerable<ProcessStep> steps, CancellationToken cancellationToken);
        Task<List<BpmnProcessBaselineWithUserDto>> GetBaselinesByUserIdAsync(string userId, CancellationToken cancellationToken);
    }
}
