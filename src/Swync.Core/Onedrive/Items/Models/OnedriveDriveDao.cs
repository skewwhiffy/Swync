// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
namespace Swync.Core.Onedrive.Items.Models
{
    public class OnedriveDriveDao
    {
        public string id { get; set; }
        public string driveType { get; set; }
        public OnedriveActorDao owner { get; set; }
        public OnedriveQuotaDao quota { get; set; }
    }

    public class OnedriveQuotaDao
    {
        public long deleted { get; set; }
        public long remaining { get; set; }
        public string state { get; set; }
        public long total { get; set; }
        public long used { get; set; }
    }
}
