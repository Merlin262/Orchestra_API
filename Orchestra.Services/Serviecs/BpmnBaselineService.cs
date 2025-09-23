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
            var instances = await _instanceService.GetAllAsync(cancellationToken);
            return instances.Any(i => i.BpmnProcessBaselineId == baselineId);
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
    }
}
