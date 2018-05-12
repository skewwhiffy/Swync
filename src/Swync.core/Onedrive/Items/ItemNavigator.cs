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

        public Task<OnedriveContainer<OnedriveDrive>> GetDrivesAsync() =>
            _access.GetAsync<OnedriveContainer<OnedriveDrive>>("drives");

        public Task<OnedriveContainer<OnedriveItem>> GetItems(OnedriveDrive drive) =>
            _access.GetAsync<OnedriveContainer<OnedriveItem>>($"drives/{drive.id}/root/children");
    }
}