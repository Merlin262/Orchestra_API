using Orchestra.Data.Context;
using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.Repoitories
{
    public class BpmnProcessRepository : GenericRepository<BpmnProcessBaseline>, IBpmnProcessRepository
    {
        public BpmnProcessRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
