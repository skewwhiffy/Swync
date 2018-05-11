
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
namespace Swync.core.Onedrive.Items
{
    public class OnedriveDrive
    {
        public string id { get; set; }
        public string driveType { get; set; }
        public OnedriveDriveOwner owner { get; set; }
        public OnedriveQuota quota { get; set; }
    }

    public class OnedriveDriveOwner
    {
        public OnedriveDriveUser user { get; set; }
    }

    public class OnedriveDriveUser
    {
        public string id { get; set; }
        public string displayName { get; set; }
    }

    public class OnedriveQuota
    {
        public long deleted { get; set; }
        public long remaining { get; set; }
        public string state { get; set; }
        public long total { get; set; }
        public long used { get; set; }
    }
}
