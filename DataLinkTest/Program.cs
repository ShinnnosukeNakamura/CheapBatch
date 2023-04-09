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

        
        var taskDirectory = taskSettings.TaskDirectory;
        var inProgressDirectory = taskSettings.InProgressDirectory;
        var completedDirectory = taskSettings.CompletedDirectory; ;
        var failedDirectory = taskSettings.FailedDirectory;

        var batchTaskProcessor = new BatchTaskProcessor(taskDirectory, inProgressDirectory, completedDirectory, failedDirectory);
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        await batchTaskProcessor.MonitorTasksAsync(cts.Token);
    
    }

}