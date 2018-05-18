using Newtonsoft.Json;
using Swync.core.Functional;

namespace Swync.test.common.Extensions
{
    public static class JsonExtensions
    {
        public static string PrettyJson(this string json) => json
            .Pipe(JsonConvert.DeserializeObject)
            .Pipe(it => JsonConvert.SerializeObject(it, Formatting.Indented));
    }
}