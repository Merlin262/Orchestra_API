using Microsoft.AspNetCore.Http;

namespace Orchestra.Dtos
{
    public class UpdateBaselineWithNewBpmnRequest
    {
        public IFormFile File { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        //public double Version { get; set; }
    }
}
