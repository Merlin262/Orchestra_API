using MediatR;
using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.Handler.BpmnBaseline.Querry.GetById
{
    public class GetBpmnProcessByIdQueryHandler : IRequestHandler<GetBpmnProcessByIdQuery, BpmnProcessBaseline?>
    {
        private readonly IBpmnProcessRepository _repository;

        public GetBpmnProcessByIdQueryHandler(IBpmnProcessRepository repository)
        {
            _repository = repository;
        }

        public async Task<BpmnProcessBaseline?> Handle(GetBpmnProcessByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
