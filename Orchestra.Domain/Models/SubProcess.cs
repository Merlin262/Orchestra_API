using Orchestra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Orchestra.Domain.Models
{
    public class SubProcess
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? XmlContent { get; set; }
        public int ProcessBaselineId { get; set; }
        [JsonIgnore]
        public BpmnProcessBaseline ProcessBaseline { get; set; }
        public List<Tasks> Tasks { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public double BaselineVersion { get; set; }
        public int BaselineHistoryId { get; set; }
        public BaselineHistory? BaselineHistory { get; set; }
    }
}
