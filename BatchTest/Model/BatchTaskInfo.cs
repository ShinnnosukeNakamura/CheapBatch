namespace BatchTest.Model
{
    public enum BatchType
    {
        Immediate,
        Daily,
        Monthly
    }
    public class BatchTaskInfo
    {
        public string BatchName { get; set; }
        public string GUID { get; set; }
        public DateTimeOffset ScheduledStartTime { get; set; }
        public string Email { get; set; }
        public List<PostInfo> PostInfos { get; set; }
        public BatchType BatchType { get; set; } // バッチの種類
        public int LoopCount { get; set; } // ループの回数
    }
}
