using Orchestra.Domain.Services;
using Orchestra.Dtos;
using Orchestra.Enums;
using Orchestra.Infrastructure.Repositories;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Repoitories;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Serviecs.Intefaces;
using System.Xml.Linq;

namespace Orchestra.Services
{
    public class BpmnProcessInstanceService : IBpmnProcessInstanceService
    {
        private readonly IBpmnProcessBaselineRepository _baselineRepository;
        private readonly IGenericRepository<BpmnProcessInstance> _genericRepository;
        private readonly IBpmnProcessInstanceRepository _instanceRepository;
        private readonly IProcessStepRepository _stepRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITasksRepository _tasksRepository;
        private readonly ITaskService _taskService;

        public BpmnProcessInstanceService(
            IBpmnProcessBaselineRepository baselineRepository,
            IGenericRepository<BpmnProcessInstance> genericRepository,
            IBpmnProcessInstanceRepository instanceRepository,
            IProcessStepRepository stepRepository,
            IUserRepository userRepository,
            ITasksRepository tasksRepository,
            ITaskService taskService)
        {
            _baselineRepository = baselineRepository;
            _genericRepository = genericRepository;
            _instanceRepository = instanceRepository;
            _stepRepository = stepRepository;
            _userRepository = userRepository;
            _tasksRepository = tasksRepository;
            _taskService = taskService;
        }

        public Task<IEnumerable<BpmnProcessInstance>> GetAllAsync(CancellationToken cancellationToken)
            => _genericRepository.GetAllAsync(cancellationToken);

        public Task<BpmnProcessInstance?> GetByIdAsync(int id, CancellationToken cancellationToken)
            => _genericRepository.GetByIdAsync(id, cancellationToken);

        public async Task AddAsync(BpmnProcessInstance entity, CancellationToken cancellationToken)
        {
            await _genericRepository.AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(BpmnProcessInstance entity, CancellationToken cancellationToken)
            => await _genericRepository.UpdateAsync(entity, cancellationToken);

        public async Task DeleteAsync(BpmnProcessInstance entity, CancellationToken cancellationToken)
            => await _genericRepository.DeleteAsync(entity, cancellationToken);

        public async Task<BpmnProcessBaseline?> GetBaselineAsync(int baselineId, CancellationToken cancellationToken)
        {
            return await _baselineRepository.GetByIdAsync(baselineId, cancellationToken);
        }

        public async Task<BpmnProcessInstance> CreateInstanceAsync(BpmnProcessBaseline baseline)
        {
            var instance = new BpmnProcessInstance
            {
                Name = baseline.Name,
                XmlContent = baseline.XmlContent,
                BpmnProcessBaselineId = baseline.Id,
                CreatedAt = DateTime.UtcNow,
                version = baseline.Version
            };
            await AddAsync(instance, default);
            return instance;
        }

        public async Task<BpmnProcessInstance> CreateInstanceAsync(string createdByUserId, BpmnProcessBaseline baseline, CancellationToken cancellationToken, string? name = null, string? description = null)
        {
            var user = await _userRepository.GetByIdAsync(createdByUserId, cancellationToken);
            if (user == null)
                throw new Exception($"Usuário com id {createdByUserId} não encontrado.");

            var instance = new BpmnProcessInstance
            {
                Name = name ?? baseline.Name,
                XmlContent = baseline.XmlContent,
                BpmnProcessBaselineId = baseline.Id,
                PoolNames = baseline.PoolNames,
                CreatedAt = DateTime.UtcNow,
                version = baseline.Version,
                Description = description ?? baseline.Description,
                CreatedById = user.Id // Correção: atribuir o ID à chave estrangeira
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
            var taskElements = xDoc.Descendants(bpmn + "task").Concat(xDoc.Descendants(bpmn + "userTask"));
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
                // Após salvar, atualize o Pool de cada task usando o método do TaskService
                foreach (var task in tasks)
                {
                    await _taskService.SetTaskPoolAsync(task, xmlContent ?? "");
                }
            }

            return tasks;
        }

        public async Task<List<ProcessInstanceWithTasksDto>> GetProcessInstancesByResponsibleUserAsync(string userId, CancellationToken cancellationToken)
        {
            var userTasks = await _tasksRepository.GetByUserIdAsync(userId, cancellationToken);
            if (!userTasks.Any())
                return new List<ProcessInstanceWithTasksDto>();

            var processInstanceIds = userTasks.Select(t => t.BpmnProcessId).Distinct().ToList();
            var processInstances = new List<BpmnProcessInstance>();
            foreach (var id in processInstanceIds)
            {
                var instance = await _genericRepository.GetByIdAsync(id, cancellationToken);
                if (instance != null)
                    processInstances.Add(instance);
            }

            // Buscar usuários criadores
            var createdByIds = processInstances
                .Select(pi => pi.CreatedById)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            // Buscar todos os usuários criadores
            var users = new List<User>();
            foreach (var id in createdByIds)
            {
                var user = await _userRepository.GetByIdAsync(id, cancellationToken);
                if (user != null)
                    users.Add(user);
            }

            var result = processInstances.Select(pi => {
                var user = users.FirstOrDefault(u => u.Id == pi.CreatedById);
                return new ProcessInstanceWithTasksDto
                {
                    ProcessInstanceId = pi.Id,
                    Name = pi.Name,
                    CreatedAt = pi.CreatedAt,
                    BpmnProcessBaselineId = pi.BpmnProcessBaselineId,
                    PoolNames = pi.PoolNames,
                    version = pi.version,
                    Status = pi.Status,
                    ResponsibleName = user?.FullName,
                    Tasks = userTasks
                        .Where(t => t.BpmnProcessId == pi.Id)
                        .Select(t => new TaskWithUserDto
                        {
                            TaskId = t.Id,
                            Name = t.Name,
                            XmlTaskId = t.XmlTaskId,
                            Completed = t.Completed,
                            StatusId = (int)t.Status,
                            CreatedAt = t.CreatedAt,
                            AssigmentAt = t.AssigmentAt,
                            CompletedAt = t.CompletedAt,
                            Comments = t.Comments,
                            ResponsibleUser = t.ResponsibleUser == null ? null : new UserDto
                            {
                                Id = t.ResponsibleUser.Id,
                                UserName = t.ResponsibleUser.UserName,
                                Email = t.ResponsibleUser.Email,
                                //Roles = t.ResponsibleUser.Roles
                            }
                        })
                        .ToList()
                };
            }).ToList();

            return result;
        }

        //public async Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        //{
        //    return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        //}

        public async Task<IEnumerable<BpmnProcessInstance>> GetAllWithUserAsync(CancellationToken cancellationToken)
        {
            return await _instanceRepository.GetAllWithUserAsync(cancellationToken);
        }
    }
}
