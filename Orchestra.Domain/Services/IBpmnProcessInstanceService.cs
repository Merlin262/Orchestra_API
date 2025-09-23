using Orchestra.Dtos;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Models;

namespace Orchestra.Domain.Services;

public interface IBpmnProcessInstanceService
{
    Task<IEnumerable<BpmnProcessInstance>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<BpmnProcessInstance>> GetAllWithUserAsync(CancellationToken cancellationToken);
    Task<BpmnProcessInstance?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(BpmnProcessInstance entity, CancellationToken cancellationToken);
    Task UpdateAsync(BpmnProcessInstance entity, CancellationToken cancellationToken);
    Task DeleteAsync(BpmnProcessInstance entity, CancellationToken cancellationToken);
    Task<BpmnProcessInstance> CreateInstanceAsync(string createdByUserId, BpmnProcessBaseline baseline, CancellationToken cancellationToken, string? name = null, string? description = null); 
    Task<(List<ProcessStep> steps, Dictionary<string, ProcessStep> stepMap)> ParseAndCreateStepsAsync(BpmnProcessInstance instance, string? xmlContent);
    Task<List<Tasks>> ParseAndCreateTasksAsync(BpmnProcessInstance instance, string? xmlContent, Dictionary<string, ProcessStep> stepMap);
    Task<BpmnProcessBaseline?> GetBaselineAsync(int baselineId, CancellationToken cancellationToken);
    Task<List<ProcessInstanceWithTasksDto>> GetProcessInstancesByResponsibleUserAsync(string userId, CancellationToken cancellationToken); 
}
