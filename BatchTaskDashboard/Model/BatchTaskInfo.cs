namespace BatchTest.Model
{
    public class BatchTaskInfo
    {
        public string BatchName { get; set; }
        public string GUID { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public string Email { get; set; }
        public List<PostInfo> PostInfos { get; set; }
    }
}
