namespace Orchestra.Dtos
{
    public class TaskWithUserDto
    {
        public Guid TaskId { get; set; }
        public string Name { get; set; }
        public string XmlTaskId { get; set; }
        public bool Completed { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Comments { get; set; }
        public UserDto? ResponsibleUser { get; set; }
    }
}
