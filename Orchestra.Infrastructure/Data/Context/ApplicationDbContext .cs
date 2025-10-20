using Microsoft.EntityFrameworkCore;
using Orchestra.Domain.Models;
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
        public DbSet<Role> Roles { get; set; }
        public DbSet<TaskFile> TaskFiles { get; set; }
        public DbSet<BaselineFile> BaselineFiles { get; set; }
        public DbSet<SubProcess> SubProcesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da relação muitos-para-muitos User <-> Role
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users);

            // Configuração da relação TaskFile -> UploadedBy (User)
            modelBuilder.Entity<TaskFile>()
                .HasOne(tf => tf.UploadedBy)
                .WithMany()
                .HasForeignKey(tf => tf.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict); // ou DeleteBehavior.NoAction

            // Configuração da relação BaselineFile -> UploadedBy (User)
            modelBuilder.Entity<BaselineFile>()
                .HasOne(bf => bf.UploadedBy)
                .WithMany()
                .HasForeignKey(bf => bf.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração da relação BaselineFile -> Baseline
            modelBuilder.Entity<BaselineFile>()
                .HasOne(bf => bf.Baseline)
                .WithMany()
                .HasForeignKey(bf => bf.BaselineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubProcess>()
                .HasMany(s => s.Tasks)
                .WithOne(t => t.SubProcess)
                .HasForeignKey(t => t.SubProcessId)
                .OnDelete(DeleteBehavior.Restrict); // ou .NoAction
            
            modelBuilder.Entity<SubProcess>()
               .HasOne(sp => sp.BaselineHistory)
               .WithMany()
               .HasForeignKey(sp => sp.BaselineHistoryId)
               .OnDelete(DeleteBehavior.NoAction); // Remove cascata

        }

    }
}
