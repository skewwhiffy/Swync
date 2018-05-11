using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Swync.core.Functional;
using Swync.core.Onedrive.Authentication;
using Swync.core.Onedrive.Directory;
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

            var drivesList = drives.ToList();
            drivesList
                .Pipe(JsonConvert.SerializeObject)
                .Pipe(_output.WriteLine);
            drivesList.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CanGetListOfDirectories()
        {
            var drives = await _sut.GetDrivesAsync();
            var drive = drives.TakeRandom();
            var directories = await _sut.GetDirectories(drive);

            directories.Should().NotBeEmpty();
        }
    }
}
