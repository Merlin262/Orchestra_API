using System;
using System.Collections.Generic;

namespace Orchestra.Dtos
{
    public class DisableTasksDto
    {
        public List<Guid> DisableTaskIds { get; set; }
        public List<Guid>? AbleTaskIds { get; set; }
    }
}
