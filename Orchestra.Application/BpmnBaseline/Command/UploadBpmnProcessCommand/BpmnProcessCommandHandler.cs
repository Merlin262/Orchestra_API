using MediatR;
using Orchestra.Models;
using Orchestra.Serviecs;
using Orchestra.Serviecs.Intefaces;
using System.Xml;
using System.Xml.Linq;

namespace Orchestra.Handler.BpmnBaseline.Command.UploadBpmnProcessCommand
{
    public class BpmnProcessCommandHandler : IRequestHandler<BpmnProcessCommand, BpmnProcessResult>
    {
        private readonly IBpmnBaselineService _bpmnBaselineService;

        public BpmnProcessCommandHandler(IBpmnBaselineService bpmnBaselineService)
        {
            _bpmnBaselineService = bpmnBaselineService;
        }

        public async Task<BpmnProcessResult> Handle(BpmnProcessCommand request, CancellationToken cancellationToken)
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
                //CreatedByUser = createdByUser,
                CreatedByUserId = createdByUser.Id,
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

            // Verifica se o processo possui subprocessos analisando o XML
            bool hasSubProcess = _bpmnBaselineService.HasSubProcessInXml(xmlContent);
            List<string> subProcessNames = _bpmnBaselineService.GetSubProcessNamesFromXml(xmlContent);

            // Retorno do resultado com o campo HasSubProcess e nomes dos subprocessos
            return new BpmnProcessResult
            {
                Process = process,
                Items = null, // Preencha se necessário
                HasSubProcess = hasSubProcess,
                SubProcessNames = subProcessNames
            };
        }

        public Dictionary<string, List<string>> GetNextStepMap(XDocument xDoc, XNamespace bpmn)
        {
            // Mapeia todos os sequenceFlows: sourceRef -> targetRef
            var sequenceFlows = xDoc.Descendants(bpmn + "sequenceFlow")
                .Select(sf => new {
                    SourceRef = sf.Attribute("sourceRef")?.Value,
                    TargetRef = sf.Attribute("targetRef")?.Value
                })
                .Where(sf => sf.SourceRef != null && sf.TargetRef != null)
                .ToList();

            // Cria um dicionário de NextStepId: elementoId -> lista de próximos ids
            var nextStepMap = sequenceFlows
                .GroupBy(sf => sf.SourceRef)
                .ToDictionary(g => g.Key, g => g.Select(sf => sf.TargetRef).ToList());

            return nextStepMap;
        }

        public Dictionary<string, List<string>> GetPreviousStepMap(XDocument xDoc, XNamespace bpmn)
        {
            // Mapeia todos os sequenceFlows: targetRef -> sourceRef
            var sequenceFlows = xDoc.Descendants(bpmn + "sequenceFlow")
                .Select(sf => new {
                    SourceRef = sf.Attribute("sourceRef")?.Value,
                    TargetRef = sf.Attribute("targetRef")?.Value
                })
                .Where(sf => sf.SourceRef != null && sf.TargetRef != null)
                .ToList();

            // Cria um dicionário de PreviousStepId: elementoId -> lista de anteriores ids
            var previousStepMap = sequenceFlows
                .GroupBy(sf => sf.TargetRef)
                .ToDictionary(g => g.Key, g => g.Select(sf => sf.SourceRef).ToList());

            return previousStepMap;
        }

        public async Task ParseAndSaveSteps(string xmlContent, int bpmnProcessId, CancellationToken cancellationToken)
        {
            var xDoc = XDocument.Parse(xmlContent);
            XNamespace bpmn = "http://www.omg.org/spec/BPMN/20100524/MODEL";

            var steps = new List<ProcessStep>();

            // Lista de elementos de interesse (pode ser expandida)
            var elementTypes = new[] { "startEvent", "task", "userTask", "scriptTask", "exclusiveGateway", "endEvent" };

            // Mapeia todos os elementos de interesse pelo id
            var elementIdToType = xDoc.Descendants()
                .Where(e => elementTypes.Contains(e.Name.LocalName))
                .ToDictionary(e => e.Attribute("id")?.Value, e => e.Name.LocalName);

            // Obtém o mapeamento dos próximos passos
            var nextStepMap = GetNextStepMap(xDoc, bpmn);
            // Obtém o mapeamento dos passos anteriores
            var previousStepMap = GetPreviousStepMap(xDoc, bpmn);

            foreach (var kvp in elementIdToType)
            {
                var elementId = kvp.Key;
                var type = kvp.Value;
                var element = xDoc.Descendants(bpmn + type).FirstOrDefault(e => e.Attribute("id")?.Value == elementId);
                if (element == null) continue;

                var nextStepIds = nextStepMap.ContainsKey(elementId) ? nextStepMap[elementId] : new List<string>();
                var previousStepIds = previousStepMap.ContainsKey(elementId) ? previousStepMap[elementId] : new List<string>();
                // Para simplificação, salva apenas o primeiro NextStepId e o primeiro LastStepId (pode ser adaptado para múltiplos)
                var step = new ProcessStep
                {
                    Id = Guid.NewGuid(),
                    BpmnId = elementId ?? Guid.NewGuid().ToString(),
                    Name = element.Attribute("name")?.Value ?? type,
                    NextStepId = nextStepIds.FirstOrDefault(),
                    LastStepId = previousStepIds.FirstOrDefault(),
                    Type = type,
                    BpmnProcessId = bpmnProcessId
                };
                steps.Add(step);
            }

            if (steps.Any())
            {
                await _bpmnBaselineService.AddProcessStepsAsync(steps, cancellationToken);
            }
        }
    }
}
