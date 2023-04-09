using BatchTest.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BatchTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly string _waitTaskDirectory;
        private readonly string _runningTaskDirectory;

        public BatchController(IOptions<BatchTaskSettings> batchTaskSettings)
        {
            _waitTaskDirectory = batchTaskSettings.Value.WaitTaskDirectory;
            _runningTaskDirectory = batchTaskSettings.Value.RunningTaskDirectory;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterBatchTask([FromBody] BatchTaskInfo taskInfo)
        {
            // タスクファイル名を作成
            string fileName = $"{taskInfo.ScheduledStartTime:yyyyMMddHHmmss}_{taskInfo.GUID}_{taskInfo.Email}.json";

            // appsettings.jsonで指定したディレクトリにタスクファイルを保存
            var taskFilePath = Path.Combine(_waitTaskDirectory, fileName);
            System.IO.File.WriteAllText(taskFilePath, JsonConvert.SerializeObject(taskInfo));


            return Ok();
        }
        [HttpPost("get-tasks")]
        public IActionResult GetTasks([FromBody] EmailRequest emailRequest)
        {
            var pendingTasks = GetTasksByEmail(emailRequest.Email, _waitTaskDirectory);
            var runningTasks = GetTasksByEmail(emailRequest.Email, _runningTaskDirectory);

            return Ok(new { PendingTasks = pendingTasks, RunningTasks = runningTasks });
        }

        private List<BatchTaskInfo> GetTasksByEmail(string email, string directory)
        {
            var taskFiles = Directory.GetFiles(directory, $"*_{email}.*");
            var tasks = new List<BatchTaskInfo>();

            foreach (var taskFile in taskFiles)
            {
                var taskInfo = JsonConvert.DeserializeObject<BatchTaskInfo>(System.IO.File.ReadAllText(taskFile));
                tasks.Add(taskInfo);
            }

            return tasks;
        }
    }
}
