using System.Collections.Generic;

namespace Swync.Core.Onedrive.Models
{
    public class OnedriveDrive : IOnedriveItem
    {
        public OnedriveDrive(string id)
        {
            Id = id;
            Drive = this;
            Path = new List<string>();
        }
        
        public string Id { get; }
        public OnedriveDrive Drive { get; }
        public IReadOnlyList<string> Path { get; }
    }
}