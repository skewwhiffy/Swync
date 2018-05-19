using Newtonsoft.Json;
using Swync.Core.Functional;
using Xunit.Abstractions;

namespace Swync.Test.Common.Extensions
{
    public static class XunitExtensions
    {
        public static void WriteJson<T>(this ITestOutputHelper output, T item) => item
            .Pipe(SerializeToPrettyJson)
            .Pipe(output.WriteLine);

        public static string SerializeToPrettyJson<T>(this T it)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(it, Formatting.Indented, settings);
        }
    }
}