using MediatR;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Handler.BpmnInstance.Query.GetProcessInstance
{
    public class GetBpmnProcessInstancesQuery : IRequest<IEnumerable<BpmnProcessInstance>>
    {
    }
}
