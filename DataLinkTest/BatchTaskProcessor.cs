using BatchTest.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLinkTest
{
    public class BatchTaskProcessor
    {
        private readonly string _taskDirectory;
        private readonly string _inProgressDirectory;
        private readonly string _completedDirectory;
        private readonly string _failedDirectory;
        private readonly SemaphoreSlim _semaphore;

        public BatchTaskProcessor(string taskDirectory, string inProgressDirectory, string completedDirectory, string failedDirectory, int maxConcurrentTasks)
        {
            _taskDirectory = taskDirectory;
            _inProgressDirectory = inProgressDirectory;
            _completedDirectory = completedDirectory;
            _failedDirectory = failedDirectory;
            _semaphore = new SemaphoreSlim(maxConcurrentTasks, maxConcurrentTasks);
        }

        public async Task MonitorTasksAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var taskFiles = Directory.GetFiles(_taskDirectory, "*.json");

                // 実行対象のタスクファイルをフィルタリング
                var now = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(9));
                taskFiles = taskFiles.Where(taskFile =>
                {
                    var fileName = Path.GetFileNameWithoutExtension(taskFile);
                    var parts = fileName.Split('_');
                    DateTimeOffset scheduledTime;
                    return DateTimeOffset.TryParseExact(parts[0], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out scheduledTime) && scheduledTime <= now;
                }).ToArray();

                // 実行対象のタスクファイルをすべて実行中ディレクトリに移動
                var inProgressFiles = taskFiles.Select(taskFile => MoveTaskFile(taskFile, _inProgressDirectory)).ToList();

                // 実行対象のタスクをすべて並列実行
                var tasks = inProgressFiles.Select(async inProgressFile =>
                {
                    try
                    {
                        await _semaphore.WaitAsync();
                        var taskResult = await ExecuteTaskAsync(inProgressFile);

                        if (taskResult)
                        {
                            MoveTaskFile(inProgressFile, _completedDirectory);
                        }
                        else
                        {
                            MoveTaskFile(inProgressFile, _failedDirectory);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error executing task {Path.GetFileName(inProgressFile)}: {ex.Message}");
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                });

                await Task.WhenAll(tasks);

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        private async Task<bool> ExecuteTaskAsync(string taskFile)
        {
            for(int i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                await Task.Delay(1000);
            }
            var taskInfo = JsonConvert.DeserializeObject<BatchTaskInfo>(File.ReadAllText(taskFile));

            var results = new List<HttpResponseMessage>();

            using (var httpClient = new HttpClient())
            {
                foreach (var postInfo in taskInfo.PostInfos)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(postInfo.param), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(postInfo.Url, content);
                    results.Add(response);
                }
            }

            var success = results.All(response => response.IsSuccessStatusCode);
            return success;



            // ③: タスクファイルを読み込み、各POSTリクエストに対してJSONペイロードを送信する実装
            // ④: リストの内容をCSVファイルに出力し、そのリンクをメールで送信する実装

            // タスクの実行結果を返します。成功の場合はtrue、失敗の場合はfalse。
            return true;
        }

        private string MoveTaskFile(string taskFile, string targetDirectory)
        {
            var fileName = Path.GetFileName(taskFile);
            var targetPath = Path.Combine(targetDirectory, fileName);

            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            File.Move(taskFile, targetPath);
            return targetPath;
        }
    }

}
