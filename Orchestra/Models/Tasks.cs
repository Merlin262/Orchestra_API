using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Models
{
    public class Tasks
    {
        public Guid Id { get; set; }
        public int BpmnProcessId { get; set; }
        public BpmnProcessInstance BpmnProcess { get; set; }
        public Guid ProcessStepId { get; set; }
        public ProcessStep ProcessStep { get; set; }
        public string ResponsibleUserId { get; set; }
        public User ResponsibleUser { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public string? Comments { get; set; } 
    }
}
