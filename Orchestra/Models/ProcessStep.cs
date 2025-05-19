namespace Orchestra.Models
{
    public class ProcessStep
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BpmnId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!; // Ex: StartEvent, Task, Gateway

        public int BpmnProcessId { get; set; } 
        public string ResponsibleUserId { get; set; } = null!;
        public BpmnProcess BpmnProcess { get; set; } = null!; 
    }
}
