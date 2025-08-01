using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Dtos;
using Orchestra.Handler.Tasks.Querry.GetTaskWithUser;
using Orchestra.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestra.Handler.Tasks.Query.GetTaskWithUser
{
    public class GetTaskWithUserQueryHandler : IRequestHandler<GetTaskWithUserQuery, List<TaskWithUserDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetTaskWithUserQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskWithUserDto>> Handle(GetTaskWithUserQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _context.Tasks
                .Include(t => t.ResponsibleUser)
                .Where(t => t.BpmnProcessId == request.ProcessInstanceId && t.ResponsibleUser != null)
                .Select(t => new TaskWithUserDto
                {
                    TaskId = t.Id,
                    Name = t.Name,
                    XmlTaskId = t.XmlTaskId,
                    Completed = t.Completed,
                    StatusId = (int)t.Status,
                    CreatedAt = t.CreatedAt,
                    CompletedAt = t.CompletedAt,
                    Comments = t.Comments,
                    ResponsibleUser = t.ResponsibleUser == null ? null : new UserDto
                    {
                        Id = t.ResponsibleUser.Id,
                        UserName = t.ResponsibleUser.UserName,
                        Email = t.ResponsibleUser.Email,
                        FullName = t.ResponsibleUser.FullName,
                        //Roles = t.ResponsibleUser.Roles
                    }
                })
                .ToListAsync(cancellationToken);

            return tasks;
        }
    }
}