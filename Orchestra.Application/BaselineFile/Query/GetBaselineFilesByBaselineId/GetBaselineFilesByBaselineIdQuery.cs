using MediatR;
using System.Collections.Generic;
using Orchestra.Dtos;

namespace Orchestra.Application.BaselineFile.Query.GetBaselineFilesByBaselineId
{
    public class GetBaselineFilesByBaselineIdQuery : IRequest<List<GetBaselineFilesByBaselineIdHandlerResult>>
    {
        public int BaselineId { get; }
        public GetBaselineFilesByBaselineIdQuery(int baselineId)
        {
            BaselineId = baselineId;
        }
    }
}
