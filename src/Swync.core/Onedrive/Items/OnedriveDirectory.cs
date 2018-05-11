using System;
using Newtonsoft.Json;

namespace Swync.core.Onedrive.Items
{
    public class OnedriveItem
    {
        [JsonProperty("createdDateTime")]
        public DateTime Created { get; set; }
        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}