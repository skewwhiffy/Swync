using System.Collections.Generic;

namespace Swync.Core.Onedrive.Models
{
    public interface IOnedriveItem
    {
        OnedriveDrive Drive { get; }
        IReadOnlyList<string> Path { get; }
    }
}