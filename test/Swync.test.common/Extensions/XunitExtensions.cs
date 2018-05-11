using Newtonsoft.Json;
using Swync.core.Functional;
using Xunit.Abstractions;

namespace Swync.test.common.Extensions
{
    public static class XunitExtensions
    {
        public static void WriteJson<T>(this ITestOutputHelper output, T item) => item
            .Pipe(Serialize)
            .Pipe(output.WriteLine);

        private static string Serialize<T>(T it)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(it, Formatting.Indented, settings);
        }
    }
}