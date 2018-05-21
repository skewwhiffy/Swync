using System.Collections.Generic;
using System.Linq;

namespace Swync.Core.Onedrive.Models
{
    public class OnedriveDirectory : IOnedriveItem
    {
        public OnedriveDirectory(OnedriveDrive drive, IOnedriveItem parent, string itemId, string name)
        {
            Drive = drive;
            Parent = parent;
            ItemId = itemId;
            var path = parent.Path.ToList();
            path.Add(name);
            Path = path;
            Name = name;
        }

        public OnedriveDrive Drive { get; }
        public IReadOnlyList<string> Path { get; }
        public IOnedriveItem Parent { get; }
        public string ItemId { get; }
        public string Name { get; }
    }
}