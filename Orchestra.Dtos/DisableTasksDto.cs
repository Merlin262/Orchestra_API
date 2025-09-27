using System;
using System.Collections.Generic;

namespace Orchestra.Dtos
{
    public class DisableTasksDto
    {
        public List<Guid> TaskIds { get; set; }
    }
}
