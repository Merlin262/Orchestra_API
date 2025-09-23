using MediatR;
using Orchestra.Dtos;

namespace Orchestra.Handler.Tasks.Querry.GetTaskWithUser
{
    public class GetTaskWithUserQuery : IRequest<List<TaskWithUserDto>>
    {
        public int ProcessInstanceId { get; }

        public GetTaskWithUserQuery(int processInstanceId)
        {
            ProcessInstanceId = processInstanceId;
        }
    }
}
