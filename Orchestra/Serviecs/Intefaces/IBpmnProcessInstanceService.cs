using Orchestra.Models.Orchestra.Models;
using Orchestra.Models;

namespace Orchestra.Serviecs.Intefaces
{
    public interface IBpmnProcessInstanceService
    {
        Task<IEnumerable<BpmnProcessInstance>> GetAllAsync(CancellationToken cancellationToken);
        Task<BpmnProcessInstance?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(BpmnProcessInstance entity, CancellationToken cancellationToken);
        Task UpdateAsync(BpmnProcessInstance entity, CancellationToken cancellationToken);
        Task DeleteAsync(BpmnProcessInstance entity, CancellationToken cancellationToken);
        Task<BpmnProcessInstance> CreateInstanceAsync(BpmnProcessBaseline baseline);
        Task<(List<ProcessStep> steps, Dictionary<string, ProcessStep> stepMap)> ParseAndCreateStepsAsync(BpmnProcessInstance instance, string? xmlContent);
        Task<List<Tasks>> ParseAndCreateTasksAsync(BpmnProcessInstance instance, string? xmlContent, Dictionary<string, ProcessStep> stepMap);
        Task<BpmnProcessBaseline?> GetBaselineAsync(int baselineId, CancellationToken cancellationToken);
    }
}
