using Newtonsoft.Json;
using Swync.Core.Functional;

namespace Swync.Test.Common.Extensions
{
    public static class JsonExtensions
    {
        public static string PrettyJson(this string json) => json
            .Pipe(JsonConvert.DeserializeObject)
            .Pipe(it => JsonConvert.SerializeObject(it, Formatting.Indented));
    }
}