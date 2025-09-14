using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestra.Models
{
    public class BaselineFile
    {
        [Key]
        public Guid Id { get; set; }
        public int BaselineId { get; set; }
        public BpmnProcessBaseline Baseline { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public string UploadedByUserId { get; set; }
        public User UploadedBy { get; set; }
        public string XmlTaskId { get; set; }
    }
}
