using BatchTest.Model;

namespace BatchTaskDashboard.Model
{
    public class BatchTaskSummary
    {
        public string Status { get; set; }
        public BatchTaskInfo BatchTaskInfo { get; set; }
    }
}
