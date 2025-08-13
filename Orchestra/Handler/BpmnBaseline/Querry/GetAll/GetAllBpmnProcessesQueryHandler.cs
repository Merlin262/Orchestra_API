using System.Linq;
using MediatR;
using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.Handler.BpmnBaseline.Querry.GetAll
{
    public class GetAllBpmnProcessesQueryHandler : IRequestHandler<GetAllBpmnProcessesQuery, IEnumerable<BpmnProcessBaseline>>
    {
        private readonly IBpmnProcessRepository _repository;

        public GetAllBpmnProcessesQueryHandler(IBpmnProcessRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BpmnProcessBaseline>> Handle(GetAllBpmnProcessesQuery request, CancellationToken cancellationToken)
        {
            var processes = await _repository.GetAllAsync(cancellationToken);
            var latestBaselines = processes
                .GroupBy(p => p.Name)
                .Select(g => g.OrderByDescending(x => x.Version).First())
                .ToList();
            return latestBaselines;
        }
    }
}
