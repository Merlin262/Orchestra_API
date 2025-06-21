using MediatR;
using Orchestra.Models;

namespace Orchestra.Handler.BpmnBaseline.Querry.GetById
{
    public class GetBpmnProcessByIdQuery : IRequest<BpmnProcessBaseline?>
    {
        public int Id { get; }

        public GetBpmnProcessByIdQuery(int id)
        {
            Id = id;
        }
    }
}
