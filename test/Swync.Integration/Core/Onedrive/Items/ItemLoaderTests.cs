using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Swync.Core.Onedrive.Authentication;
using Swync.Core.Onedrive.Http;
using Swync.Core.Onedrive.Items;
using Swync.Core.Onedrive.Items.Models;
using Swync.Test.Common.Extensions;
using Swync.Test.Common.Interceptors;
using Swync.Test.Common.TestFiles;
using Xunit;
using Xunit.Abstractions;

namespace Swync.Integration.Core.Onedrive.Items
{
    public class ItemLoaderTests : IDisposable
    {
        private readonly HttpInterceptor _client;
        private readonly TestFolder _folder;
        private readonly ItemNavigator _navigator;
        private readonly ItemLoader _sut;
        private readonly ITestOutputHelper _output;

        public ItemLoaderTests(ITestOutputHelper output)
        {
            _client = new HttpInterceptor();
            var authenticator = new Authenticator();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(f => f.GetClient()).Returns(_client);
            var authenticatedAccess = new OnedriveAuthenticatedAccess(authenticator, httpClientFactory.Object);
            var nonAuthenticatedAccess = new OnedriveNonAuthenticatedAccess(httpClientFactory.Object);
            _folder = TestFolder.WithPrefix("item_loader_tests");
            _navigator = new ItemNavigator(authenticatedAccess);
            _sut = new ItemLoader(authenticatedAccess, nonAuthenticatedAccess, _navigator);
            _output = output;
        }
        
        [Fact]
        public async Task CanUploadAndDeleteSmallFile()
        {
            OnedriveItemDao result = null;
            try
            {
                var file = _folder.CreateRandomFile(100);
                var drives = await _navigator.GetDrivesAsync(CancellationToken.None);
                var directories = await _navigator.GetItemsAsync(drives.value.TakeRandom().id, CancellationToken.None);
                var directory = directories.value.Where(it => it.folder != null).TakeRandom();
                var remoteFileName = $"_item_upload_test_{Guid.NewGuid()}.swync.test";
                _client.ClearCache();

                result = await _sut.UploadNewFileAsync(directory.id, remoteFileName, file, CancellationToken.None);

                result.SerializeToPrettyJson().Should().Be(_client.Cache.Values.Single().PrettyJson());
                _output.WriteLine(directory.name);

                var downloadedFile = Path.Combine(_folder.Info.FullName, $"{Guid.NewGuid()}.swync.test");
                await _sut.DownloadFileAsync(result.id, downloadedFile, CancellationToken.None);
                AssertFilesBinaryIdentical(file, downloadedFile);

                var downloadedFileInParts = Path.Combine(_folder.Info.FullName, $"{Guid.NewGuid()}.sywnc.partial.test");
                await _sut.DownloadFileInPartsAsync(result.id, downloadedFileInParts, 17, CancellationToken.None);
                AssertFilesBinaryIdentical(file, downloadedFileInParts);
            }
            finally
            {
                if (result != null)
                {
                    await _sut.DeleteFileAsync(result.id, CancellationToken.None);
                }
            }
        }

        [Fact(Skip = "Need to get this working")]
        public async Task CanUploadFileInChunks()
        {
            OnedriveItemDao result = null;
            try
            {
                var file = _folder.CreateRandomFile(3000);
                var drives = await _navigator.GetDrivesAsync(CancellationToken.None);
                var directories = await _navigator.GetItemsAsync(drives.value.TakeRandom().id, CancellationToken.None);
                var directory = directories.value.Where(it => it.folder != null).TakeRandom();
                var remoteFileName = $"_item_upload_in_parts_test_{Guid.NewGuid()}.swync.test";
                _client.ClearCache();

                result = await _sut.UploadNewFileInPartsAsync(
                    directory.id,
                    remoteFileName,
                    file,
                    117,
                    CancellationToken.None);
            }
            finally
            {
                if (result != null)
                {
                    await _sut.DeleteFileAsync(result.id, CancellationToken.None);
                }
            }
        }

        private void AssertFilesBinaryIdentical(FileInfo file, string downloadedFile)
        {
            using (var source = file.OpenRead())
            using (var destination = new FileStream(downloadedFile, FileMode.Open, FileAccess.Read))
            {
                source.Length.Should().Be(destination.Length);
                for (var i = 0; i < source.Length; i++)
                {
                    source.ReadByte().Should().Be(destination.ReadByte(), $"{downloadedFile} differs");
                }
            }
        }

        public void Dispose()
        {
            _folder.Dispose();
        }
    }
}