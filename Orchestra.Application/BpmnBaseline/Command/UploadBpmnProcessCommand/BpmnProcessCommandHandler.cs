using MediatR;
using Orchestra.Models;
using Orchestra.Serviecs;
using Orchestra.Serviecs.Intefaces;
using System.Xml;
using System.Xml.Linq;

namespace Orchestra.Handler.BpmnBaseline.Command.UploadBpmnProcessCommand
{
    public class BpmnProcessCommandHandler : IRequestHandler<BpmnProcessCommand, BpmnProcessBaseline>
    {
        private readonly IBpmnBaselineService _bpmnBaselineService;

        public BpmnProcessCommandHandler(IBpmnBaselineService bpmnBaselineService)
        {
            _bpmnBaselineService = bpmnBaselineService;
        }

        public async Task<BpmnProcessBaseline> Handle(BpmnProcessCommand request, CancellationToken cancellationToken)
        {
            string xmlContent;
            using (var reader = new StreamReader(request.File.OpenReadStream()))
            {
                xmlContent = await reader.ReadToEndAsync();
            }

            // Corrige os dataObject para dataObjectReference
            xmlContent = _bpmnBaselineService.FixDataObjectToDataObjectReference(xmlContent);

            // Transforma associações
            xmlContent = BpmnBaselineService.ConvertAssociationToDataOutputAssociation(xmlContent);

            var poolNames = _bpmnBaselineService.ExtractPoolNames(xmlContent);

            var createdByUser = await _bpmnBaselineService.GetUserByIdAsync(request.UserId, cancellationToken);

            var process = new BpmnProcessBaseline
            {
                Name = request.Name,
                XmlContent = xmlContent,
                CreatedAt = DateTime.UtcNow,
                PoolNames = poolNames,
                CreatedByUser = createdByUser,
                Version = 1.0,
                Description = request.Description,
                IsActive = true
            };

            await _bpmnBaselineService.AddBaselineAsync(process, cancellationToken);

            // Save BaselineHistory (Upload)
            var history = new BaselineHistory
            {
                BpmnProcessBaselineId = process.Id,
                Name = process.Name,
                XmlContent = process.XmlContent,
                Description = process.Description,
                Version = process.Version,
                ChangedBy = process.CreatedByUser?.Id, // FK do usuário
                ChangedAt = process.CreatedAt,
                ChangeType = "Upload",
                Responsible = process.CreatedByUser?.Id // FK do usuário
            };
            await _bpmnBaselineService.AddBaselineHistoryAsync(history, cancellationToken);

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
                        Type = type,
                        BpmnProcessId = bpmnProcessId
                    };
                    steps.Add(step);
                }
            }

            if (steps.Any())
            {
                await _bpmnBaselineService.AddProcessStepsAsync(steps, cancellationToken);
            }
        }
    }
}
