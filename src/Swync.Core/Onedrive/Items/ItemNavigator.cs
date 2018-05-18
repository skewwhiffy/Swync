using System.Threading;
using System.Threading.Tasks;
using Swync.core.Onedrive.Http;
using Swync.core.Onedrive.Items.Models;

namespace Swync.core.Onedrive.Items
{
    public class ItemNavigator
    {
        private readonly IOnedriveAuthenticatedAccess _access;

        public ItemNavigator(IOnedriveAuthenticatedAccess access)
        {
            _access = access;
        }

        public Task<OnedriveContainer<OnedriveDrive>> GetDrivesAsync(CancellationToken ct) =>
            _access.GetAsync<OnedriveContainer<OnedriveDrive>>("drives", ct);

        public Task<OnedriveContainer<OnedriveItem>> GetItemsAsync(OnedriveDrive drive, CancellationToken ct) =>
            _access.GetAsync<OnedriveContainer<OnedriveItem>>($"drives/{drive.id}/root/children", ct);

        public Task<OnedriveContainer<OnedriveItem>> GetChildrenAsync(string parentId, CancellationToken ct) =>
            _access.GetAsync<OnedriveContainer<OnedriveItem>>($"drive/items/{parentId}/children", ct);

        public async Task<OnedriveItem> CreateDirectoryAsync(OnedriveDrive drive, string name, CancellationToken ct)
        {
            var item = new OnedriveItem
            {
                name = name,
                folder = new OnedriveItemFolder()
            };
                
            return await _access.PostAsync<OnedriveItem, OnedriveItem>($"drives/{drive.id}/root/children", item, ct);
        }

        public async Task DeleteItemAsync(OnedriveDrive drive, string itemId, CancellationToken ct)
        {
            await _access.DeleteAsync($"drives/{drive.id}/items/{itemId}", ct);
        }
    }
}