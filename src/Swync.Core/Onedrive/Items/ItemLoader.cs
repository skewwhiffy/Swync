using System;
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
        private readonly IItemNavigator _navigator;

        public ItemLoader(IOnedriveAuthenticatedAccess access, IItemNavigator navigator)
        {
            _access = access;
            _navigator = navigator;
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

        public async Task DownloadFileInPartsAsync(
            string itemId,
            string fullFilePath,
            int partSizeBytes,
            CancellationToken ct)
        {
            var item = await _navigator.GetItemAsync(itemId, ct);
            if (item.size == null)
            {
                throw new ArgumentNullException(nameof(item.size), "Expected size to come back from Onedrive");
            }
            var sizeLong = item.size.Value;
            if (sizeLong > int.MaxValue)
            {
                throw new NotImplementedException("File too large (for the mo)");
            }

            var size = (int) sizeLong;
            var downloadUrl = item.microsoftGraphDownloadUrl;
            using (var file = File.Create(fullFilePath, partSizeBytes))
            using (var writer = new StreamWriter(file))
            {
                var startRange = 0;
                while (startRange < size)
                {
                    var endRange = startRange + partSizeBytes - 1;
                    if (endRange > size)
                    {
                        endRange = size;
                    }
                    var header = Tuple.Create("Range", $"bytes={startRange}-{endRange}");

                    using (var source = await _access.GetContentStreamAsync(downloadUrl, header, ct))
                    {
                        await source.CopyToAsync(file);
                    }

                    startRange = endRange + 1;
                }

                await writer.FlushAsync();
            }
        }
    }
}