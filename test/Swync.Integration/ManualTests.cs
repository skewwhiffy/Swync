using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Swync.Core.Functional;
using Swync.Core.Onedrive;
using Swync.Core.Onedrive.Authentication;
using Swync.Core.Onedrive.Http;
using Swync.Core.Onedrive.Items;
using Xunit;
using Xunit.Abstractions;

namespace Swync.Integration
{
    public class ManualTests
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly OnedriveClient _onedriveClient;
        private readonly ITestOutputHelper _output;

        public ManualTests(ITestOutputHelper output)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _output = output;
            var authenticator = new Authenticator();
            var httpClientFactory = new HttpClientFactory();
            var onedriveAuthenticatedAccess = new OnedriveAuthenticatedAccess(authenticator, httpClientFactory);
            var itemNavigator = new ItemNavigator(onedriveAuthenticatedAccess);
            _onedriveClient = new OnedriveClient(itemNavigator);
        }
        
        [Fact(Skip = "Run me manually")]
        public async Task GetAllDrives()
        {
            var result = await _onedriveClient.GetDrivesAsync(_cancellationTokenSource.Token);
            foreach (var drive in result)
            {
                _output.WriteLine(drive.Id);
            }
        }

        [Fact(Skip = "Run me manually")]
        public async Task CycleThroughDirectories()
        {
            const int directoriesToFetch = 200;
            bool IsRunning(Task t) =>
                new[] {TaskStatus.Running, TaskStatus.WaitingForActivation}.Contains(t.Status);
            var directories = new ConcurrentBag<string>();
            var done = 0;
            var task = _onedriveClient.CycleThroughDirectoriesAsync(
                d =>
                {
                    directories.Add(d.Path.Join("/"));
                    return Task.FromResult(0);
                }, () =>
                {
                    Interlocked.Increment(ref done);
                    return Task.FromResult(0);
                },
                _cancellationTokenSource.Token);

            var lastCount = 0;
            while (!IsRunning(task) && !task.IsCompleted)
            {
                await Task.Delay(100);
            }
            while (IsRunning(task) && directories.Count < directoriesToFetch)
            {
                var currentCount = directories.Count;
                _output.WriteLine(currentCount == lastCount
                    ? "No new directories found"
                    : $"{directories.Count} directories have been discovered");

                lastCount = currentCount;
                await Task.Delay(500);
            }

            _output.WriteLine("");
            directories.ToList().ForEach(_output.WriteLine);

            new[] {TaskStatus.Running, TaskStatus.RanToCompletion, TaskStatus.WaitingForActivation}.Should().Contain(task.Status);
            _cancellationTokenSource.Cancel();
            while (task.Status == TaskStatus.Running || task.Status == TaskStatus.WaitingForActivation)
            {
                await Task.Delay(100);
            }
        }
    }
}