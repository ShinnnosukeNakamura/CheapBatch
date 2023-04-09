using BatchTest.Model;
using DataLinkTest.Model;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using DataLinkTest;

class Program
{
    private string _waitTaskDirectory = "";
    private string _runningTaskDirectory = "";
    static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

        var taskSettings = new TaskSettings();
        configuration.GetSection("TaskSettings").Bind(taskSettings);
        // ディレクトリのパスを設定します
        string taskDirectory = Path.Combine(Directory.GetCurrentDirectory(), taskSettings.TaskDirectory);
        string inProgressDirectory = Path.Combine(Directory.GetCurrentDirectory(), taskSettings.InProgressDirectory);
        string completedDirectory = Path.Combine(Directory.GetCurrentDirectory(), taskSettings.CompletedDirectory);
        string failedDirectory = Path.Combine(Directory.GetCurrentDirectory(), taskSettings.FailedDirectory);

        // ディレクトリが存在しない場合、作成
        Directory.CreateDirectory(taskDirectory);
        Directory.CreateDirectory(inProgressDirectory);
        Directory.CreateDirectory(completedDirectory);
        Directory.CreateDirectory(failedDirectory);

        // タスクプロセッサーのインスタンスを作成
        var batchTaskProcessor = new BatchTaskProcessor(taskDirectory, inProgressDirectory, completedDirectory, failedDirectory, maxConcurrentTasks: 50);

        // タスク監視を開始
        var cancellationTokenSource = new CancellationTokenSource();
        var monitorTask = batchTaskProcessor.MonitorTasksAsync(cancellationTokenSource.Token);

        Console.WriteLine("Press any key to stop the task processor...");
        Console.ReadKey();

        // タスク監視を終了
        cancellationTokenSource.Cancel();
        await monitorTask;
    }

}