using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
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
        private readonly Dictionary<HttpRequestMessage, string> _getRequests;
        private readonly ITestOutputHelper _output;
        private readonly ItemNavigator _sut;

        public ItemNavigatorTests(ITestOutputHelper output)
        {
            _getRequests = new Dictionary<HttpRequestMessage, string>();
            IAuthenticator authenticator = new Authenticator();
            var mockClientFactory = new Mock<IHttpClientFactory>();
            var interceptingClient = new InterceptingClient(_getRequests);
            mockClientFactory.Setup(f => f.GetClient()).Returns(interceptingClient);
            var access = new OnedriveAuthenticatedAccess(authenticator, mockClientFactory.Object);
            _sut = new ItemNavigator(access);
            _output = output;
        }
        
        [Fact]
        public async Task CanGetListOfChildItemsInRootOfDrive()
        {
            var drives = await _sut.GetDrivesAsync(CancellationToken.None);
            var drivesPayload = _getRequests.Values.Single();
            drives.SerializeToPrettyJson().Should().Be(drivesPayload.PrettyJson());
            
            _getRequests.Clear();
            
            var drive = drives.value.TakeRandom();
            var directories = await _sut.GetItems(drive, CancellationToken.None);
            var directoriesPayload = _getRequests.Values.Single();
            directories.SerializeToPrettyJson().Should().Be(directoriesPayload.PrettyJson());
        }

        class InterceptingClient : IHttpClient
        {
            private readonly Dictionary<HttpRequestMessage, string> _cache;
            
            public InterceptingClient(Dictionary<HttpRequestMessage, string> cache)
            {
                _cache = cache;
            }
            
            public void Dispose()
            {
            }

            public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken ct)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(message, ct);
                    var payload = await response.Content.ReadAsStringAsync();
                    _cache[message] = payload;
                    return response;
                }
            }
        }
    }
}
