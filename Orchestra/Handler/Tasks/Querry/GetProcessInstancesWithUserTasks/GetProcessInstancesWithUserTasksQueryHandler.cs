using MediatR;
using Orchestra.Data.Context;
using Orchestra.Dtos;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Handler.Tasks.Querry.GetProcessInstancesWithUserTasks
{
    public class GetProcessInstancesWithUserTasksQueryHandler : IRequestHandler<GetProcessInstancesWithUserTasksQuery, List<ProcessInstanceWithTasksDto>>
    {
        private readonly ITaskService _taskService;

        public GetProcessInstancesWithUserTasksQueryHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<List<ProcessInstanceWithTasksDto>> Handle(GetProcessInstancesWithUserTasksQuery request, CancellationToken cancellationToken)
        {
            var userTasks = await _taskService.GetUserTasksAsync(request.UserId, cancellationToken);

            var processInstanceIds = userTasks
                .Select(t => t.BpmnProcessId)
                .Distinct()
                .ToList();

            var processInstances = await _taskService.GetProcessInstancesByIdsAsync(processInstanceIds, cancellationToken);

            var result = _taskService.MapProcessInstancesWithTasks(processInstances, userTasks);

            return result;
        }
    }
}
