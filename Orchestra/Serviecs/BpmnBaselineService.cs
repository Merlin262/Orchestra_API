using System.Xml.Linq;

namespace Orchestra.Serviecs
{
    public class BpmnBaselineService
    {
        public List<string> ExtractPoolNames(string xmlContent)
        {
            var pools = new List<string>();
            var doc = XDocument.Parse(xmlContent);
            XNamespace ns = "http://www.omg.org/spec/BPMN/20100524/MODEL";
            pools = doc.Descendants(ns + "lane")
                       .Select(l => (string)l.Attribute("name"))
                       .Where(n => !string.IsNullOrWhiteSpace(n))
                       .ToList();
            return pools;
        }
    }
}
