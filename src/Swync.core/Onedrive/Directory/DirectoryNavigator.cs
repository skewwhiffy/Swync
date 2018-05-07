using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Swync.core.Onedrive.Authentication;

namespace Swync.core.Onedrive.Directory
{
    public class DirectoryNavigator
    {
        private readonly IAuthenticator _authenticator;

        public DirectoryNavigator(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

        public async Task<IEnumerable<Drive>> GetDrivesAsync()
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
                var deserialized = JsonConvert.DeserializeObject<DrivesContainer>(payload);
                return deserialized.Value;
            }
        }

        private class DrivesContainer
        {
            [JsonProperty("value")]
            public IEnumerable<Drive> Value { get; set; }
        }
    }
}