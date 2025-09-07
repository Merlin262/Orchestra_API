namespace Orchestra.Dtos
{
    public class UploadTaskFileDto
    {
        public IFormFile File { get; set; }
        public string UploadedBy { get; set; } // Id do usuário que está fazendo upload
    }
}
