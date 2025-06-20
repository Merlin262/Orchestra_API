using Orchestra.Enums;

namespace Orchestra.Models
{
    namespace Orchestra.Models
    {
        public class BpmnProcessInstance
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? XmlContent { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public int BpmnProcessBaselineId { get; set; }
            public BpmnProcessBaseline BpmnProcessBaseline { get; set; } = null!;
            public List<string> PoolNames { get; set; } = new();
            public StatusEnum Status { get; set; }
        }
    }
}
