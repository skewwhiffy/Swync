using System.Threading;
using System.Threading.Tasks;
using Swync.Core.Onedrive.Http;
using Swync.Core.Onedrive.Items.Models;

namespace Swync.Core.Onedrive.Items
{
    public interface IItemNavigator
    {
        Task<OnedriveContainerDao<OnedriveDriveDao>> GetDrivesAsync(CancellationToken ct);
        Task<OnedriveItemDao> GetItemAsync(string itemId, CancellationToken ct);
        Task<OnedriveContainerDao<OnedriveItemDao>> GetItemsAsync(string driveId, CancellationToken ct);
        Task<OnedriveContainerDao<OnedriveItemDao>> GetChildrenAsync(string parentId, CancellationToken ct);
    }
    
    public class ItemNavigator : IItemNavigator
    {
        private readonly IOnedriveAuthenticatedAccess _access;

        public ItemNavigator(IOnedriveAuthenticatedAccess access)
        {
            _access = access;
        }

        public Task<OnedriveContainerDao<OnedriveDriveDao>> GetDrivesAsync(CancellationToken ct) =>
            _access.GetAsync<OnedriveContainerDao<OnedriveDriveDao>>("drives", ct);

        public Task<OnedriveItemDao> GetItemAsync(
            string itemId,
            CancellationToken ct) =>
            _access.GetAsync<OnedriveItemDao>($"drive/items/{itemId}", ct);
        
        public Task<OnedriveContainerDao<OnedriveItemDao>> GetItemsAsync(string driveId, CancellationToken ct) =>
            _access.GetAsync<OnedriveContainerDao<OnedriveItemDao>>($"drives/{driveId}/root/children", ct);

        public Task<OnedriveContainerDao<OnedriveItemDao>> GetChildrenAsync(string parentId, CancellationToken ct) =>
            _access.GetAsync<OnedriveContainerDao<OnedriveItemDao>>($"drive/items/{parentId}/children", ct);

        public async Task<OnedriveItemDao> CreateDirectoryAsync(OnedriveDriveDao drive, string name, CancellationToken ct)
        {
            var item = new OnedriveItemDao
            {
                name = name,
                folder = new OnedriveItemFolderDao()
            };
                
            return await _access.PostAsync<OnedriveItemDao, OnedriveItemDao>($"drives/{drive.id}/root/children", item, ct);
        }

        public async Task DeleteItemAsync(OnedriveDriveDao drive, string itemId, CancellationToken ct)
        {
            await _access.DeleteAsync($"drives/{drive.id}/items/{itemId}", ct);
        }
    }
}