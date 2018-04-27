using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.OneDrive.Sdk;
using Swync.core.Authentication;
using Swync.integration.Config;
using Xunit;

namespace Swync.integration
{
    public class AuthenticationTests : IntegrationTestBase
    {
        [Fact]
        public void CredentialsHaveBeenSet()
        {
            var config = new AppSettings();

            var credentials = config.OnedriveCredentials;
            credentials.HasValue.Should().BeTrue();
            credentials.Value.Username.Should().NotBeNullOrWhiteSpace();
            credentials.Value.Password.Should().NotBeNullOrWhiteSpace();
        }
    }
}
