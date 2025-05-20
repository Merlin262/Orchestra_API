using Orchestra.Data.Context;
using Orchestra.Models;

namespace Orchestra.Repoitories
{
    public class BpmnProcessRepository : GenericRepository<BpmnProcessBaseline>, IBpmnProcessRepository
    {
        public BpmnProcessRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
