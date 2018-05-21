using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Swync.Core.Functional;
using Swync.Core.Onedrive.Items;
using Swync.Core.Onedrive.Models;

namespace Swync.Core.Onedrive
{
    public class OnedriveClient
    {
        private readonly IItemNavigator _itemNavigator;

        public OnedriveClient(IItemNavigator itemNavigator)
        {
            _itemNavigator = itemNavigator;
        }
        
        public async Task<IList<OnedriveDrive>> GetDrivesAsync(CancellationToken ct)
        {
            var drives = await _itemNavigator.GetDrivesAsync(ct);
            return drives.value.Select(it => it.ToModel()).ToList();
        }

        public async Task CycleThroughDirectoriesAsync(
            Func<OnedriveDirectory, Task> actionPerDirectory,
            Func<Task> actionWhenDone,
            CancellationToken ct)
        {
            var drives = await GetDrivesAsync(ct);
            await drives
                .Select(d => CycleThroughDirectoriesAsync(d, actionPerDirectory, ct))
                .Pipe(Task.WhenAll);
            await actionWhenDone();
        }

        private async Task CycleThroughDirectoriesAsync(
            OnedriveDrive drive,
            Func<OnedriveDirectory, Task> actionPerDirectory,
            CancellationToken ct)
        {
            var childrenRaw = await _itemNavigator.GetItemsAsync(drive.Id, ct);
            var childrenDirectories = childrenRaw
                .value
                .Where(it => it.folder != null)
                .Select(it => it.ToModel(drive, drive))
                .ToList();
            var actionTasks = childrenDirectories.Select(actionPerDirectory);
            var getChildrenTasks = childrenDirectories.Select(it => CycleThroughDirectoriesAsync(it, actionPerDirectory, ct));
            await actionTasks
                .Union(getChildrenTasks)
                .Pipe(Task.WhenAll);
        }

        private async Task CycleThroughDirectoriesAsync(
            OnedriveDirectory directory,
            Func<OnedriveDirectory, Task> actionPerDirectory,
            CancellationToken ct)
        {
            var childrenRaw = await _itemNavigator.GetChildrenAsync(directory.ItemId, ct);
            var childrenDirectories = childrenRaw
                .value
                .Where(it => it.folder != null)
                .Select(it => it.ToModel(directory.Drive, directory))
                .ToList();
            var actionTasks = childrenDirectories.Select(actionPerDirectory);
            var getChildrenTasks = childrenDirectories
                .Select(it => CycleThroughDirectoriesAsync(it, actionPerDirectory, ct));
            await actionTasks
                .Union(getChildrenTasks)
                .Pipe(Task.WhenAll);

        }

        public async Task<IList<OnedriveDirectory>> GetDirectories(CancellationToken ct)
        {
            var drives = await GetDrivesAsync(ct);
            var result = new List<OnedriveDirectory>();
            foreach (var drive in drives)
            {
                result.AddRange(await GetDirectories(drive, ct));
            }

            return result;
        }

        private async Task<IEnumerable<OnedriveDirectory>> GetDirectories(OnedriveDrive drive, CancellationToken ct)
        {
            var children = await _itemNavigator.GetItemsAsync(drive.Id, ct);
            var result = children
                .value
                .Where(it => it.folder != null)
                .Select(it => it.ToModel(drive, drive))
                .ToList();
            var recursive = new List<OnedriveDirectory>();
            foreach (var directory in result)
            {
                recursive.AddRange(await GetDirectories(directory, ct));
            }

            result.AddRange(recursive);
            return result;
        }

        private async Task<IEnumerable<OnedriveDirectory>> GetDirectories(
            OnedriveDirectory directory,
            CancellationToken ct)
        {
            var children = await _itemNavigator.GetChildrenAsync(directory.ItemId, ct);
            var result = children
                .value
                .Where(it => it.folder != null)
                .Select(it => it.ToModel(directory.Drive, directory))
                .ToList();
            var recursive = new List<OnedriveDirectory>();
            foreach (var subDirectory in result)
            {
                recursive.AddRange(await GetDirectories(subDirectory, ct));
            }

            result.AddRange(recursive);
            return result;
            
        }
    }
}