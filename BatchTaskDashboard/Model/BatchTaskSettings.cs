namespace BatchTest.Model
{
    public class BatchTaskSettings
    {
        public string TaskDirectory { get; set; }
        public string InProgressDirectory { get; set; }
        public string CompletedDirectory { get; set; }
        public string FailedDirectory { get; set; }
    }
}
