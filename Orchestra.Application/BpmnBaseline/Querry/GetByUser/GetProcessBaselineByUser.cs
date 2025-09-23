using MediatR;
using Orchestra.Dtos;

namespace Orchestra.Handler.BpmnBaseline.Querry.GetByUser
{
    public class GetProcessBaselineByUser : IRequest<List<BpmnProcessBaselineWithUserDto>>
    {
        public string UserId { get; }

        public GetProcessBaselineByUser(string userId)
        {
            UserId = userId;
        }
    }
}
