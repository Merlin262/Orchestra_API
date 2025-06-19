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
            // Busque os processos do repositório
            var processes = await _repository.GetAllAsync(cancellationToken);
            return processes;
        }
    }
}
