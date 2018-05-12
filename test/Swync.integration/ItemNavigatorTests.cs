using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Swync.core.Onedrive.Authentication;
using Swync.core.Onedrive.Http;
using Swync.core.Onedrive.Items;
using Swync.test.common.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Swync.integration
{
    public class ItemNavigatorTests : IntegrationTestBase
    {
        private readonly ITestOutputHelper _output;
        private readonly ItemNavigator _sut;

        public ItemNavigatorTests(ITestOutputHelper output)
        {
            IAuthenticator authenticator = new Authenticator();
            IHttpClientFactory httpClientFactory = new HttpClientFactory();
            IOnedriveAuthenticatedAccess access = new OnedriveAuthenticatedAccess(authenticator, httpClientFactory);
            _sut = new ItemNavigator(access);
            _output = output;
        }
        
        [Fact]
        public async Task CanGetListOfDrives()
        {
            var drives = await _sut.GetDrivesAsync(CancellationToken.None);

            _output.WriteJson(drives);
            drives.value.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CanGetListOfChildItemsInRootOfDrive()
        {
            var drives = await _sut.GetDrivesAsync(CancellationToken.None);
            var drive = drives.value.TakeRandom();
            
            var directories = await _sut.GetItems(drive, CancellationToken.None);

            _output.WriteJson(directories);
            directories.value.Should().NotBeEmpty();
        }
    }
}
