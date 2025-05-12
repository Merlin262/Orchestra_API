namespace Orchestra.Models
{
    public class ProcessStep
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BpmnId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!; // Ex: StartEvent, Task, Gateway

        public int BpmnProcessId { get; set; } // Chave estrangeira
        public BpmnProcess BpmnProcess { get; set; } = null!; // Propriedade de navegação
    }


}
