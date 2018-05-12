using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Swync.core.Functional;
using Swync.core.Onedrive.Authentication;

namespace Swync.core.Onedrive.Http
{
    public class OnedriveAuthenticatedAccess : IOnedriveAuthenticatedAccess
    {
        private readonly IAuthenticator _authenticator;
        private readonly string _baseUrl;

        public OnedriveAuthenticatedAccess(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
            _baseUrl = "https://graph.microsoft.com/v1.0/me/";
        }

        public async Task<T> GetAsync<T>(string relativeUrl)
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
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                var payload = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(payload);
            }
        }
    }

    public interface IOnedriveAuthenticatedAccess
    {
        Task<T> GetAsync<T>(string relativeUrl);
    }
}