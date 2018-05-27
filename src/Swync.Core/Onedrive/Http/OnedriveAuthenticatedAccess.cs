using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Swync.Core.Functional;
using Swync.Core.Onedrive.Authentication;

namespace Swync.Core.Onedrive.Http
{
    public class OnedriveAuthenticatedAccess : IOnedriveAuthenticatedAccess
    {
        private readonly IAuthenticator _authenticator;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;

        public OnedriveAuthenticatedAccess(
            IAuthenticator authenticator,
            IHttpClientFactory httpClientFactory)
        { _authenticator = authenticator;
            _httpClientFactory = httpClientFactory;
            _baseUrl = "https://graph.microsoft.com/v1.0/me/";
        }

        public Task<Stream> GetContentStreamAsync(string relativeUrl, CancellationToken ct) => Invoke(
            relativeUrl,
            it => it.Method = HttpMethod.Get,
            it => it.Content.ReadAsStreamAsync(),
            ct);
        
        public Task<Stream> GetContentStreamAsync(
            Uri absoluteUri,
            Tuple<string, string> header,
            CancellationToken ct) => Invoke(
            absoluteUri,
            it =>
            {
                it.Method = HttpMethod.Get;
                it.Headers.Add(header.Item1, header.Item2);
            },
            it => it.Content.ReadAsStreamAsync(),
            ct);

        public Task<T> GetAsync<T>(string relativeUrl, CancellationToken ct) => Invoke(
            relativeUrl,
            it => it.Method = HttpMethod.Get,
            async it =>
            {
                var payload = await it.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(payload);
            },
            ct);

        public Task<T> PutAsync<T>(string relativeUrl, Byte[] bytes, CancellationToken ct) => Invoke(
            relativeUrl,
            it =>
            {
                it.Method = HttpMethod.Put;
                it.Content = new ByteArrayContent(bytes);
            },
            async it => JsonConvert.DeserializeObject<T>(await it.Content.ReadAsStringAsync()),
            ct);

        public async Task DeleteAsync(string relativeUrl, CancellationToken ct) => await Invoke(
            relativeUrl,
            it => it.Method = HttpMethod.Delete,
            it => Task.FromResult(0),
            ct);

        public Task<TResponsePayload> PostAsync<TPayload, TResponsePayload>(
            string relativeUrl,
            TPayload payload,
            CancellationToken ct) => Invoke(
            relativeUrl,
            it =>
            {
                it.Method = HttpMethod.Post;
                it.Content = SerializeToJson(payload);
            },
            async it => JsonConvert.DeserializeObject<TResponsePayload>(await it.Content.ReadAsStringAsync()),
            ct);

        private async Task<T> Invoke<T>(
            string relativeUrl,
            Action<HttpRequestMessage> constructMessage,
            Func<HttpResponseMessage, Task<T>> constructResponse,
            CancellationToken ct)
        {
            var uri = new[] {_baseUrl, relativeUrl}
                .Select(it => it.Trim('/'))
                .Join("/")
                .Pipe(it => new Uri(it));
            return await Invoke(uri, constructMessage, constructResponse, ct);
        }
        private async Task<T> Invoke<T>(
            Uri absoluteUri,
            Action<HttpRequestMessage> constructMessage,
            Func<HttpResponseMessage, Task<T>> constructResponse,
            CancellationToken ct)
        {
            var code = await _authenticator.GetAccessTokenAsync();
            var request = new HttpRequestMessage
            {
                RequestUri = absoluteUri,
            };
            request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
            constructMessage(request);
            using (var client = _httpClientFactory.GetClient())
            {
                var response = await client.SendAsync(request, ct);
                return await constructResponse(response);
            }
        }

        private StringContent SerializeToJson<T>(T it)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var jsonPayload = JsonConvert.SerializeObject(it, Formatting.None, settings);
            return new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        }
    }

    public interface IOnedriveAuthenticatedAccess
    {
        Task<T> GetAsync<T>(string relativeUrl, CancellationToken ct);
        Task<Stream> GetContentStreamAsync(string relativeUrl, CancellationToken ct);
        Task<Stream> GetContentStreamAsync(Uri absoluteUri, Tuple<string, string> header, CancellationToken ct);
        Task<TResponsePayload> PostAsync<TPayload, TResponsePayload>(string relativeUrl, TPayload payload, CancellationToken ct);
        Task<T> PutAsync<T>(string relativeUrl, Byte[] bytes, CancellationToken ct);
        Task DeleteAsync(string relativeUrl, CancellationToken ct);
    }
}