using System;
using Newtonsoft.Json;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
namespace Swync.Core.Onedrive.Items.Models
{
    public class OnedriveItemDao
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
        public OnedriveActorDao createdBy { get; set; }
        public OnedriveActorDao lastModifiedBy { get; set; }
        public OnedriveItemParentReferenceDao parentReference { get; set; }
        public OnedriveItemAudioDao audio { get; set; }
        public OnedriveItemFileDao file { get; set; }
        public OnedriveItemFileSystemInfoDao fileSystemInfo { get; set; }
        public OnedriveItemPackageDao package { get; set; }
        public OnedriveItemImageDao image { get; set; }
        public OnedriveItemLocationDao location { get; set; }
        public OnedriveItemPhotoDao photo { get; set; }
        public OnedriveItemFolderDao folder { get; set; }
        public OnedriveItemSharedDao shared { get; set; }
        public OnedriveItemVideoDao video { get; set; }
        public OnedriveItemSpecialFolderDao specialFolder { get; set; }
    }

    public class OnedriveItemAudioDao
    {
        public long bitrate { get; set; }
        public long duration { get; set; }
        public bool hasDrm { get; set; }
    }

    public class OnedriveItemPackageDao
    {
        public string type { get; set; }
    }

    public class OnedriveItemVideoDao
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

    public class OnedriveItemLocationDao
    {
        public decimal altitude { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
    }

    public class OnedriveItemImageDao
    {
        public long height { get; set; }
        public long width { get; set; }
    }

    public class OnedriveItemPhotoDao
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

    public class OnedriveItemFileDao
    {
        public string mimeType { get; set; }
        public bool? processingMetadata { get; set; }
        public OnedriveItemFileHashesDao hashes { get; set; }
    }

    public class OnedriveItemFileHashesDao
    {
        public string crc32Hash { get; set; }
        public string sha1Hash { get; set; }
    }

    public class OnedriveItemSpecialFolderDao
    {
        public string name { get; set; }
    }

    public class OnedriveItemCreatedByDeviceDao
    {
        public string id { get; set; }
    }

    public class OnedriveSyncDao
    {
        [JsonProperty("@odata.type")]
        public string odataType { get; set; }
        public string id { get; set; }
    }

    public class OnedriveItemParentReferenceDao
    {
        public string driveId { get; set; }
        public string driveType { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
    }

    public class OnedriveItemFileSystemInfoDao
    {
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
    }

    public class OnedriveItemFolderDao
    {
        public long? childCount { get; set; }
        public OnedriveItemFolderViewDao view { get; set; }
    }

    public class OnedriveItemFolderViewDao
    {
        public string viewType { get; set; }
        public string sortBy { get; set; }
        public string sortOrder { get; set; }
    }

    public class OnedriveItemSharedDao
    {
        public string scope { get; set; }
        public OnedriveActorDao owner { get; set; }
    }
}