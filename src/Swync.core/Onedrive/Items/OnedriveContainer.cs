// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CollectionNeverUpdated.Global
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Swync.core.Onedrive.Items
{
    public class OnedriveContainer<T>
    {
            [JsonProperty("@odata.context")]
            public Uri OdataContext { get; set; }
            public IList<T> value { get; set; }
    }
}