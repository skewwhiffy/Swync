using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Swync.core.Onedrive.Authentication;
using Swync.core.Onedrive.Directory;
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
            _output.WriteLine(JsonConvert.SerializeObject(drives));
        }
    }
}
