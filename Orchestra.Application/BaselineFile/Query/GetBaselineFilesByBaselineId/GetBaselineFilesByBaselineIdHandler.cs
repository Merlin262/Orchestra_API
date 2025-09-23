using MediatR;
using Orchestra.Services.Serviecs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestra.Application.BaselineFile.Query.GetBaselineFilesByBaselineId
{
    public class GetBaselineFilesByBaselineIdHandler : IRequestHandler<GetBaselineFilesByBaselineIdQuery, List<GetBaselineFilesByBaselineIdHandlerResult>>
    {
        private readonly IBaselineFileService _service;
        public GetBaselineFilesByBaselineIdHandler(IBaselineFileService service)
        {
            _service = service;
        }

        public async Task<List<GetBaselineFilesByBaselineIdHandlerResult>> Handle(GetBaselineFilesByBaselineIdQuery request, CancellationToken cancellationToken)
        {
            var files = await _service.GetFilesByBaselineIdAsync(request.BaselineId);
            var result = files.Select(f => new GetBaselineFilesByBaselineIdHandlerResult
            {
                Id = f.Id,
                FileName = f.FileName,
                ContentType = f.ContentType,
                UploadedAt = f.UploadedAt,
                UploadedByName = f.UploadedBy != null ? f.UploadedBy.FullName : null,
                XmlTaskId = f.XmlTaskId
            }).ToList();
            return result;
        }
    }
}
