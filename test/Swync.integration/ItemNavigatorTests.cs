using System.Linq;
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
using Swync.test.common.Interceptors;
using Xunit;

namespace Swync.integration
{
    public class ItemNavigatorTests : IntegrationTestBase
    {
        private readonly bool _exhaustive;
        private readonly ItemNavigator _sut;
        private readonly HttpInterceptor _interceptingClient;

        public ItemNavigatorTests()
        {
            IAuthenticator authenticator = new Authenticator();
            var mockClientFactory = new Mock<IHttpClientFactory>();
            _interceptingClient = new HttpInterceptor();
            mockClientFactory.Setup(f => f.GetClient()).Returns(_interceptingClient);
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
            var drivesPayload =_interceptingClient.Cache.Values.Single();
            drives.SerializeToPrettyJson().Should().Be(drivesPayload.PrettyJson());
            
            _interceptingClient.ClearCache();

            var drivesToTest = _exhaustive ? drives.value : new[] {drives.value.TakeRandom()};
            foreach (var drive in drivesToTest)
            {
                var itemsAtRoot = await _sut.GetItemsAsync(drive, CancellationToken.None);
                var directoriesPayload = _interceptingClient.Cache.Values.Single();
                itemsAtRoot.SerializeToPrettyJson().Should().Be(directoriesPayload.PrettyJson());
                
                _interceptingClient.ClearCache();

                var foldersToTest = _exhaustive ? itemsAtRoot.value : new[] {itemsAtRoot.value.TakeRandom()};
                foreach (var folderAtRoot in foldersToTest)
                {
                    var childItems = await _sut.GetChildrenAsync(folderAtRoot.id, CancellationToken.None);
                    var childItemsPayload = _interceptingClient.Cache.Values.Single();
                    childItems.SerializeToPrettyJson().Should().Be(childItemsPayload.PrettyJson());
                    
                    _interceptingClient.ClearCache();
                }
            }
        }
    }
}
