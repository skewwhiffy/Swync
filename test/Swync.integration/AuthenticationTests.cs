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
        }
    }
}
