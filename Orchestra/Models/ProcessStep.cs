namespace Orchestra.Models
{
    public class ProcessStep
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BpmnId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int BpmnProcessId { get; set; }
        public string NextStepId { get; set; }
        public string LastStepId { get; set; }
    }
}
