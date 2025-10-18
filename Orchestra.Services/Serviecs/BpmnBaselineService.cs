using Orchestra.Domain.Services;
using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Serviecs.Intefaces;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Orchestra.Infrastructure.Repositories;
using Orchestra.Dtos;
using Orchestra.Domain.Repositories;
using System.Linq;

namespace Orchestra.Serviecs
{
    public class BpmnBaselineService : IBpmnBaselineService
    {
        private readonly IBpmnProcessInstanceService _instanceService;
        private readonly IBpmnProcessBaselineRepository _bpmnProcessBaselineRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBaselineHistoryRepository _baselineHistoryRepository;
        private readonly IProcessStepRepository _processStepRepository;

        public BpmnBaselineService(
            IBpmnProcessInstanceService instanceService,
            IBpmnProcessBaselineRepository bpmnProcessBaselineRepository,
            IUserRepository userRepository,
            IBaselineHistoryRepository baselineHistoryRepository,
            IProcessStepRepository processStepRepository)
        {
            _instanceService = instanceService;
            _bpmnProcessBaselineRepository = bpmnProcessBaselineRepository;
            _userRepository = userRepository;
            _baselineHistoryRepository = baselineHistoryRepository;
            _processStepRepository = processStepRepository;
        }

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

        public async Task<bool> HasRelatedInstancesAsync(int baselineId, CancellationToken cancellationToken)
        {
            var baseline = await _bpmnProcessBaselineRepository.GetByIdAsync(baselineId, cancellationToken);
            if (baseline == null)
            {
                return false;
            }

            var instances = await _instanceService.GetAllAsync(cancellationToken);
            return instances.Any(i => i.BpmnProcessBaselineId == baselineId && i.version == baseline.Version);
        }

        public async Task<BpmnProcessBaseline?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _bpmnProcessBaselineRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task DeleteAsync(BpmnProcessBaseline baseline, CancellationToken cancellationToken)
        {
            await _bpmnProcessBaselineRepository.DeleteAsync(baseline, cancellationToken);
        }

        public async Task<BpmnProcessBaseline> AddBaselineAsync(BpmnProcessBaseline baseline, CancellationToken cancellationToken)
        {
            await _bpmnProcessBaselineRepository.AddAsync(baseline, cancellationToken);
            return baseline;
        }

        public async Task<BpmnProcessBaseline?> GetByBaselineIdAndVersionAsync(int baselineId, double version, CancellationToken cancellationToken)
        {
            return await _bpmnProcessBaselineRepository.GetByBaselineIdAndVersionAsync(baselineId, version, cancellationToken);
        }

        public async Task<BpmnProcessBaseline> UpdateBaselineAsync(BpmnProcessBaseline baseline, CancellationToken cancellationToken)
        {
            await _bpmnProcessBaselineRepository.UpdateAsync(baseline, cancellationToken);
            return baseline;
        }

        public async Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _userRepository.GetByIdAsync(userId, cancellationToken);
        }

        public async Task AddBaselineHistoryAsync(BaselineHistory history, CancellationToken cancellationToken)
        {
            await _baselineHistoryRepository.AddAsync(history, cancellationToken);
        }

        public async Task<IEnumerable<BaselineHistory>> GetBaselineHistoryByBaselineIdAsync(int baselineId, CancellationToken cancellationToken)
        {
            return await _baselineHistoryRepository.GetByBaselineIdAsync(baselineId, cancellationToken);
        }

        public async Task DeleteBaselineHistoryAsync(BaselineHistory history, CancellationToken cancellationToken)
        {
            await _baselineHistoryRepository.DeleteAsync(history, cancellationToken);
        }

        public async Task AddProcessStepsAsync(IEnumerable<ProcessStep> steps, CancellationToken cancellationToken)
        {
            await _processStepRepository.AddRangeAsync(steps);
        }

