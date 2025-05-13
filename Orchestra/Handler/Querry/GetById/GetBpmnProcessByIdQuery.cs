using MediatR;
using Orchestra.Models;

namespace Orchestra.Handler.Querry.GetById
{
    public class GetBpmnProcessByIdQuery : IRequest<BpmnProcess?>
    {
        public int Id { get; }

        public GetBpmnProcessByIdQuery(int id)
        {
            Id = id;
        }
    }
}
