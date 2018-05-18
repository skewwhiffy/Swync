using Swync.Core.Onedrive.Items.Models;

namespace Swync.Core.Onedrive.Models
{
    public static class Mapping
    {
        public static OnedriveDrive ToModel(this OnedriveDriveDao dao)
        {
            return new OnedriveDrive(dao.id);
        }

        public static OnedriveDirectory ToModel(this OnedriveItemDao dao, OnedriveDrive drive, IOnedriveItem parent)
        {
            return new OnedriveDirectory(drive, parent, dao.id, dao.name);
        }
    }
}