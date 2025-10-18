using Microsoft.AspNetCore.Http;

namespace Orchestra.DTOs
{
    public class CreateSubProcessWithFileDto
    {
        public string Name { get; set; }
        public int ProcessBaselineId { get; set; }
        public string? UserId { get; set; }
        public IFormFile? File { get; set; }
    }
}