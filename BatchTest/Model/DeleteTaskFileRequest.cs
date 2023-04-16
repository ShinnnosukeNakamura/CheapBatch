namespace BatchManagementAPI.Model
{
    public class DeleteTaskFileRequest
    {
        public DateTimeOffset ScheduledStartTime { get; set; }
        public string GUID { get; set; }
        public string Email { get; set; }
    }
}
