using MediatR;
using Orchestra.Dtos;

namespace Orchestra.Handler.Tasks.Querry.GetProcessInstancesWithUserTasks
{
    public class GetProcessInstancesWithUserTasksQuery : IRequest<List<ProcessInstanceWithTasksDto>>
    {
        public string UserId { get; }
        public CancellationToken CancellationToken { get; }

        public GetProcessInstancesWithUserTasksQuery(string userId, CancellationToken cancellationToken)
        {
            UserId = userId;
            CancellationToken = cancellationToken;
        }
    }
}
