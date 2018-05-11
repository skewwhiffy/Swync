using Newtonsoft.Json;
using Swync.core.Functional;
using Xunit.Abstractions;

namespace Swync.test.common.Extensions
{
    public static class XunitExtensions
    {
        public static void WriteJson<T>(this ITestOutputHelper output, T item) => item
            .Pipe(it => JsonConvert.SerializeObject(it, Formatting.Indented))
            .Pipe(output.WriteLine);
    }
}