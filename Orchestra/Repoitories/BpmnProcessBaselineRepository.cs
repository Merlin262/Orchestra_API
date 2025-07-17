using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.Repoitories
{
    public class BpmnProcessBaselineRepository : GenericRepository<BpmnProcessBaseline>, IBpmnProcessBaselineRepository
    {

        public BpmnProcessBaselineRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}