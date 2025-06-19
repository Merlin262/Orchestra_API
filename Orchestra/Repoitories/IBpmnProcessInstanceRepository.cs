using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Repoitories
{
    public interface IBpmnProcessInstanceRepository
    {
        Task<BpmnProcessInstance> AddAsync(BpmnProcessInstance instance);
    }
}
