using BatchManagementAPI.Model;
using BatchTest.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IO;
using IOFile = System.IO.File; // エイリアスを追加

namespace BatchTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly string _taskDirectory;
        private readonly string _inprogressTaskDirectory;

        public BatchController(IOptions<BatchTaskSettings> batchTaskSettings)
        {
            _taskDirectory = batchTaskSettings.Value.TaskDirectory;
            _inprogressTaskDirectory = batchTaskSettings.Value.InProgressDirectory;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterBatchTask([FromBody] BatchTaskInfo taskInfo)
        {
            // タスクファイル名を作成
            string fileName = $"{taskInfo.ScheduledStartTime:yyyyMMddHHmmss}_{taskInfo.GUID}_{taskInfo.Email}.json";

            // appsettings.jsonで指定したディレクトリにタスクファイルを保存
            var taskFilePath = Path.Combine(_taskDirectory, fileName);
            System.IO.File.WriteAllText(taskFilePath, JsonConvert.SerializeObject(taskInfo));


            return Ok();
        }
        [HttpPost("get-tasks")]
        public IActionResult GetTasks([FromBody] EmailRequest emailRequest)
        {
            var pendingTasks = GetTasksByEmail(emailRequest.Email, _taskDirectory, "pending");
            var runningTasks = GetTasksByEmail(emailRequest.Email, _inprogressTaskDirectory, "running");

            var allTasks = new List<BatchTaskSummary>();
            allTasks.AddRange(pendingTasks);
            allTasks.AddRange(runningTasks);

            return Ok(allTasks);
        }

        private List<BatchTaskSummary> GetTasksByEmail(string email, string directory, string status)
        {
            var taskFiles = Directory.GetFiles(directory, $"*_{email}.*");
            var tasks = new List<BatchTaskSummary>();

            foreach (var taskFile in taskFiles)
            {
                var taskInfo = JsonConvert.DeserializeObject<BatchTaskInfo>(System.IO.File.ReadAllText(taskFile));
                tasks.Add(new BatchTaskSummary
                {
                    Status = status,
                    BatchTaskInfo = taskInfo
                });
            }

            return tasks;
        }

        [HttpPost("DeleteTaskFile")]
        public async Task<ActionResult<DeleteTaskFileResult>> DeleteTaskFileAsync([FromBody] DeleteTaskFileRequest request)
        {
            var result = new DeleteTaskFileResult();

            try
            {
                string fileName = $"{request.ScheduledStartTime:yyyyMMddHHmmss}_{request.GUID}_{request.Email}.json";

                string filePath = Path.Combine(_taskDirectory, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    result.Result = true;
                    result.Message = "タスクファイルが正常に削除されました。";
                }
                else
                {
                    result.Result = false;
                    result.Message = "タスクファイルが見つかりませんでした。";
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = $"タスクファイルの削除中にエラーが発生しました: {ex.Message}";
            }

            return Ok(result);
        }



    }
}
