using System;
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
                var deserialized = JsonConvert.DeserializeObject<DrivesContainer>(payload);
                return deserialized.Value;
            }
        }

        public async Task<IEnumerable<OnedriveDirectory>> GetDirectories(OnedriveDrive drive)
        {
            var code = await _authenticator.GetAccessTokenAsync();
            using (var client = new HttpClient())
            {
                var url = $"https://graph.microsoft.com/v1.0/me/drives/{drive.Id}/root/children";
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };
                request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
                var response = await client.SendAsync(request);
                var payload = await response.Content.ReadAsStringAsync();
                throw new NotImplementedException();
            }
        }

        private class DrivesContainer
        {
            [JsonProperty("value")]
            public IEnumerable<OnedriveDrive> Value { get; set; }
        }
    }
}