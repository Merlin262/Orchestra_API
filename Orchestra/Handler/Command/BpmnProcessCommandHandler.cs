using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models;
using System.Xml;
using System.Xml.Linq;

namespace Orchestra.Handler.Command
{
    public class BpmnProcessCommandHandler : IRequestHandler<BpmnProcessCommand, BpmnProcess>
    {
        private readonly ApplicationDbContext _context;
        public BpmnProcessCommandHandler(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<BpmnProcess> Handle(BpmnProcessCommand request, CancellationToken cancellationToken)
        {
            string xmlContent;
            using (var reader = new StreamReader(request.File.OpenReadStream()))
            {
                xmlContent = await reader.ReadToEndAsync();
            }

            var process = new BpmnProcess
            {
                Name = request.Name,
                XmlContent = xmlContent,
                CreatedAt = DateTime.UtcNow
            };

            _context.BpmnProcess.Add(process);
            await _context.SaveChangesAsync(cancellationToken);

            await ParseAndSaveSteps(xmlContent, process.Id, cancellationToken);

            return process;
        }


        public async Task ParseAndSaveSteps(string xmlContent, int bpmnProcessId, CancellationToken cancellationToken)
        {
            var xDoc = XDocument.Parse(xmlContent);
            XNamespace bpmn = "http://www.omg.org/spec/BPMN/20100524/MODEL";

            var steps = new List<ProcessStep>();

            // Lista de elementos de interesse (pode ser expandida)
            var elementTypes = new[] { "startEvent", "task", "userTask", "scriptTask", "exclusiveGateway", "endEvent" };

            foreach (var type in elementTypes)
            {
                var elements = xDoc.Descendants(bpmn + type);
                foreach (var element in elements)
                {
                    var step = new ProcessStep
                    {
                        Id = Guid.NewGuid(),
                        BpmnId = element.Attribute("id")?.Value ?? Guid.NewGuid().ToString(),
                        Name = element.Attribute("name")?.Value ?? type,
                        //Type = type,
                        BpmnProcessId = bpmnProcessId
                    };
                    steps.Add(step);
                }
            }

            if (steps.Any())
            {
                _context.ProcessStep.AddRange(steps);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
