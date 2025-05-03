namespace Orchestra.Models
{
    public class BpmnProcess
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? XmlContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
