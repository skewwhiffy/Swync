using System;
using System.Net.Http;
using System.Threading.Tasks;
using Swync.core.Onedrive.Authentication;
using Xunit;
using Xunit.Abstractions;

namespace Swync.integration
{
    public class DirectoryTests : IntegrationTestBase
    {
        private readonly ITestOutputHelper _output;

        public DirectoryTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public async Task CanGetListOfDrives()
        {
            var authenticator = new Authenticator();
            var code = await authenticator.GetAccessTokenAsync();
            using (var client = new HttpClient())
            {
                var url = $"https://graph.microsoft.com/v1.0/me/drives";
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };
                request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
                var response = await client.SendAsync(request);
                var payload = await response.Content.ReadAsStringAsync();
                _output.WriteLine(payload);
            }
        }
    }
}
