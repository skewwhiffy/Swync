using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Swync.Core.Onedrive.Http;

namespace Swync.Test.Common.Interceptors
{
    public class HttpInterceptor : IHttpClient
    {
        private readonly ConcurrentDictionary<HttpRequestMessage, string> _cache;

        public HttpInterceptor()
        {
            _cache = new ConcurrentDictionary<HttpRequestMessage, string>();
        }
        
        public void Dispose()
        {
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken ct)
        {
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(message, ct);
                var payload = await response.Content.ReadAsStringAsync();
                _cache[message] = payload;
                return response;
            }
        }

        public ImmutableDictionary<HttpRequestMessage, string> Cache => _cache
            .ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public void ClearCache() => _cache.Clear();
    }
}