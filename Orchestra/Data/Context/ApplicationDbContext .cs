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
        public DbSet<UserGroup> UserGroups => Set<UserGroup>();
        public DbSet<SubTask> SubTasks => Set<SubTask>();   
        public DbSet<Status> Status => Set<Status>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tasks -> Status
            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.Status)
                .WithMany()
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict); // ou .NoAction()

            // SubTask -> Status
            modelBuilder.Entity<SubTask>()
                .HasOne(s => s.Status)
                .WithMany()
                .HasForeignKey(s => s.StatusId)
                .OnDelete(DeleteBehavior.Restrict); // ou .NoAction()

            // SubTask -> Tasks (essa pode ser em cascata, se desejar)
            modelBuilder.Entity<SubTask>()
                .HasOne(s => s.Task)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(s => s.TaskId)
                .OnDelete(DeleteBehavior.Cascade); // OK
        }

    }
}
