using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly CancellationTokenSource _ctSource;
        private readonly OnedriveClient _onedriveClient;
        private readonly ITestOutputHelper _output;

        public ManualTests(ITestOutputHelper output)
        {
            _ctSource = new CancellationTokenSource();
            _output = output;
            var authenticator = new Authenticator();
            var httpClientFactory = new HttpClientFactory();
            var onedriveAuthenticatedAccess = new OnedriveAuthenticatedAccess(authenticator, httpClientFactory);
            var itemNavigator = new ItemNavigator(onedriveAuthenticatedAccess);
            _onedriveClient = new OnedriveClient(itemNavigator);
        }
        
        [Fact]
        public async Task GetAllDrives()
        {
            var result = await _onedriveClient.GetDrivesAsync(_ctSource.Token);
            foreach (var drive in result)
            {
                _output.WriteLine(drive.Id);
            }
        }

        [Fact]
        public async Task GetAllDirectories()
        {
            var directories = new ConcurrentBag<string>();
            var cycleTask = _onedriveClient.CycleThroughDirectoriesAsync(d =>
                {
                    var path = d.Path.Join("/");
                    _output.WriteLine(path);
                    directories.Add(path);
                    return 0.Pipe(Task.FromResult);
                }, () => 0.Pipe(Task.FromResult),
                _ctSource.Token);

            var lastDirectoryCount = 0;
            while (directories.Count < 100 && cycleTask.Status == TaskStatus.Running)
            {
                var currentDirectoryCount = directories.Count;
                if (currentDirectoryCount == lastDirectoryCount)
                {
                    _output.WriteLine("Count has not increased");
                }
                await Task.Delay(1000);
            }

            _ctSource.Cancel();
        }
    }
}