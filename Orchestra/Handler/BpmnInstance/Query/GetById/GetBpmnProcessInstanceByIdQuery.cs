using MediatR;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Handler.BpmnInstance.Query.GetById
{
    public class GetBpmnProcessInstanceByIdQuery : IRequest<BpmnProcessInstance?>
    {
        public int Id { get; }

        public GetBpmnProcessInstanceByIdQuery(int id)
        {
            Id = id;
        }
    }
}
