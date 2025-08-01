using Orchestra.Enums;

namespace Orchestra.Models
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<Role> Roles { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Tasks> AssignedTasks { get; set; }
        public ProfileTypeEnum ProfileType { get; set; }
    }
}
