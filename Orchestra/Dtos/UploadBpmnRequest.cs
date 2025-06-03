namespace Orchestra.Dtos
{
    public class UploadBpmnRequest
    {
        public IFormFile File { get; set; }
        public string? UserId { get; set; }
    }
}
