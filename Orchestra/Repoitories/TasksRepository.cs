using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.Repoitories
{
    public class TasksRepository : ITasksRepository
    {
        private readonly ApplicationDbContext _context;

        public TasksRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Tasks> tasks)
        {
            _context.Tasks.AddRange(tasks);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Tasks>> GetByProcessInstanceIdAsync(int processInstanceId, CancellationToken cancellationToken = default)
        {
            return await _context.Tasks
                .Where(t => t.BpmnProcessId == processInstanceId)
                .Include(t => t.ResponsibleUser)
                .Include(t => t.Status)
                .ToListAsync(cancellationToken);
        }
    }
}
