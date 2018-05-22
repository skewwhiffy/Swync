using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Swync.Core.Onedrive.Http;
using Swync.Core.Onedrive.Items.Models;

namespace Swync.Core.Onedrive.Items
{
    public class ItemLoader
    {
        private readonly IOnedriveAuthenticatedAccess _access;

        public ItemLoader(IOnedriveAuthenticatedAccess access)
        {
            _access = access;
        }

        public async Task<OnedriveItemDao> UploadNewFileAsync(
            string parentId,
            string remoteFileName,
            FileInfo file,
            CancellationToken ct)
        {
            var bytes = File.ReadAllBytes(file.FullName);
            return await _access.PutAsync<OnedriveItemDao>(
                $"drive/items/{parentId}:/{remoteFileName}:/content",
                bytes,
                ct);
        }

        public async Task DeleteFileAsync(string itemId, CancellationToken ct)
        {
            await _access.DeleteAsync($"drive/items/{itemId}", ct);
        }

        public async Task DownloadFileAsync(
            string itemId,
            string fullFilePath,
            CancellationToken ct)
        {
            using (var source = await _access.GetContentStreamAsync($"drive/items/{itemId}/content", ct))
            using (var destination = new FileStream(fullFilePath, FileMode.CreateNew, FileAccess.Write))
            {
                await source.CopyToAsync(destination);
            }
        }
    }
}