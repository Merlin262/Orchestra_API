using Microsoft.EntityFrameworkCore;
using Orchestra.Models;

namespace Orchestra.Data.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<BpmnProcess> BpmnProcess => Set<BpmnProcess>();
        public DbSet<ProcessStep> ProcessStep => Set<ProcessStep>();
        public DbSet<BpmnItem> BpmnItem => Set<BpmnItem>();
    }
}
