using System.Threading;
using System.Threading.Tasks;
using Swync.Core.Functional;
using Swync.Core.Onedrive;
using Swync.Core.Onedrive.Authentication;
using Swync.Core.Onedrive.Http;
using Swync.Core.Onedrive.Items;
using Swync.Test.Common.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Swync.Integration
{
    public class ManualTests
    {
        private readonly OnedriveClient _onedriveClient;
        private readonly ITestOutputHelper _output;

        public ManualTests(ITestOutputHelper output)
        {
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
            var result = await _onedriveClient.GetDrivesAsync(CancellationToken.None);
            foreach (var drive in result)
            {
                _output.WriteLine(drive.Id);
            }
        }

        [Fact]
        [igno]
        public async Task GetAllDirectories()
        {
            var result = await _onedriveClient.GetDirectories(CancellationToken.None);
            foreach (var directory in result)
            {
                _output.WriteLine(directory.Path.Join("/"));
            }
        }
    }
}