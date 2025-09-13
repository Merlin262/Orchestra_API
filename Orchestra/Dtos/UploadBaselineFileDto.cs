namespace Orchestra.Dtos
{
    public class UploadBaselineFileDto
    {
        public IFormFile File { get; set; }
        public string UploadedByUserId { get; set; }
        public string XmlDataObjectId { get; set; }
    }
}
