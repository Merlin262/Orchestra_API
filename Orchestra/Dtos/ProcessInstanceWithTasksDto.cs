namespace Orchestra.Dtos
{
    public class ProcessInstanceWithTasksDto
    {
        public int ProcessInstanceId { get; set; }
        public string? Name { get; set; }
        public string? XmlContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public int BpmnProcessBaselineId { get; set; }
        public List<string> PoolNames { get; set; }
        public List<TaskWithUserDto> Tasks { get; set; }
    }
}
