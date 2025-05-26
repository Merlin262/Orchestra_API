namespace Orchestra.Dtos
{
    public class AssignUserToTaskDto
    {
        public Guid TaskId { get; set; }
        public string UserId { get; set; }
    }
}
