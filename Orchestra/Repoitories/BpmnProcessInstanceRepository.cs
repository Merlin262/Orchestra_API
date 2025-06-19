using Orchestra.Data.Context;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Repoitories
{
    public class BpmnProcessInstanceRepository : IBpmnProcessInstanceRepository
    {
        private readonly ApplicationDbContext _context;
        public BpmnProcessInstanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BpmnProcessInstance> AddAsync(BpmnProcessInstance instance)
        {
            _context.bpmnProcessInstances.Add(instance);
            await _context.SaveChangesAsync();
            return instance;
        }
    }
}
