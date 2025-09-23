using Orchestra.Models;

namespace Orchestra.Dtos
{
    public class TaskWithUserDto
    {
        public Guid TaskId { get; set; }
        public string Name { get; set; }
        public string XmlTaskId { get; set; }
        public bool Completed { get; set; } = false;
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Comments { get; set; }
        public UserDto? ResponsibleUser { get; set; }
        public string? Pool { get; set; }
    }
}
