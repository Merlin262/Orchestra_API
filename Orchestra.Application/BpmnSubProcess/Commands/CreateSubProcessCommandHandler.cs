using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Orchestra.Domain.Models;
using Orchestra.Domain.Repositories;
using Orchestra.Serviecs.Intefaces;
using Orchestra.Serviecs;

namespace Orchestra.Application.BpmnSubProcess.Commands
{
    public class CreateSubProcessCommandHandler : IRequestHandler<CreateSubProcessCommand, SubProcess>
    {
        private readonly ISubProcessRepository _repository;
        private readonly IBpmnBaselineService _bpmnBaselineService;
        public CreateSubProcessCommandHandler(ISubProcessRepository repository, IBpmnBaselineService bpmnBaselineService)
        {
            _repository = repository;
            _bpmnBaselineService = bpmnBaselineService;
        }

        public async Task<SubProcess> Handle(CreateSubProcessCommand request, CancellationToken cancellationToken)
        {
            string? xmlContent = request.XmlContent;
            if (!string.IsNullOrWhiteSpace(xmlContent))
            {
                // Aplica os mesmos ajustes de formatação do BpmnBaselineService
                xmlContent = _bpmnBaselineService.FixDataObjectToDataObjectReference(xmlContent);
                xmlContent = BpmnBaselineService.ConvertAssociationToDataOutputAssociation(xmlContent);
            }

            // Busca a baseline para obter a versão
            var baseline = await _bpmnBaselineService.GetByIdAsync(request.ProcessBaselineId, cancellationToken);
            if (baseline == null)
            {
                throw new Exception($"Baseline com id {request.ProcessBaselineId} não encontrada.");
            }

            // Busca o histórico da baseline com a versão atual
            var baselineHistories = await _bpmnBaselineService.GetBaselineHistoryByBaselineIdAsync(request.ProcessBaselineId, cancellationToken);
            var baselineHistory = baselineHistories
                .Where(h => h.Version == baseline.Version)
                .OrderByDescending(h => h.ChangedAt)
                .FirstOrDefault();

            if (baselineHistory == null)
            {
                throw new Exception($"BaselineHistory não encontrado para a baseline {request.ProcessBaselineId} versão {baseline.Version}.");
            }

            var subProcess = new SubProcess
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ProcessBaselineId = request.ProcessBaselineId,
                UserId = request.UserId,
                XmlContent = xmlContent, // Salva o conteúdo do arquivo, já ajustado
                BaselineVersion = baseline.Version,
                BaselineHistoryId = baselineHistory.Id
            };
            await _repository.AddAsync(subProcess, cancellationToken);
            return subProcess;
        }
    }
}