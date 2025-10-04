using Orchestra.Dtos;
using Orchestra.Infrastructure.Repositories;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Repoitories;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Serviecs.Intefaces;
using System.Xml.Linq;

namespace Orchestra.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly IBpmnProcessInstanceRepository _bpmnProcessInstanceRepository;
        private readonly IUserRepository _userRepository; 

        public TaskService(ITasksRepository tasksRepository, IBpmnProcessInstanceRepository bpmnProcessInstanceRepository, IUserRepository userRepository)
        {
            _tasksRepository = tasksRepository;
            _bpmnProcessInstanceRepository = bpmnProcessInstanceRepository;
            _userRepository = userRepository;
        }

        public async Task<List<TaskWithUserDto>> GetTasksForProcessInstanceAsync(int processInstanceId, CancellationToken cancellationToken = default)
        {
            var tasks = await _tasksRepository.GetByProcessInstanceIdAsync(processInstanceId, cancellationToken);

            return tasks.Select(t => new TaskWithUserDto
            {
                TaskId = t.Id,
                Name = t.Name,
                XmlTaskId = t.XmlTaskId,
                Completed = t.Completed,
                StatusId = (int)t.Status,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                Comments = t.Comments,
                Pool = t.Pool,
                ResponsibleUser = t.ResponsibleUser == null ? null : new UserDto
                {
                    Id = t.ResponsibleUser.Id,
                    UserName = t.ResponsibleUser.UserName,
                    Email = t.ResponsibleUser.Email,
                    FullName = t.ResponsibleUser.FullName,
                    //Role = t.ResponsibleUser.Role
                }
            }).ToList();
        }

        public async Task<List<Tasks>> GetTasksByProcessInstanceIdAsync(int processInstanceId, CancellationToken cancellationToken = default)
        {
            return await _tasksRepository.GetByProcessInstanceIdAsync(processInstanceId, cancellationToken);
        }

        public async Task AddTasksAsync(IEnumerable<Tasks> tasks)
        {
            await _tasksRepository.AddRangeAsync(tasks);
        }

        public async Task<List<Tasks>> GetUserTasksAsync(string userId, CancellationToken cancellationToken)
        {
            return await _tasksRepository.GetByUserIdAsync(userId, cancellationToken);
        }

        public async Task<List<BpmnProcessInstance>> GetProcessInstancesByIdsAsync(List<int> processInstanceIds, CancellationToken cancellationToken)
        {
            var result = new List<BpmnProcessInstance>();
            foreach (var id in processInstanceIds)
            {
                // Supondo que você tenha um repositório IBpmnProcessInstanceRepository injetado como _bpmnProcessInstanceRepository
                var instance = await _bpmnProcessInstanceRepository.GetByIdAsync(id, cancellationToken);
                if (instance != null)
                    result.Add(instance);
            }
            return result;
        }

        public List<ProcessInstanceWithTasksDto> MapProcessInstancesWithTasks(
            List<BpmnProcessInstance> processInstances,
            List<Tasks> userTasks)
        {
            return processInstances.Select(pi => new ProcessInstanceWithTasksDto
            {
                ProcessInstanceId = pi.Id,
                Name = pi.Name,
                XmlContent = pi.XmlContent,
                CreatedAt = pi.CreatedAt,
                BpmnProcessBaselineId = pi.BpmnProcessBaselineId,
                PoolNames = pi.PoolNames,
                Tasks = userTasks
                    .Where(t => t.BpmnProcessId == pi.Id)
                    .Select(t => MapTaskWithUserDto(t))
                    .ToList()
            }).ToList();
        }

        private TaskWithUserDto MapTaskWithUserDto(Tasks t)
        {
            return new TaskWithUserDto
            {
                TaskId = t.Id,
                Name = t.Name,
                XmlTaskId = t.XmlTaskId,
                Completed = t.Completed,
                StatusId = (int)t.Status,
                AssigmentAt = t.AssigmentAt,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                Comments = t.Comments,
                ResponsibleUser = t.ResponsibleUser == null ? null : new UserDto
                {
                    Id = t.ResponsibleUser.Id,
                    UserName = t.ResponsibleUser.UserName,
                    Email = t.ResponsibleUser.Email,
                    FullName = t.ResponsibleUser.FullName,
                    //Role = t.ResponsibleUser.Role
                }
            };
        }

        public async Task<bool> UpdateTaskStatusAsync(Guid taskId, int statusId, CancellationToken cancellationToken = default)
        {
            // Busca a task pelo repositório
            var task = await _tasksRepository.GetByIdAsync(taskId, cancellationToken);
            if (task == null)
                return false;

            // Busca o status pelo método do repositório
            var status = await _tasksRepository.GetStatusByIdAsync(statusId, cancellationToken);
            if (status == null)
                return false;

            task.Status = status.Value;

            await _tasksRepository.UpdateAsync(task, cancellationToken);

            return true;
        }

        public async Task<bool> AssignUserToTaskAsync(int processInstanceId, string xmlTaskId, string userId, CancellationToken cancellationToken = default)
        {
            // Busca a task pelo repositório
            var task = (await _tasksRepository.GetAllAsync(cancellationToken))
                .FirstOrDefault(t => t.XmlTaskId == xmlTaskId && t.BpmnProcessId == processInstanceId);

            task.AssigmentAt = DateTime.UtcNow;

            if (task == null)
                return false;

            // Busca o usuário pelo repositório
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return false;

            task.ResponsibleUserId = user.Id;
            //task.ResponsibleUser = user;

            await _tasksRepository.UpdateAsync(task, cancellationToken);

            return true;
        }

        public async Task<bool> UnassignUserFromTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            var task = await _tasksRepository.GetByIdAsync(taskId, cancellationToken);
            if (task == null || string.IsNullOrEmpty(task.ResponsibleUserId))
                return false;

            task.AssigmentAt = null;

            task.ResponsibleUserId = null;
            task.ResponsibleUser = null;

            await _tasksRepository.UpdateAsync(task, cancellationToken);

            return true;
        }

        public async Task<bool> SetTaskPoolAsync(Tasks task, string xmlContent, CancellationToken cancellationToken = default)
        {
            // Use GetTasksWithLanes to find the lane/pool for the given task
            var tasksWithLanes = GetTasksWithLanes(xmlContent);
            var laneTuple = tasksWithLanes.FirstOrDefault(t => t.TaskName == task.Name);
            var laneName = laneTuple.LaneName;
            if (string.IsNullOrEmpty(laneName))
                return false;

            task.Pool = laneName;
            await _tasksRepository.UpdateAsync(task, cancellationToken);
            return true;
        }

        /// <summary>
        /// Identifica a pool da task pelo XmlTaskId e salva no banco.
        /// Busca recursivamente em lanes e childLaneSet.
        /// </summary>
        public static List<(string TaskName, string LaneName)> GetTasksWithLanes(string bpmnXml)
        {
            var doc = XDocument.Parse(bpmnXml);

            // Namespaces usados no BPMN
            XNamespace bpmn = "http://www.omg.org/spec/BPMN/20100524/MODEL";
            XNamespace di = "http://www.omg.org/spec/BPMN/20100524/DI";
            XNamespace dc = "http://www.omg.org/spec/DD/20100524/DC";

            // 1. Mapear lanes -> Bounds
            var lanes = doc.Descendants(bpmn + "lane")
                .Select(l => new
                {
                    Id = l.Attribute("id")?.Value,
                    Name = l.Attribute("name")?.Value
                }).ToList();

            var laneBounds = doc.Descendants(di + "BPMNShape")
                .Where(s => lanes.Any(l => l.Id == s.Attribute("bpmnElement")?.Value))
                .Select(s => new
                {
                    LaneId = s.Attribute("bpmnElement")?.Value,
                    Bounds = s.Element(dc + "Bounds")
                })
                .ToDictionary(
                    s => s.LaneId,
                    s => new
                    {
                        X = double.Parse(s.Bounds.Attribute("x")?.Value ?? "0"),
                        Y = double.Parse(s.Bounds.Attribute("y")?.Value ?? "0"),
                        Width = double.Parse(s.Bounds.Attribute("width")?.Value ?? "0"),
                        Height = double.Parse(s.Bounds.Attribute("height")?.Value ?? "0")
                    }
                );

            // 2. Mapear tasks -> Bounds
            var tasks = doc.Descendants(bpmn + "task")
                .Select(t => new
                {
                    Id = t.Attribute("id")?.Value,
                    Name = t.Attribute("name")?.Value
                }).ToList();

            var taskBounds = doc.Descendants(di + "BPMNShape")
                .Where(s => tasks.Any(t => t.Id == s.Attribute("bpmnElement")?.Value))
                .Select(s => new
                {
                    TaskId = s.Attribute("bpmnElement")?.Value,
                    Bounds = s.Element(dc + "Bounds")
                })
                .ToDictionary(
                    s => s.TaskId,
                    s => new
                    {
                        X = double.Parse(s.Bounds.Attribute("x")?.Value ?? "0"),
                        Y = double.Parse(s.Bounds.Attribute("y")?.Value ?? "0"),
                        Width = double.Parse(s.Bounds.Attribute("width")?.Value ?? "0"),
                        Height = double.Parse(s.Bounds.Attribute("height")?.Value ?? "0")
                    }
                );

            var result = new List<(string TaskName, string LaneName)>();

            // 3. Para cada task, verificar em qual lane está
            foreach (var task in tasks)
            {
                if (!taskBounds.TryGetValue(task.Id, out var tb))
                    continue;

                string laneName = "Desconhecida";

                foreach (var lane in lanes)
                {
                    if (!laneBounds.TryGetValue(lane.Id, out var lb))
                        continue;

                    bool insideX = tb.X >= lb.X && (tb.X + tb.Width) <= (lb.X + lb.Width);
                    bool insideY = tb.Y >= lb.Y && (tb.Y + tb.Height) <= (lb.Y + lb.Height);

                    if (insideX && insideY)
                    {
                        laneName = lane.Name;
                        break;
                    }
                }

                result.Add((task.Name, laneName));
            }

            return result;
        }


    }
}
