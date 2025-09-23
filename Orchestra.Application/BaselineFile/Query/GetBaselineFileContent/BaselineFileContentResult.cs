namespace Orchestra.Application.BaselineFile.Query.GetBaselineFileContent
{
    public class BaselineFileContentResult
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
