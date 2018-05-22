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
        
        // TODO: Lots of DRY below, please

        public async Task<Stream> GetContentStreamAsync(string relativeUrl, CancellationToken ct)
        {
            var uri = new[] {_baseUrl, relativeUrl}
                .Select(it => it.Trim('/'))
                .Join("/")
                .Pipe(it => new Uri(it));
            var code = await _authenticator.GetAccessTokenAsync();
            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
            using (var client = _httpClientFactory.GetClient())
            {
                var response = await client.SendAsync(request, ct);
                return await response.Content.ReadAsStreamAsync();
            }
        }
        
        public async Task<T> GetAsync<T>(string relativeUrl, CancellationToken ct)
        {
            var uri = new[] {_baseUrl, relativeUrl}
                .Select(it => it.Trim('/'))
                .Join("/")
                .Pipe(it => new Uri(it));
            var code = await _authenticator.GetAccessTokenAsync();
            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Get
            };
            request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
            using (var client = _httpClientFactory.GetClient())
            {
                var response = await client.SendAsync(request, ct);
                var payload = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(payload);
            }
        }

        public async Task<TResponsePayload> PostAsync<TPayload, TResponsePayload>(string relativeUrl, TPayload payload, CancellationToken ct)
        {
            var uri = new[] {_baseUrl, relativeUrl}
                .Select(it => it.Trim('/'))
                .Join("/")
                .Pipe(it => new Uri(it));
            var code = await _authenticator.GetAccessTokenAsync();
            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = SerializeToJson(payload)
            };
            request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
            using (var client = _httpClientFactory.GetClient())
            {
                var response = await client.SendAsync(request, ct);
                var responsePayload = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponsePayload>(responsePayload);
            }
        }

        public async Task<T> PutAsync<T>(string relativeUrl, Byte[] bytes, CancellationToken ct)
        {
            var uri = new[] {_baseUrl, relativeUrl}
                .Select(it => it.Trim('/'))
                .Join("/")
                .Pipe(it => new Uri(it));
            var code = await _authenticator.GetAccessTokenAsync();
            
            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Put,
                Content = new ByteArrayContent(bytes)
            };
            request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
            using (var client = _httpClientFactory.GetClient())
            {
                var response = await client.SendAsync(request, ct);
                var responsePayload = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responsePayload);
            }
        }
        
        public async Task DeleteAsync(string relativeUrl, CancellationToken ct)
        {
            var uri = new[] {_baseUrl, relativeUrl}
                .Select(it => it.Trim('/'))
                .Join("/")
                .Pipe(it => new Uri(it));
            var code = await _authenticator.GetAccessTokenAsync();
            var request = new HttpRequestMessage
            {
                RequestUri = uri,
                Method = HttpMethod.Delete
            };
            request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
            using (var client = _httpClientFactory.GetClient())
            {
                var response = await client.SendAsync(request, ct);
                await response.Content.ReadAsStringAsync();
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
        Task<TResponsePayload> PostAsync<TPayload, TResponsePayload>(string relativeUrl, TPayload payload, CancellationToken ct);
        Task<T> PutAsync<T>(string relativeUrl, Byte[] bytes, CancellationToken ct);
        Task DeleteAsync(string relativeUrl, CancellationToken ct);
    }
}