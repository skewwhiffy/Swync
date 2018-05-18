// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CollectionNeverUpdated.Global

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Swync.Core.Onedrive.Items.Models
{
    public class OnedriveContainerDao<T>
    {
        [JsonProperty("@odata.context")]
        public Uri OdataContext { get; set; }
        [JsonProperty("@odata.nextLink")]
        public Uri OdataNextLink { get; set; }
        public IList<T> value { get; set; }
    }
}