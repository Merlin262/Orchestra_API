using MediatR;
using Orchestra.Dtos;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.BpmnBaseline.Querry.GetByUser
{
    public class GetBpmnProcessByUserIdQueryHandler : IRequestHandler<GetProcessBaselineByUser, List<BpmnProcessBaselineWithUserDto>>
    {
        private readonly IBpmnBaselineService _bpmnBaselineService;

        public GetBpmnProcessByUserIdQueryHandler(IBpmnBaselineService bpmnBaselineService)
        {
            _bpmnBaselineService = bpmnBaselineService;
        }

        public async Task<List<BpmnProcessBaselineWithUserDto>> Handle(GetProcessBaselineByUser request, CancellationToken cancellationToken)
        {
            return await _bpmnBaselineService.GetBaselinesByUserIdAsync(request.UserId, cancellationToken);
        }
    }
}
