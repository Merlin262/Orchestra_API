using Microsoft.EntityFrameworkCore;
using Orchestra.Dtos;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Repoitories;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Serviecs.Intefaces;

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

        public async Task<bool> AssignUserToTaskAsync(string xmlTaskId, string userId, CancellationToken cancellationToken = default)
        {
            // Busca a task pelo repositório
            var task = (await _tasksRepository
                .GetAllAsync(cancellationToken))
                .FirstOrDefault(t => t.XmlTaskId == xmlTaskId);

            if (task == null)
                return false;

            // Busca o usuário pelo repositório
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return false;

            task.ResponsibleUserId = user.Id;
            task.ResponsibleUser = user;

            await _tasksRepository.UpdateAsync(task, cancellationToken);

            return true;
        }
    }
}
