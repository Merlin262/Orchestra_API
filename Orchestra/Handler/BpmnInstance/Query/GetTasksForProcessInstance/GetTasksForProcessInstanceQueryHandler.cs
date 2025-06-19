using MediatR;
using Orchestra.Data.Context;
using Orchestra.Dtos;
using Orchestra.Services;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.BpmnInstance.Query.GetTasksForProcessInstance
{
    public class GetTasksForProcessInstanceQueryHandler : IRequestHandler<GetTasksForProcessInstanceQuery, List<TaskWithUserDto>>
    {
        private readonly ITaskService _taskService;

        public GetTasksForProcessInstanceQueryHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<List<TaskWithUserDto>> Handle(GetTasksForProcessInstanceQuery request, CancellationToken cancellationToken)
        {
            return await _taskService.GetTasksForProcessInstanceAsync(request.ProcessInstanceId, cancellationToken);
        }
    }
}