        public static string ConvertAssociationToDataOutputAssociation(string xmlContent)
        {
            var doc = XDocument.Parse(xmlContent, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

            // Namespace BPMN (assume que o namespace padrão do root é o BPMN model namespace)
            XNamespace bpmn = doc.Root.GetDefaultNamespace();

            // Coleta todas as associations existentes (em lista para evitar alteração durante iteração)
            var associations = doc.Descendants(bpmn + "association").ToList();

            foreach (var assoc in associations)
            {
                string assocId = (string)assoc.Attribute("id");
                string sourceRef = (string)assoc.Attribute("sourceRef");
                string targetRef = (string)assoc.Attribute("targetRef");

                // monta o novo elemento dataOutputAssociation com o MESMO id (importante para preservar DI)
                var dataOutput = new XElement(bpmn + "dataOutputAssociation",
                    new XAttribute("id", assocId)
                );

                // adiciona sourceRef/targetRef como nós filhos (formato aceito pelos viewers)
                if (!string.IsNullOrEmpty(sourceRef))
                    dataOutput.Add(new XElement(bpmn + "sourceRef", sourceRef));
                if (!string.IsNullOrEmpty(targetRef))
                    dataOutput.Add(new XElement(bpmn + "targetRef", targetRef));

                // Se a association tinha extensionElements, copie para o novo elemento
                var ext = assoc.Elements().FirstOrDefault(e => e.Name.LocalName == "extensionElements");
                if (ext != null)
                {
                    // clone extensionElements (mantém conteúdo)
                    dataOutput.Add(new XElement(ext));
                }

                // tenta localizar a task cujo id == sourceRef
                XElement task = null;
                if (!string.IsNullOrEmpty(sourceRef))
                {
                    task = doc.Descendants(bpmn + "task")
                              .FirstOrDefault(t => (string)t.Attribute("id") == sourceRef);
                }

                if (task != null)
                {
                    // evita duplicidade de id dentro da task
                    bool alreadyHas = task.DescendantsAndSelf().Any(e => (string)e.Attribute("id") == assocId);
                    if (!alreadyHas)
                    {
                        task.Add(dataOutput);
                    }
                    // remove a antiga association (no nível do processo)
                    assoc.Remove();
                }
                else
                {
                    // se não encontrou a task, substitui a association no mesmo lugar por dataOutputAssociation
                    assoc.ReplaceWith(dataOutput);
                }
            }

            return doc.ToString();
        }

        public async Task<List<BpmnProcessBaselineWithUserDto>> GetBaselinesByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            var baselines = await _bpmnProcessBaselineRepository.GetAllAsync(cancellationToken);
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            var userFullName = user?.FullName;

            return baselines
                .Where(b => b.CreatedByUserId == userId)
                .Select(b => new BpmnProcessBaselineWithUserDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    XmlContent = b.XmlContent,
                    CreatedAt = b.CreatedAt,
                    PoolNames = b.PoolNames,
                    CreatedBy = b.CreatedByUserId,
                    Version = b.Version,
                    CreatedByUserName = userFullName,
                    Description = b.Description
                })
                .ToList();
        }

        public async Task<List<BpmnProcessBaseline>> GetAllBaselinesByNameAsync(string name, CancellationToken cancellationToken)
        {
            var baselines = await _bpmnProcessBaselineRepository.GetAllAsync(cancellationToken);
            return baselines
                .Where(b => b.Name == name)
                .OrderByDescending(b => b.Version)
                .ToList();
        }

        // Novo método para verificar subprocessos no XML
        public bool HasSubProcessInXml(string xmlContent)
        {
            var xDoc = XDocument.Parse(xmlContent);
            // Busca por callActivity, que representa subprocessos
            return xDoc.Descendants().Any(e => e.Name.LocalName == "callActivity");
        }

        // Novo método para retornar nomes dos subprocessos
        public List<string> GetSubProcessNamesFromXml(string xmlContent)
        {
            var xDoc = XDocument.Parse(xmlContent);
            // Busca por callActivity, que representa subprocessos, e retorna o atributo name
            return xDoc.Descendants()
                .Where(e => e.Name.LocalName == "callActivity")
                .Select(e => (string)e.Attribute("name"))
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();
        }

        private Dictionary<string, List<string>> GetNextStepMap(XDocument xDoc, XNamespace bpmn)
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

        private Dictionary<string, List<string>> GetPreviousStepMap(XDocument xDoc, XNamespace bpmn)
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

        public async Task ParseAndSaveStepsAsync(string xmlContent, int bpmnProcessId, CancellationToken cancellationToken)
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
                await _processStepRepository.AddRangeAsync(steps);
            }
        }
    }
}
