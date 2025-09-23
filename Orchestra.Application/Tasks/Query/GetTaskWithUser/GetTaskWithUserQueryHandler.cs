using MediatR;
using Orchestra.Dtos;
using Orchestra.Handler.Tasks.Querry.GetTaskWithUser;
using Orchestra.Serviecs.Intefaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestra.Handler.Tasks.Query.GetTaskWithUser
{
    public class GetTaskWithUserQueryHandler : IRequestHandler<GetTaskWithUserQuery, List<TaskWithUserDto>>
    {
        private readonly ITaskService _taskService;

        public GetTaskWithUserQueryHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<List<TaskWithUserDto>> Handle(GetTaskWithUserQuery request, CancellationToken cancellationToken)
        {
            return await _taskService.GetTasksForProcessInstanceAsync(request.ProcessInstanceId, cancellationToken);
        }
    }
}