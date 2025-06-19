using MediatR;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Services;

namespace Orchestra.Handler.BpmnInstance.Command
{
    public class CreateBpmnProcessInstanceFromBaselineCommandHandler : IRequestHandler<CreateBpmnProcessInstanceFromBaselineCommand, BpmnProcessInstance>
    {
        private readonly IBpmnProcessInstanceService _service;

        public CreateBpmnProcessInstanceFromBaselineCommandHandler(IBpmnProcessInstanceService service)
        {
            _service = service;
        }

        public async Task<BpmnProcessInstance> Handle(CreateBpmnProcessInstanceFromBaselineCommand request, CancellationToken cancellationToken)
        {
            var baseline = await _service.GetBaselineAsync(request.BaselineId);
            if (baseline == null)
                throw new Exception($"BpmnProcessBaseline com id {request.BaselineId} não encontrado.");

            var instance = await _service.CreateInstanceAsync(baseline);
            var (_, stepMap) = await _service.ParseAndCreateStepsAsync(instance, baseline.XmlContent);
            await _service.ParseAndCreateTasksAsync(instance, baseline.XmlContent, stepMap);

            return instance;
        }
    }
}
