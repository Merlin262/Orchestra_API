using Orchestra.Data.Context;
using Orchestra.Enums;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Repoitories;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Serviecs.Intefaces;
using System.Xml.Linq;

namespace Orchestra.Services
{
    //public interface IBpmnProcessInstanceService : IGenericRepository<BpmnProcessInstance>
    //{
    //    Task<BpmnProcessBaseline?> GetBaselineAsync(int baselineId);
    //    Task<BpmnProcessInstance> CreateInstanceAsync(BpmnProcessBaseline baseline);
    //    Task<(List<ProcessStep> steps, Dictionary<string, ProcessStep> stepMap)> ParseAndCreateStepsAsync(BpmnProcessInstance instance, string? xmlContent);
    //    Task<List<Tasks>> ParseAndCreateTasksAsync(BpmnProcessInstance instance, string? xmlContent, Dictionary<string, ProcessStep> stepMap);
    //}

    public class BpmnProcessInstanceService : IBpmnProcessInstanceService
    {
        private readonly IBpmnProcessBaselineRepository _baselineRepository;
        private readonly IGenericRepository<BpmnProcessInstance> _genericRepository;
        private readonly IProcessStepRepository _stepRepository;
        private readonly ITasksRepository _tasksRepository;

        public BpmnProcessInstanceService(
            IBpmnProcessBaselineRepository baselineRepository,
            IGenericRepository<BpmnProcessInstance> genericRepository,
            IProcessStepRepository stepRepository,
            ITasksRepository tasksRepository)
        {
            _baselineRepository = baselineRepository;
            _genericRepository = genericRepository;
            _stepRepository = stepRepository;
            _tasksRepository = tasksRepository;
        }

        public Task<IEnumerable<BpmnProcessInstance>> GetAllAsync(CancellationToken cancellationToken)
            => _genericRepository.GetAllAsync(cancellationToken);

        public Task<BpmnProcessInstance?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => _genericRepository.GetByIdAsync(id, cancellationToken);

        //public async Task AddAsync(BpmnProcessInstance entity, CancellationToken cancellationToken)
        //    => await _genericRepository.AddAsync(entity, cancellationToken);
        public async Task AddAsync(BpmnProcessInstance entity, CancellationToken cancellationToken)
        {
            await _genericRepository.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(BpmnProcessInstance entity, CancellationToken cancellationToken)
            => await _genericRepository.UpdateAsync(entity, cancellationToken);

        public async Task DeleteAsync(BpmnProcessInstance entity, CancellationToken cancellationToken)
            => await _genericRepository.DeleteAsync(entity, cancellationToken);

        public async Task<BpmnProcessBaseline?> GetBaselineAsync(int baselineId)
        {
            return await _baselineRepository.GetByIdAsync(baselineId);
        }

        public async Task<BpmnProcessInstance> CreateInstanceAsync(BpmnProcessBaseline baseline)
        {
            var instance = new BpmnProcessInstance
            {
                Name = baseline.Name,
                XmlContent = baseline.XmlContent,
                BpmnProcessBaselineId = baseline.Id,
                CreatedAt = DateTime.UtcNow
            };
            await AddAsync(instance, default);
            return instance;
        }

        public async Task<(List<ProcessStep> steps, Dictionary<string, ProcessStep> stepMap)> ParseAndCreateStepsAsync(BpmnProcessInstance instance, string? xmlContent)
        {
            var xDoc = XDocument.Parse(xmlContent ?? "");
            XNamespace bpmn = "http://www.omg.org/spec/BPMN/20100524/MODEL";
            var processSteps = new List<ProcessStep>();
            var stepMap = new Dictionary<string, ProcessStep>();
            var elementTypes = new[] { "startEvent", "task", "userTask", "scriptTask", "exclusiveGateway", "endEvent" };

            foreach (var type in elementTypes)
            {
                var elements = xDoc.Descendants(bpmn + type);
                foreach (var element in elements)
                {
                    var bpmnId = element.Attribute("id")?.Value ?? Guid.NewGuid().ToString();
                    var step = new ProcessStep
                    {
                        Id = Guid.NewGuid(),
                        BpmnId = bpmnId,
                        Name = element.Attribute("name")?.Value ?? type,
                        Type = type,
                        BpmnProcessId = instance.Id
                    };
                    processSteps.Add(step);
                    stepMap[bpmnId] = step;
                }
            }

            if (processSteps.Count > 0)
            {
                await _stepRepository.AddRangeAsync(processSteps);
            }

            return (processSteps, stepMap);
        }

        public async Task<List<Tasks>> ParseAndCreateTasksAsync(BpmnProcessInstance instance, string? xmlContent, Dictionary<string, ProcessStep> stepMap)
        {
            var xDoc = XDocument.Parse(xmlContent ?? "");
            XNamespace bpmn = "http://www.omg.org/spec/BPMN/20100524/MODEL";
            var tasks = new List<Tasks>();
            var taskElements = xDoc.Descendants(bpmn + "task");
            foreach (var element in taskElements)
            {
                var bpmnId = element.Attribute("id")?.Value;
                var taskName = element.Attribute("name")?.Value;
                if (!string.IsNullOrEmpty(bpmnId) && stepMap.TryGetValue(bpmnId, out var step))
                {
                    var task = new Tasks
                    {
                        Id = Guid.NewGuid(),
                        Name = taskName,
                        XmlTaskId = bpmnId,
                        BpmnProcessId = instance.Id,
                        BpmnProcess = instance,
                        ProcessStepId = step.Id,
                        ProcessStep = step,
                        ResponsibleUserId = null,
                        ResponsibleUser = null,
                        Completed = false,
                        CreatedAt = DateTime.UtcNow,
                        CompletedAt = null,
                        Comments = null,
                        Status = StatusEnum.NotStarted
                    };
                    tasks.Add(task);
                }
            }

            if (tasks.Count > 0)
            {
                await _tasksRepository.AddRangeAsync(tasks);
            }

            return tasks;
        }

        public async Task<double> GetBaselineVersionById(double baselineId)
        {
            var baseline = await _baselineRepository.GetByIdAsync((int)baselineId);
            if (baseline == null || !baseline.Version.HasValue)
                return 0.0;
            return baseline.Version.Value;
        }
    }
}
