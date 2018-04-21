using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Swync.integration
{
    public class AuthenticationTests
    {
        [Fact]
        public void CredentialsHaveBeenSet()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            config["onedrive:username"].Should().NotBeNullOrEmpty();
            config["onedrive:password"].Should().NotBeNullOrEmpty();
        }
    }
}
