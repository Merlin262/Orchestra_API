using MediatR;
using Orchestra.Models;
using Orchestra.Repoitories;

namespace Orchestra.Handler.Querry
{
    public class GetAllBpmnProcessesQueryHandler : IRequestHandler<GetAllBpmnProcessesQuery, IEnumerable<BpmnProcess>>
    {
        private readonly IBpmnProcessRepository _repository;

        public GetAllBpmnProcessesQueryHandler(IBpmnProcessRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BpmnProcess>> Handle(GetAllBpmnProcessesQuery request, CancellationToken cancellationToken)
        {
            // Busque os processos do repositório
            var processes = await _repository.GetAllAsync(cancellationToken);
            return processes;
        }
    }
}
