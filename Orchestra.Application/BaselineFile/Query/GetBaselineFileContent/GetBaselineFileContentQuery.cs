using System;
using MediatR;

namespace Orchestra.Application.BaselineFile.Query.GetBaselineFileContent
{
    public class GetBaselineFileContentQuery : IRequest<BaselineFileContentResult>
    {
        public Guid Id { get; set; }
        public GetBaselineFileContentQuery(Guid id)
        {
            Id = id;
        }
    }
}
