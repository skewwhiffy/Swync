using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Swync.Core.Functional;
using Swync.Core.Onedrive.Authentication;
using Swync.Core.Onedrive.Http;
using Swync.Core.Onedrive.Items;
using Swync.Core.Onedrive.Items.Models;
using Swync.Test.Common.Extensions;
using Swync.Test.Common.Interceptors;
using Xunit;

namespace Swync.Integration.Core.Onedrive.Items
{
    public class ItemNavigatorTests
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
                var itemsAtRoot = await _sut.GetItemsAsync(drive.id, CancellationToken.None);
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

        [Fact]
        public async Task CanCreateAndDeleteFolderAtRootOfDrive()
        {
            var drives = await _sut.GetDrivesAsync(CancellationToken.None);
            _interceptingClient.ClearCache();
            var drivesToTest = _exhaustive ? drives.value : new[] {drives.value.TakeRandom()};
            foreach (var drive in drivesToTest)
            {
                var directoryName = $"__swync_test_folder_{Guid.NewGuid()}";
                var item = await CanCreateFolderAtRootOfDriveAsync(drive, directoryName);

                await CanDeleteFolderAtRootOfDrive(drive, directoryName, item.id);

            }
        }

        private async Task<OnedriveItemDao> CanCreateFolderAtRootOfDriveAsync(OnedriveDriveDao drive, string directoryName)
        {
            var responseItem = await _sut.CreateDirectoryAsync(drive, directoryName, CancellationToken.None);
            var response = _interceptingClient.Cache.Values.Single().PrettyJson();
            responseItem.SerializeToPrettyJson().Should().Be(response);
            var items = await _sut.GetItemsAsync(drive.id, CancellationToken.None);
            var item = items.value.Single(it => it.name == directoryName);
            item.folder.Should().NotBeNull();
            _interceptingClient.ClearCache();
            return responseItem;
        }

        private async Task CanDeleteFolderAtRootOfDrive(OnedriveDriveDao drive, string directoryName,  string itemId)
        {
            await _sut.DeleteItemAsync(drive, itemId, CancellationToken.None);
            var items = await _sut.GetItemsAsync(drive.id, CancellationToken.None);
            items.value.Where(it => it.name == directoryName).Should().BeEmpty();
            _interceptingClient.ClearCache();
        }
    }
}
