using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Swync.Core.Onedrive.Http
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public IHttpClient GetClient() => new RealHttpClientWrapper();
    }

    public interface IHttpClientFactory
    {
        IHttpClient GetClient();
    }

    public class RealHttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _client;

        public RealHttpClientWrapper()
        {
            _client = new HttpClient();
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken ct)
        {
            return _client.SendAsync(message, ct);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }

    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken ct);
    }
}