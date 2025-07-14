namespace Orchestra.Handler.BpmnInstance.Query.GetAllProcessInstance
{
    public class GetBpmnProcessInstancesQueryResult
    {
        public int id { get; set; }
        public string? Name { get; set; }
        public string? XmlContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public int BpmnProcessBaselineId { get; set; }
        public double? Version { get; set; }
    }
}
