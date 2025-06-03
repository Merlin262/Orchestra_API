namespace Orchestra.Dtos
{
    public class BpmnProcessBaselineWithUserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? XmlContent { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> PoolNames { get; set; }
        public double? Version { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByUserName { get; set; }
    }
}
