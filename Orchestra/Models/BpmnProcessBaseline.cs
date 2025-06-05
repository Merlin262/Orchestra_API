namespace Orchestra.Models
{
    public class BpmnProcessBaseline
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? XmlContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<string> PoolNames { get; set; } = new();
        public string? CreatedBy { get; set; }
        public User? CreatedByUser { get; set; }
        public double? Version { get; set; }
    }
}