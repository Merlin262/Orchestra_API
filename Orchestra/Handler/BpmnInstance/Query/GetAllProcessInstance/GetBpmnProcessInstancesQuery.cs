using MediatR;
using Orchestra.Dtos;
using Orchestra.Handler.BpmnInstance.Query.GetAllProcessInstance;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Handler.BpmnInstance.Query.GetProcessInstance
{
    public class GetBpmnProcessInstancesQuery : IRequest<IEnumerable<GetBpmnProcessInstancesQueryResult>>
    {
    }
}
