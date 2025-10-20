using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orchestra.Domain.Models;
using Orchestra.Domain.Repositories;
using Orchestra.Data.Context;

namespace Orchestra.Infrastructure.Repositories
{
    public class SubProcessRepository : ISubProcessRepository
    {
        private readonly ApplicationDbContext _context;
        public SubProcessRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SubProcess> AddAsync(SubProcess subProcess, CancellationToken cancellationToken = default)
        {
            _context.SubProcesses.Add(subProcess);
            await _context.SaveChangesAsync(cancellationToken);
            return subProcess;
        }

        public async Task<List<SubProcess>> GetByBaselineIdAsync(int processBaselineId, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => _context.SubProcesses
                .Where(sp => sp.ProcessBaselineId == processBaselineId)
                .ToList(), cancellationToken);
        }

        public async Task<List<SubProcess>> GetByBaselineHistoryIdAsync(int baselineHistoryId, CancellationToken cancellationToken = default)
        {
            return await _context.SubProcesses
                .Where(sp => sp.BaselineHistoryId == baselineHistoryId)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(SubProcess subProcess, CancellationToken cancellationToken = default)
        {
            _context.SubProcesses.Update(subProcess);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(SubProcess subProcess, CancellationToken cancellationToken = default)
        {
            _context.SubProcesses.Remove(subProcess);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
