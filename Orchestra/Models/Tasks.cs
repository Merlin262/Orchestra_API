using Microsoft.Identity.Client;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Models
{
    public class Tasks
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string XmlTaskId { get; set; }
        public int BpmnProcessId { get; set; }
        public BpmnProcessInstance BpmnProcess { get; set; }
        public Guid ProcessStepId { get; set; }
        public ProcessStep ProcessStep { get; set; }
        public string? ResponsibleUserId { get; set; }
        public User ResponsibleUser { get; set; }
        public bool Completed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? ExpectedConclusionDate { get; set; }
        public string? Comments { get; set; }
        public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
        public int StatusId { get; set; }
        public Status Status { get; set; }
    }
}
