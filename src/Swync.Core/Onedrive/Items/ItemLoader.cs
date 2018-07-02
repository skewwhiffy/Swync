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
        private readonly IOnedriveNonAuthenticatedAccess _nonAuthenticatedAccess;
        private readonly IOnedriveAuthenticatedAccess _authenticatedAccess;
        private readonly IItemNavigator _navigator;

        public ItemLoader(
            IOnedriveAuthenticatedAccess authenticatedAccess,
            IOnedriveNonAuthenticatedAccess nonAuthenticatedAccess,
            IItemNavigator navigator)
        {
            _authenticatedAccess = authenticatedAccess;
            _navigator = navigator;
            _nonAuthenticatedAccess = nonAuthenticatedAccess;
        }

        public async Task<OnedriveItemDao> UploadNewFileAsync(
            string parentId,
            string remoteFileName,
            FileInfo file,
            CancellationToken ct)
        {
            var bytes = File.ReadAllBytes(file.FullName);
            return await _authenticatedAccess.PutAsync<OnedriveItemDao>(
                $"drive/items/{parentId}:/{remoteFileName}:/content",
                bytes,
                ct);
        }

        public async Task<OnedriveItemDao> UploadNewFileInPartsAsync(
            string parentId,
            string remoteFileName,
            FileInfo file,
            int partBytesCount,
            CancellationToken ct)
        {
            throw new NotImplementedException("This doesn't work. Best fix it at some point when you need it.");
            var session = await _authenticatedAccess.PostAsync<object, OnedriveUploadSessionDao>(
                    $"drive/items/{parentId}:/{remoteFileName}:/createUploadSession",
                    new {},
                    ct);
            var uri = session.uploadUrl;
            var fileSize = file.Length;
            var start = 0L;
            var end = start + partBytesCount - 1;
            OnedriveItemDao response = null;
            using (var stream = file.OpenRead())
            using (var reader = new BinaryReader(stream))
            {
                while (start < fileSize)
                {
                    if (end >= fileSize)
                    {
                        end = fileSize;
                    }

                    var contentLength = (int)(end - start + 1);
                    var bytes = reader.ReadBytes(contentLength);
                    response = await _nonAuthenticatedAccess.PutAsync<OnedriveItemDao>(
                        uri,
                        bytes,
                        ct,
                        Tuple.Create("Content-Length", contentLength.ToString()),
                        Tuple.Create("Content-Range", $"bytes {start}-{end}/{fileSize}"));
                    start = end + 1;
                }
            }

            return response;
        }
            
        public async Task DeleteFileAsync(string itemId, CancellationToken ct)
        {
            await _authenticatedAccess.DeleteAsync($"drive/items/{itemId}", ct);
        }

        public async Task DownloadFileAsync(
            string itemId,
            string fullFilePath,
            CancellationToken ct)
        {
            using (var source = await _authenticatedAccess.GetContentStreamAsync($"drive/items/{itemId}/content", ct))
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

                    using (var source = await _authenticatedAccess.GetContentStreamAsync(downloadUrl, header, ct))
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