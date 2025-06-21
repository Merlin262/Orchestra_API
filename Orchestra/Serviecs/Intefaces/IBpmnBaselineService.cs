namespace Orchestra.Serviecs.Intefaces
{
    public interface IBpmnBaselineService
    {
        List<string> ExtractPoolNames(string xmlContent);
    }
}
