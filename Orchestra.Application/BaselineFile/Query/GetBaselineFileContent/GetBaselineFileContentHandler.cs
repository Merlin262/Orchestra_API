using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Orchestra.Services.Serviecs;

namespace Orchestra.Application.BaselineFile.Query.GetBaselineFileContent
{
    public class GetBaselineFileContentHandler : IRequestHandler<GetBaselineFileContentQuery, BaselineFileContentResult>
    {
        private readonly IBaselineFileService _service;
        public GetBaselineFileContentHandler(IBaselineFileService service)
        {
            _service = service;
        }

        public async Task<BaselineFileContentResult> Handle(GetBaselineFileContentQuery request, CancellationToken cancellationToken)
        {
            var file = await _service.GetFileContentAsync(request.Id);
            if (file == null)
                return null;
            return new BaselineFileContentResult
            {
                Content = file.Content,
                ContentType = file.ContentType,
                FileName = file.FileName
            };
        }
    }
}
