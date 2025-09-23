using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Repoitories.Interfaces
{
    public interface IBpmnProcessInstanceRepository : IGenericRepository<BpmnProcessInstance>
    {
        Task<BpmnProcessInstance> AddAsync(BpmnProcessInstance instance, CancellationToken cancellationToken = default);
        Task<List<BpmnProcessInstance>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken = default);
        Task<IEnumerable<BpmnProcessInstance>> GetAllWithUserAsync(CancellationToken cancellationToken = default);
    }
}
