using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestra.Application.BaselineFile.Query.GetBaselineFilesByBaselineId
{
    public class GetBaselineFilesByBaselineIdHandlerResult
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedByName { get; set; }
        public string XmlTaskId { get; set; }
    }
}
