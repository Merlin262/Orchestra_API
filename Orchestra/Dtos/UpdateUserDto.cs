using System.Text.Json.Serialization;
using System.Collections.Generic;
using Orchestra.Enums;

namespace Orchestra.Dtos
{
    public class UpdateUserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public ProfileTypeEnum ProfileType { get; set; } 
        public List<string> Roles { get; set; }
        public bool IsActive { get; set; }
    }
}
