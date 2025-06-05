namespace Orchestra.Models
{
    public class SubTask
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid TaskId { get; set; }
        public Tasks Task { get; set; }
    }
}
