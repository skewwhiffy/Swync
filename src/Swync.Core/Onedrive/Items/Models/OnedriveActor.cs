// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
namespace Swync.Core.Onedrive.Items.Models
{
    public class OnedriveActor
    {
        public OnedriveUser application { get; set; }
        public OnedriveItemCreatedByDevice device { get; set; }
        public OnedriveUser user { get; set; }
        public OnedriveSync oneDriveSync { get; set; }
    }

    public class OnedriveUser
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }
}