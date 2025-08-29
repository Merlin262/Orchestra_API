using System.ComponentModel.DataAnnotations;

namespace Orchestra.Models
{
    public class TaskFile
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public Tasks Task { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
