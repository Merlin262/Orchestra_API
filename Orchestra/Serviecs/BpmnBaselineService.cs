using Orchestra.Serviecs.Intefaces;
using System.Xml.Linq;

namespace Orchestra.Serviecs
{
    public class BpmnBaselineService : IBpmnBaselineService
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

        public string FixDataObjectToDataObjectReference(string xmlContent)
        {
            var doc = XDocument.Parse(xmlContent);
            XNamespace bpmn = "http://www.omg.org/spec/BPMN/20100524/MODEL";

            var dataObjects = doc.Descendants(bpmn + "dataObject").ToList();

            foreach (var dataObject in dataObjects)
            {
                var attributes = new List<XAttribute>();

                var id = dataObject.Attribute("id")?.Value;
                if (!string.IsNullOrEmpty(id))
                    attributes.Add(new XAttribute("id", id));

                var name = dataObject.Attribute("name")?.Value;
                if (!string.IsNullOrEmpty(name))
                    attributes.Add(new XAttribute("name", name));

                var dataObjectReference = new XElement(bpmn + "dataObjectReference", attributes);

                foreach (var child in dataObject.Elements())
                {
                    dataObjectReference.Add(new XElement(child));
                }

                dataObject.ReplaceWith(dataObjectReference);
            }

            return doc.ToString();
        }
    }
}
