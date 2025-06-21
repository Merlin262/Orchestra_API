namespace Orchestra.Serviecs.Intefaces
{
    public interface IBpmnBaselineService
    {
        public List<string> ExtractPoolNames(string xmlContent);
        public string FixDataObjectToDataObjectReference(string xmlContent);
    }
}
