using Orchestra.Data.Context;
using Orchestra.Models;

namespace Orchestra.Repoitories
{
    public class BpmnProcessRepository : GenericRepository<BpmnProcess>, IBpmnProcessRepository
    {
        public BpmnProcessRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
