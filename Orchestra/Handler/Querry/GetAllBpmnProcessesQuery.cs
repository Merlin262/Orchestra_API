using MediatR;
using Orchestra.Models;

namespace Orchestra.Handler.Querry
{
    public class GetAllBpmnProcessesQuery : IRequest<IEnumerable<BpmnProcessBaseline>>
    {
    }
}
