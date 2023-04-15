using BatchTest.Model;

namespace BatchManagementAPI.Model
{
    public class BatchTaskSummary
    {
        public string Status { get; set; }
        public BatchTaskInfo BatchTaskInfo { get; set; }
    }
}
