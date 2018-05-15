﻿using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Swync.core.Functional;
using Swync.core.Onedrive.Authentication;

namespace Swync.core.Onedrive.Http
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
                new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            using (var client = _httpClientFactory.GetClient())
            {
                var response = await client.SendAsync(request, ct);
                var responsePayload = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponsePayload>(responsePayload);
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
        Task<TResponsePayload> PostAsync<TPayload, TResponsePayload>(string relativeUrl, TPayload payload, CancellationToken ct);
    }
}