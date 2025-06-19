using Orchestra.Data.Context;
using Orchestra.Models;

namespace Orchestra.Repoitories
{
    public class ProcessStepRepository : IProcessStepRepository
    {
        private readonly ApplicationDbContext _context;

        public ProcessStepRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<ProcessStep> steps)
        {
            _context.ProcessStep.AddRange(steps);
            await _context.SaveChangesAsync();
        }
    }
}
