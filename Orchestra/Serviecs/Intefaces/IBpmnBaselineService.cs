using Orchestra.Models;

namespace Orchestra.Serviecs.Intefaces
{
    public interface IBpmnBaselineService
    {
        List<string> ExtractPoolNames(string xmlContent);
        string FixDataObjectToDataObjectReference(string xmlContent);
        Task<bool> HasRelatedInstancesAsync(int baselineId, CancellationToken cancellationToken);
        Task<BpmnProcessBaseline?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task DeleteAsync(BpmnProcessBaseline baseline, CancellationToken cancellationToken);
    }
}
