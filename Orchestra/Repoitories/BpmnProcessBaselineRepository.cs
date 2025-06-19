using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models;

namespace Orchestra.Repoitories
{
    public class BpmnProcessBaselineRepository : IBpmnProcessBaselineRepository
    {
        private readonly ApplicationDbContext _context;

        public BpmnProcessBaselineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BpmnProcessBaseline?> GetByIdAsync(int id)
        {
            return await _context.BpmnProcess.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
