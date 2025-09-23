using MediatR;
using Orchestra.Dtos;

namespace Orchestra.Handler.BpmnInstance.Query.GetTasksForProcessInstance
{
    public class GetTasksForProcessInstanceQuery : IRequest<List<TaskWithUserDto>>
    {
        public int ProcessInstanceId { get; }
        public GetTasksForProcessInstanceQuery(int processInstanceId)
        {
            ProcessInstanceId = processInstanceId;
        }
    }
}
