using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Swync.core.Onedrive.Authentication;
using Swync.core.Onedrive.Items;
using Swync.test.common.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Swync.integration
{
    public class DirectoryTests : IntegrationTestBase
    {
        private readonly ITestOutputHelper _output;
        private readonly DirectoryNavigator _sut;

        public DirectoryTests(ITestOutputHelper output)
        {
            IAuthenticator authenticator = new Authenticator();
            _sut = new DirectoryNavigator(authenticator);
            _output = output;
        }
        
        [Fact]
        public async Task CanGetListOfDrives()
        {
            var drives = await _sut.GetDrivesAsync();

            _output.WriteJson(drives);
            drives.value.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CanGetListOfChildItemsInRootOfDrive()
        {
            var drives = await _sut.GetDrivesAsync();
            var drive = drives.value.TakeRandom();
            
            var directories = await _sut.GetItems(drive);

            _output.WriteJson(directories);
            directories.value.Should().NotBeEmpty();
        }
    }
}
