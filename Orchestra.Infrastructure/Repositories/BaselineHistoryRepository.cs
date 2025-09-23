using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Domain.Repositories;
using Orchestra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestra.Infrastructure.Repositories
{
    public class BaselineHistoryRepository : IBaselineHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public BaselineHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BaselineHistory>> GetByBaselineIdAsync(int baselineId, CancellationToken cancellationToken)
        {
            return await _context.BaselineHistories
                .AsNoTracking()
                .Where(h => h.Id == baselineId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BaselineHistory>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.BaselineHistories
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<BaselineHistory?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.BaselineHistories
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        }

        public async Task AddAsync(BaselineHistory entity, CancellationToken cancellationToken = default)
        {
            await _context.BaselineHistories.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(BaselineHistory entity, CancellationToken cancellationToken = default)
        {
            _context.BaselineHistories.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(BaselineHistory entity, CancellationToken cancellationToken = default)
        {
            _context.BaselineHistories.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
