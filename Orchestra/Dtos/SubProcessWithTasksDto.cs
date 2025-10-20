using System;
using System.Collections.Generic;

namespace Orchestra.DTOs
{
    public class SubProcessWithTasksDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? XmlContent { get; set; }
        public int ProcessBaselineId { get; set; }
        public string? UserId { get; set; }
        public List<TaskDto> Tasks { get; set; } = new();
        public double? Version { get; set; }
        public int BaselineHistoryId { get; set; }
    }

    public class TaskDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string XmlTaskId { get; set; }
        public bool Completed { get; set; }
        public string? Comments { get; set; }
        public string? ResponsibleUserId { get; set; }
        public string? Pool { get; set; }
        public int StatusId { get; set; }
    }
}
