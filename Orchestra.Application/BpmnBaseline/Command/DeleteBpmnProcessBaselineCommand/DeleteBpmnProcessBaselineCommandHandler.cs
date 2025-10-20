using MediatR;
using Orchestra.Domain.Repositories;
using Orchestra.Serviecs.Intefaces;
using System.Linq;

namespace Orchestra.Handler.BpmnBaseline.Command.DeleteBpmnProcessBaselineCommand
{
    public class DeleteBpmnProcessBaselineCommandHandler : IRequestHandler<DeleteBpmnProcessBaselineCommand, DeleteBpmnProcessBaselineCommandResult>
    {
        private readonly IBpmnBaselineService _bpmnBaselineService;
        private readonly ISubProcessRepository _subProcessRepository;

        public DeleteBpmnProcessBaselineCommandHandler(
            IBpmnBaselineService bpmnBaselineService,
            ISubProcessRepository subProcessRepository)
        {
            _bpmnBaselineService = bpmnBaselineService;
            _subProcessRepository = subProcessRepository;
        }

        public async Task<DeleteBpmnProcessBaselineCommandResult> Handle(DeleteBpmnProcessBaselineCommand request, CancellationToken cancellationToken)
        {
            var baseline = await _bpmnBaselineService.GetByIdAsync(request.Id, cancellationToken);

            if (baseline == null)
                return new DeleteBpmnProcessBaselineCommandResult 
                { 
                    Success = false, 
                    ErrorMessage = "Baseline não encontrada." 
                };

            var hasRelatedInstances = await _bpmnBaselineService.HasRelatedInstancesAsync(request.Id, cancellationToken);

            if (hasRelatedInstances)
                return new DeleteBpmnProcessBaselineCommandResult 
                { 
                    Success = false, 
                    ErrorMessage = "Não é possível excluir esta Baseline, pois ela está vinculada a uma ou mais instâncias."
                };

            // Se a versão for maior que 1.0, restaurar a versão anterior do histórico
            if (baseline.Version > 1.0)
            {
                // Buscar o histórico de baselines pelo BpmnProcessBaselineId
                var histories = await _bpmnBaselineService.GetBaselineHistoryByBaselineIdAsync(baseline.Id, cancellationToken);
                var historyList = histories.OrderByDescending(h => h.Version).ToList();

                // Encontrar a versão anterior (segunda mais recente no histórico)
                var previousHistory = historyList.Skip(1).FirstOrDefault();

                if (previousHistory != null)
                {
                    // Atualizar a baseline com os dados da versão anterior
                    baseline.Name = previousHistory.Name;
                    baseline.XmlContent = previousHistory.XmlContent;
                    baseline.Description = previousHistory.Description;
                    baseline.Version = previousHistory.Version;
                    baseline.CreatedAt = previousHistory.ChangedAt;
                    baseline.IsActive = previousHistory.IsActive;

                    await _bpmnBaselineService.UpdateBaselineAsync(baseline, cancellationToken);

                    // Deletar o último registro do histórico (versão que está sendo removida)
                    var latestHistory = historyList.FirstOrDefault();
                    if (latestHistory != null)
                    {
                        // Buscar todos os SubProcesses que referenciam este histórico
                        var subProcesses = await _subProcessRepository.GetByBaselineHistoryIdAsync(latestHistory.Id, cancellationToken);
                        
                        // Deletar os SubProcesses ligados ao histórico mais recente
                        foreach (var subProcess in subProcesses)
                        {
                            await _subProcessRepository.DeleteAsync(subProcess, cancellationToken);
                        }

                    // Agora pode deletar o histórico sem violar a constraint
                        await _bpmnBaselineService.DeleteBaselineHistoryAsync(latestHistory, cancellationToken);
                    }

                    return new DeleteBpmnProcessBaselineCommandResult 
                    { 
                        Success = true 
                    };
                }
                else
                {
                    // Se não encontrou versão anterior no histórico, não pode deletar
                    return new DeleteBpmnProcessBaselineCommandResult 
                    { 
                        Success = false, 
                        ErrorMessage = "Não foi possível encontrar a versão anterior no histórico." 
                    };
                }
            }
            else
            {
                // Se for versão 1.0, deletar completamente a baseline
                await _bpmnBaselineService.DeleteAsync(baseline, cancellationToken);

                return new DeleteBpmnProcessBaselineCommandResult 
                { 
                    Success = true 
                };
            }
        }
    }
}