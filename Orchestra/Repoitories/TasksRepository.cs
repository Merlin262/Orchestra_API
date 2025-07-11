﻿using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Enums;
using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.Repoitories
{
    public class TasksRepository : GenericRepository<Tasks>, ITasksRepository
    {
        private readonly ApplicationDbContext _context;

        public TasksRepository(ApplicationDbContext context) : base(context)
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
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Tasks>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Tasks
                .Include(t => t.BpmnProcess)
                .Include(t => t.ResponsibleUser)
                .Where(t => t.ResponsibleUserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<StatusEnum?> GetStatusByIdAsync(int statusId, CancellationToken cancellationToken = default)
        {
            // Verifica se o valor recebido corresponde a um valor válido do enum
            if (Enum.IsDefined(typeof(StatusEnum), statusId))
                return (StatusEnum)statusId;
            return null;
        }
        public async Task<Tasks?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
    }
}
