using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
namespace Swync.Core.Onedrive.Items.Models
{
    public class OnedriveUploadSessionDao
    {
        [JsonProperty("@odata.context")]
        public Uri odataContext { get; set; }
        public Uri uploadUrl { get; set; }
        public DateTime expirationDateTime { get; set; }
        public IList<string> nextExpectedRanges { get; set; }
        
    }
}