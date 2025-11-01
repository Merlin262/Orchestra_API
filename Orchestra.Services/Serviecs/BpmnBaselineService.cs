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

            XNamespace bpmn = doc.Root.GetDefaultNamespace();

            var associations = doc.Descendants(bpmn + "association").ToList();

            foreach (var assoc in associations)
            {
                string assocId = (string)assoc.Attribute("id");
                string sourceRef = (string)assoc.Attribute("sourceRef");
                string targetRef = (string)assoc.Attribute("targetRef");

                // Verifica se sourceRef é um DataObject, DataObjectReference ou DataStoreReference
                var sourceIsDataObject = doc.Descendants()
                    .Any(e => (e.Name.LocalName == "dataObject" 
                              || e.Name.LocalName == "dataObjectReference"
                              || e.Name.LocalName == "dataStoreReference") 
                              && (string)e.Attribute("id") == sourceRef);

                // Verifica se targetRef é um DataObject, DataObjectReference ou DataStoreReference
                var targetIsDataObject = doc.Descendants()
                    .Any(e => (e.Name.LocalName == "dataObject" 
                              || e.Name.LocalName == "dataObjectReference"
                              || e.Name.LocalName == "dataStoreReference") 
                              && (string)e.Attribute("id") == targetRef);

                XElement targetElement = null;
                XElement newAssociation = null;

                // CASO 1: DataObject/DataStoreReference -> Task (dataInputAssociation dentro da task)
                if (sourceIsDataObject && !string.IsNullOrEmpty(targetRef))
                {
                    // Busca o elemento de destino (task, userTask, etc)
                    targetElement = doc.Descendants()
                        .FirstOrDefault(e => (e.Name.LocalName == "task" 
                                            || e.Name.LocalName == "userTask" 
                                            || e.Name.LocalName == "serviceTask"
                                            || e.Name.LocalName == "scriptTask"
                                            || e.Name.LocalName == "manualTask"
                                            || e.Name.LocalName == "businessRuleTask"
                                            || e.Name.LocalName == "sendTask"
                                            || e.Name.LocalName == "receiveTask")
                                            && (string)e.Attribute("id") == targetRef);

                    if (targetElement != null)
                    {
                        // Cria dataInputAssociation
                        newAssociation = new XElement(bpmn + "dataInputAssociation",
                            new XAttribute("id", assocId)
                        );

                        // sourceRef é o DataObject/DataStoreReference
                        newAssociation.Add(new XElement(bpmn + "sourceRef", sourceRef));
                        
                        // targetRef é a propriedade/campo da task (opcional)
                        if (!string.IsNullOrEmpty(targetRef))
                            newAssociation.Add(new XElement(bpmn + "targetRef", targetRef));
                    }
                }
                // CASO 2: Task -> DataObject/DataStoreReference (dataOutputAssociation dentro da task)
                else if (targetIsDataObject && !string.IsNullOrEmpty(sourceRef))
                {
                    // Busca a task de origem
                    targetElement = doc.Descendants()
                        .FirstOrDefault(e => (e.Name.LocalName == "task" 
                                            || e.Name.LocalName == "userTask" 
                                            || e.Name.LocalName == "serviceTask"
                                            || e.Name.LocalName == "scriptTask"
                                            || e.Name.LocalName == "manualTask"
                                            || e.Name.LocalName == "businessRuleTask"
                                            || e.Name.LocalName == "sendTask"
                                            || e.Name.LocalName == "receiveTask")
                                            && (string)e.Attribute("id") == sourceRef);

                    if (targetElement != null)
                    {
                        // Cria dataOutputAssociation
                        newAssociation = new XElement(bpmn + "dataOutputAssociation",
                            new XAttribute("id", assocId)
                        );

                        // sourceRef é a propriedade/campo da task (opcional)
                        if (!string.IsNullOrEmpty(sourceRef))
                            newAssociation.Add(new XElement(bpmn + "sourceRef", sourceRef));
                        
                        // targetRef é o DataObject/DataStoreReference
                        newAssociation.Add(new XElement(bpmn + "targetRef", targetRef));
                    }
                }

                // Se encontrou um elemento de destino e criou a nova associação
                if (targetElement != null && newAssociation != null)
                {
                    // Copia extensionElements se existir
                    var ext = assoc.Elements().FirstOrDefault(e => e.Name.LocalName == "extensionElements");
                    if (ext != null)
                    {
                        newAssociation.Add(new XElement(ext));
                    }

                    // Evita duplicidade de id dentro do elemento
                    bool alreadyHas = targetElement.DescendantsAndSelf().Any(e => (string)e.Attribute("id") == assocId);
                    if (!alreadyHas)
                    {
                        targetElement.Add(newAssociation);
                    }

                    // Remove a antiga association
                    assoc.Remove();
                }
                else
                {
                    // Se não conseguiu processar, mantém como dataOutputAssociation (comportamento anterior)
                    var dataOutput = new XElement(bpmn + "dataOutputAssociation",
                        new XAttribute("id", assocId)
                    );

                    if (!string.IsNullOrEmpty(sourceRef))
                        dataOutput.Add(new XElement(bpmn + "sourceRef", sourceRef));
                    if (!string.IsNullOrEmpty(targetRef))
                        dataOutput.Add(new XElement(bpmn + "targetRef", targetRef));

                    var ext = assoc.Elements().FirstOrDefault(e => e.Name.LocalName == "extensionElements");
                    if (ext != null)
                    {
                        dataOutput.Add(new XElement(ext));
                    }

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
