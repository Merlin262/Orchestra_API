using Orchestra.Dtos;
using Orchestra.Repoitories.Interfaces; // Certifique-se de usar o namespace correto
using Microsoft.EntityFrameworkCore;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITasksRepository _tasksRepository;

        public TaskService(ITasksRepository tasksRepository)
        {
            _tasksRepository = tasksRepository;
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
                StatusId = t.StatusId,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                Comments = t.Comments,
                ResponsibleUser = t.ResponsibleUser == null ? null : new UserDto
                {
                    Id = t.ResponsibleUser.Id,
                    UserName = t.ResponsibleUser.UserName,
                    Email = t.ResponsibleUser.Email,
                    FullName = t.ResponsibleUser.FullName,
                    Role = t.ResponsibleUser.Role
                }
            }).ToList();
        }

    }
}
