namespace Orchestra.Models
{
    public class BpmnProcessBaseline
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? XmlContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<string> PoolNames { get; set; } = new();
        public string? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
        public double Version { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}