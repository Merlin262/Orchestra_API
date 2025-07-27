using System.Linq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Dtos;

namespace Orchestra.Handler.BpmnBaseline.Querry.GetByUser
{
    public class GetBpmnProcessByUserIdQueryHandler : IRequestHandler<GetProcessBaselineByUser, List<BpmnProcessBaselineWithUserDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetBpmnProcessByUserIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BpmnProcessBaselineWithUserDto>> Handle(GetProcessBaselineByUser request, CancellationToken cancellationToken)
        {
            var baselines = await _context.BpmnProcess
                .Where(b => b.CreatedBy == request.UserId)
                .ToListAsync(cancellationToken);

            var latestBaselines = baselines
                .GroupBy(b => b.Name)
                .Select(g => g.OrderByDescending(x => x.Version).First())
                .ToList();

            var userFullName = await GetUserFullNameByIdAsync(request.UserId, cancellationToken);

            var result = latestBaselines.Select(b => new BpmnProcessBaselineWithUserDto
            {
                Id = b.Id,
                Name = b.Name,
                XmlContent = b.XmlContent,
                CreatedAt = b.CreatedAt,
                PoolNames = b.PoolNames,
                CreatedBy = b.CreatedBy,
                Version = b.Version,
                CreatedByUserName = userFullName,
                Description = b.Description
            }).ToList();

            return result;
        }

        public async Task<string?> GetUserFullNameByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
