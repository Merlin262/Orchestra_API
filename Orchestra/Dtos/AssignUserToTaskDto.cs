namespace Orchestra.Dtos
{
    public class AssignUserToTaskDto
    {
        public string TaskId { get; set; }
        public string UserId { get; set; }
        public int ProcessInstanceId { get; set; }
    }
}
