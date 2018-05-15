using System;
using Newtonsoft.Json;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
namespace Swync.core.Onedrive.Items.Models
{
    public class OnedriveItem
    {
        [JsonProperty("@odata.context")]
        public Uri odataContext { get; set; }
        [JsonProperty("@microsoft.graph.downloadUrl")]
        public Uri microsoftGraphDownloadUrl { get; set; }
        public DateTime? createdDateTime { get; set; }
        public string cTag { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public DateTime? lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public long? size { get; set; }
        public Uri webUrl { get; set; }
        public OnedriveActor createdBy { get; set; }
        public OnedriveActor lastModifiedBy { get; set; }
        public OnedriveItemParentReference parentReference { get; set; }
        public OnedriveItemAudio audio { get; set; }
        public OnedriveItemFile file { get; set; }
        public OnedriveItemFileSystemInfo fileSystemInfo { get; set; }
        public OnedriveItemPackage package { get; set; }
        public OnedriveItemImage image { get; set; }
        public OnedriveItemLocation location { get; set; }
        public OnedriveItemPhoto photo { get; set; }
        public OnedriveItemFolder folder { get; set; }
        public OnedriveItemShared shared { get; set; }
        public OnedriveItemVideo video { get; set; }
        public OnedriveItemSpecialFolder specialFolder { get; set; }
    }

    public class OnedriveItemAudio
    {
        public long bitrate { get; set; }
        public long duration { get; set; }
        public bool hasDrm { get; set; }
    }

    public class OnedriveItemPackage
    {
        public string type { get; set; }
    }

    public class OnedriveItemVideo
    {
        public long bitrate { get; set; }
        public long duration { get; set; }
        public long height { get; set; }
        public long width { get; set; }
        public long audioBitsPerSample { get; set; }
        public long audioChannels { get; set; }
        public string audioFormat { get; set; }
        public long audioSamplesPerSecond { get; set; }
        public string fourCC { get; set; }
        public decimal frameRate { get; set; }
    }

    public class OnedriveItemLocation
    {
        public decimal altitude { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
    }

    public class OnedriveItemImage
    {
        public long height { get; set; }
        public long width { get; set; }
    }

    public class OnedriveItemPhoto
    {
        public string cameraMake { get; set; }
        public string cameraModel { get; set; }
        public decimal? exposureDenominator { get; set; }
        public decimal? exposureNumerator { get; set; }
        public decimal? focalLength { get; set; }
        public decimal? fNumber { get; set; }
        public DateTime takenDateTime { get; set; }
        public long? iso { get; set; }
    }

    public class OnedriveItemFile
    {
        public string mimeType { get; set; }
        public bool? processingMetadata { get; set; }
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
        public string name { get; set; }
        public string path { get; set; }
    }

    public class OnedriveItemFileSystemInfo
    {
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
    }

    public class OnedriveItemFolder
    {
        public long? childCount { get; set; }
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