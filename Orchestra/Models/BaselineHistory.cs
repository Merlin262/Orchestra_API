using System;

namespace Orchestra.Models
{
    public class BaselineHistory
    {
        public int Id { get; set; }
        public int BpmnProcessBaselineId { get; set; }
        public BpmnProcessBaseline BpmnProcessBaseline { get; set; }
        public string? Name { get; set; }
        public string? XmlContent { get; set; }
        public string? Description { get; set; }
        public double Version { get; set; }
        public string? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string ChangeType { get; set; } = null!; 
        public string? Responsible { get; set; }
        public bool IsActive { get; set; }
    }
}
