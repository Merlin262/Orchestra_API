using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.Repoitories
{
    public class BpmnProcessBaselineRepository : GenericRepository<BpmnProcessBaseline>, IBpmnProcessBaselineRepository
    {
        private readonly ApplicationDbContext _context;

        public BpmnProcessBaselineRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<BpmnProcessBaseline?> GetByBaselineIdAndVersionAsync(int baselineId, double version, CancellationToken cancellationToken)
        {
            return await _context.BpmnProcess
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id != baselineId && x.Version == version, cancellationToken);
        }

        public async Task<BpmnProcessBaseline?> GetByNameAndVersionAsync(string name, double version, CancellationToken cancellationToken)
        {
            return await _context.BpmnProcess
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name && x.Version == version, cancellationToken);
        }
    }
}