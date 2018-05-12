using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Swync.core.Functional;
using Swync.core.Onedrive.Authentication;
using Swync.core.Onedrive.Http;
using Swync.core.Onedrive.Items;
using Swync.test.common.Extensions;
using Xunit;

namespace Swync.integration
{
    public class ItemNavigatorTests : IntegrationTestBase
    {
        private readonly bool _exhaustive;
        private readonly Dictionary<HttpRequestMessage, string> _getRequests;
        private readonly ItemNavigator _sut;

        public ItemNavigatorTests()
        {
            _getRequests = new Dictionary<HttpRequestMessage, string>();
            IAuthenticator authenticator = new Authenticator();
            var mockClientFactory = new Mock<IHttpClientFactory>();
            var interceptingClient = new InterceptingClient(_getRequests);
            mockClientFactory.Setup(f => f.GetClient()).Returns(interceptingClient);
            var access = new OnedriveAuthenticatedAccess(authenticator, mockClientFactory.Object);
            _sut = new ItemNavigator(access);

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _exhaustive = config["exhaustive"].Pipe(bool.Parse);
        }
        
        [Fact]
        public async Task CanNavigateChildren()
        {
            var drives = await _sut.GetDrivesAsync(CancellationToken.None);
            var drivesPayload = _getRequests.Values.Single();
            drives.SerializeToPrettyJson().Should().Be(drivesPayload.PrettyJson());
            
            _getRequests.Clear();

            var drivesToTest = _exhaustive ? drives.value : new[] {drives.value.TakeRandom()};
            foreach (var drive in drivesToTest)
            {
                var itemsAtRoot = await _sut.GetItemsAsync(drive, CancellationToken.None);
                var directoriesPayload = _getRequests.Values.Single();
                itemsAtRoot.SerializeToPrettyJson().Should().Be(directoriesPayload.PrettyJson());
                
                _getRequests.Clear();

                var foldersToTest = _exhaustive ? itemsAtRoot.value : new[] {itemsAtRoot.value.TakeRandom()};
                foreach (var folderAtRoot in foldersToTest)
                {
                    var childItems = await _sut.GetChildrenAsync(folderAtRoot.id, CancellationToken.None);
                    var childItemsPayload = _getRequests.Values.Single();
                    childItems.SerializeToPrettyJson().Should().Be(childItemsPayload.PrettyJson());
                    
                    _getRequests.Clear();
                }
            }
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
