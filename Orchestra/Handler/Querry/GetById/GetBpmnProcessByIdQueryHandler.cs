using MediatR;
using Orchestra.Models;
using Orchestra.Repoitories;

namespace Orchestra.Handler.Querry.GetById
{
    public class GetBpmnProcessByIdQueryHandler : IRequestHandler<GetBpmnProcessByIdQuery, BpmnProcess?>
    {
        private readonly IBpmnProcessRepository _repository;

        public GetBpmnProcessByIdQueryHandler(IBpmnProcessRepository repository)
        {
            _repository = repository;
        }

        public async Task<BpmnProcess?> Handle(GetBpmnProcessByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
