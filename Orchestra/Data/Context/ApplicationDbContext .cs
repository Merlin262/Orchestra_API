using Microsoft.EntityFrameworkCore;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Data.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<BpmnProcessBaseline> BpmnProcess => Set<BpmnProcessBaseline>();
        public DbSet<ProcessStep> ProcessStep => Set<ProcessStep>();
        public DbSet<BpmnItem> BpmnItem => Set<BpmnItem>();
        public DbSet<BpmnProcessInstance> bpmnProcessInstances => Set<BpmnProcessInstance>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Tasks> Tasks => Set<Tasks>();
        public DbSet<BaselineHistory> BaselineHistories => Set<BaselineHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Roles)
                .HasConversion(
                    roles => string.Join(',', roles),
                    value => value.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            base.OnModelCreating(modelBuilder);
        }

    }
}
