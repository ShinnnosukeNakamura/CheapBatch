using System;
using System.Collections.Generic;
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

        public BatchTaskProcessor(string taskDirectory, string inProgressDirectory, string completedDirectory, string failedDirectory)
        {
            _taskDirectory = taskDirectory;
            _inProgressDirectory = inProgressDirectory;
            _completedDirectory = completedDirectory;
            _failedDirectory = failedDirectory;
            _semaphore = new SemaphoreSlim(50);
        }

        public async Task MonitorTasksAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var taskFiles = Directory.GetFiles(_taskDirectory, "*.json");

                foreach (var taskFile in taskFiles)
                {
                    await _semaphore.WaitAsync();

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            var inProgressFile = MoveTaskFile(taskFile, _inProgressDirectory);
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
                            Console.WriteLine($"Error executing task {Path.GetFileName(taskFile)}: {ex.Message}");
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    });
                }

                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
        }

        private async Task<bool> ExecuteTaskAsync(string taskFile)
        {
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
