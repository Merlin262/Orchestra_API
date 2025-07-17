using MediatR;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.BpmnBaseline.Command.DeleteBpmnProcessBaselineCommand
{
    public class DeleteBpmnProcessBaselineCommandHandler : IRequestHandler<DeleteBpmnProcessBaselineCommand, bool>
    {
        private readonly IBpmnBaselineService _bpmnBaselineService;

        public DeleteBpmnProcessBaselineCommandHandler(IBpmnBaselineService bpmnBaselineService)
        {
            _bpmnBaselineService = bpmnBaselineService;
        }

        public async Task<bool> Handle(DeleteBpmnProcessBaselineCommand request, CancellationToken cancellationToken)
        {
            var baseline = await _bpmnBaselineService.GetByIdAsync(request.Id, cancellationToken);

            if (baseline == null)
                return false;

            var hasRelatedInstances = await _bpmnBaselineService.HasRelatedInstancesAsync(request.Id, cancellationToken);

            if (hasRelatedInstances)
                return false;

            await _bpmnBaselineService.DeleteAsync(baseline, cancellationToken);

            return true;
        }
    }
}