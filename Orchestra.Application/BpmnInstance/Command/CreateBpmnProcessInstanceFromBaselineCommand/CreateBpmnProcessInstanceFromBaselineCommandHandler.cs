using MediatR;
using Orchestra.Domain.Services;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Services;
using Orchestra.Serviecs.Intefaces;
using Orchestra.Domain.Repositories;

namespace Orchestra.Handler.BpmnInstance.Command.CreateBpmnProcessInstanceFromBaselineCommand
{
    public class CreateBpmnProcessInstanceFromBaselineCommandHandler : IRequestHandler<CreateBpmnProcessInstanceFromBaselineCommand, BpmnProcessInstance>
    {
        private readonly IBpmnProcessInstanceService _service;
        private readonly ISubProcessRepository _subProcessRepository;

        public CreateBpmnProcessInstanceFromBaselineCommandHandler(
            IBpmnProcessInstanceService service,
            ISubProcessRepository subProcessRepository)
        {
            _service = service;
            _subProcessRepository = subProcessRepository;
        }

        public async Task<BpmnProcessInstance> Handle(CreateBpmnProcessInstanceFromBaselineCommand request, CancellationToken cancellationToken)
        {
            var baseline = await _service.GetBaselineAsync(request.BaselineId, cancellationToken);
            if (baseline == null)
                throw new Exception($"BpmnProcessBaseline com id {request.BaselineId} não encontrado.");

            // Consulta os subprocessos e adiciona ao baseline
            baseline.SubProcesses = await _subProcessRepository.GetByBaselineIdAsync(baseline.Id, cancellationToken);

            var instance = await _service.CreateInstanceAsync(request.UserId, baseline, cancellationToken, request.Name, request.Description);
            (List<ProcessStep> _, Dictionary<string, ProcessStep> stepMap) = await _service.ParseAndCreateStepsAsync(instance, baseline.XmlContent);
            await _service.ParseAndCreateTasksAsync(instance, baseline.XmlContent, stepMap, cancellationToken);

            // Criar tasks dos SubProcesses
            if (baseline.SubProcesses != null)
            {
                foreach (var subProcess in baseline.SubProcesses)
                {
                    if (!string.IsNullOrWhiteSpace(subProcess.XmlContent))
                    {
                        (List<ProcessStep> _, Dictionary<string, ProcessStep> subStepMap) = await _service.ParseAndCreateStepsAsync(instance, subProcess.XmlContent);
                        await _service.ParseAndCreateTasksAsync(instance, subProcess.XmlContent, subStepMap, cancellationToken);
                    }
                }
            }

            return instance;
        }
    }
}
