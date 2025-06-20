using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.Repoitories
{
    public class BpmnProcessInstanceRepository : GenericRepository<BpmnProcessInstance>, IBpmnProcessInstanceRepository
    {
        private readonly ApplicationDbContext _context;

        public BpmnProcessInstanceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<BpmnProcessInstance> AddAsync(BpmnProcessInstance instance, CancellationToken cancellationToken = default)
        {
            await base.AddAsync(instance, cancellationToken);
            return instance;
        }

        public async Task<List<BpmnProcessInstance>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken = default)
        {
            return await _context.bpmnProcessInstances
                .Where(pi => ids.Contains(pi.Id))
                .ToListAsync(cancellationToken);
        }
    }

}
