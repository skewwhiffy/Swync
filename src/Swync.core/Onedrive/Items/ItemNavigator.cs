using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Swync.core.Functional;
using Swync.core.Onedrive.Authentication;

namespace Swync.core.Onedrive.Items
{
    public static class JsonExtensions
    {
        public static string PrettyJson(this string json) => json
            .Pipe(JsonConvert.DeserializeObject)
            .Pipe(it => JsonConvert.SerializeObject(it, Formatting.Indented));
    }
    public class DirectoryNavigator
    {
        private readonly IAuthenticator _authenticator;

        public DirectoryNavigator(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

        public async Task<IEnumerable<OnedriveDrive>> GetDrivesAsync()
        {
            var code = await _authenticator.GetAccessTokenAsync();
            using (var client = new HttpClient())
            {
                var url = "https://graph.microsoft.com/v1.0/me/drives";
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };
                request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
                var response = await client.SendAsync(request);
                var payload = await response.Content.ReadAsStringAsync();
                var deserialized = JsonConvert.DeserializeObject<Container<OnedriveDrive>>(payload);
                return deserialized.Value;
            }
        }

        public async Task<IEnumerable<OnedriveItem>> GetItems(OnedriveDrive drive)
        {
            var code = await _authenticator.GetAccessTokenAsync();
            using (var client = new HttpClient())
            {
                var url = $"https://graph.microsoft.com/v1.0/me/drives/{drive.id}/root/children";
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };
                request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
                var response = await client.SendAsync(request);
                var payload = await response.Content.ReadAsStringAsync();
                var pretty = payload.PrettyJson();
                return payload
                    .Pipe(JsonConvert.DeserializeObject<Container<OnedriveItem>>)
                    .Value;
            }
        }

        private class Container<T>
        {
            [JsonProperty("value")]
            public IEnumerable<T> Value { get; set; }
            
        }
    }
}