// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
namespace Swync.Core.Onedrive.Items.Models
{
    public class OnedriveActorDao
    {
        public OnedriveUserDao application { get; set; }
        public OnedriveItemCreatedByDeviceDao device { get; set; }
        public OnedriveUserDao user { get; set; }
        public OnedriveSyncDao oneDriveSync { get; set; }
    }

    public class OnedriveUserDao
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }
}