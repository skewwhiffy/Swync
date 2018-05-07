using Newtonsoft.Json;

namespace Swync.core.Onedrive.Directory
{
    public class Drive
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("driveType")]
        public string DriveType { get; set; }
    }
}