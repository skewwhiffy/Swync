using System;
using Newtonsoft.Json;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
namespace Swync.core.Onedrive.Items
{
    public class OnedriveItem
    {
        [JsonProperty("@microsoft.graph.downloadUrl")]
        public Uri microsoftGraphDownloadUrl { get; set; }
        public DateTime createdDateTime { get; set; }
        public string cTag { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public long size { get; set; }
        public Uri webUrl { get; set; }
        public OnedriveActor createdBy { get; set; }
        public OnedriveActor lastModifiedBy { get; set; }
        public OnedriveItemParentReference parentReference { get; set; }
        public OnedriveItemFile file { get; set; }
        public OnedriveItemFileSystemInfo fileSystemInfo { get; set; }
        public OnedriveItemFolder folder { get; set; }
        public OnedriveItemShared shared { get; set; }
        public OnedriveItemSpecialFolder specialFolder { get; set; }
    }

    public class OnedriveItemFile
    {
        public string mimeType { get; set; }
        public OnedriveItemFileHashes hashes { get; set; }
    }

    public class OnedriveItemFileHashes
    {
        public string crc32Hash { get; set; }
        public string sha1Hash { get; set; }
    }

    public class OnedriveItemSpecialFolder
    {
        public string name { get; set; }
    }

    public class OnedriveItemCreatedByDevice
    {
        public string id { get; set; }
    }

    public class OnedriveSync
    {
        [JsonProperty("@odata.type")]
        public string odataType { get; set; }
        public string id { get; set; }
    }

    public class OnedriveItemParentReference
    {
        public string driveId { get; set; }
        public string driveType { get; set; }
        public string id { get; set; }
        public string path { get; set; }
    }

    public class OnedriveItemFileSystemInfo
    {
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
    }

    public class OnedriveItemFolder
    {
        public long childCount { get; set; }
        public OnedriveItemFolderView view { get; set; }
    }

    public class OnedriveItemFolderView
    {
        public string viewType { get; set; }
        public string sortBy { get; set; }
        public string sortOrder { get; set; }
    }

    public class OnedriveItemShared
    {
        public string scope { get; set; }
        public OnedriveActor owner { get; set; }
    }
}