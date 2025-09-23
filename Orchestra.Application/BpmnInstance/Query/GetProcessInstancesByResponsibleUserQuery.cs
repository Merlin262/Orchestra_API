using MediatR;
using Orchestra.Dtos;
using System.Collections.Generic;

namespace Orchestra.Handler.BpmnInstance.Query
{
    public class GetProcessInstancesByResponsibleUserQuery : IRequest<List<ProcessInstanceWithTasksDto>>
    {
        public string UserId { get; }
        public GetProcessInstancesByResponsibleUserQuery(string userId)
        {
            UserId = userId;
        }
    }
}
