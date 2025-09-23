using MediatR;
using Orchestra.Models;

namespace Orchestra.Handler.BpmnBaseline.Querry.GetAll
{
    public class GetAllBpmnProcessesQuery : IRequest<IEnumerable<BpmnProcessBaseline>>
    {
    }
}
