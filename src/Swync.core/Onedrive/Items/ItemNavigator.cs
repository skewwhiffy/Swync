using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Swync.core.Functional;
using Swync.core.Onedrive.Authentication;

namespace Swync.core.Onedrive.Items
{
    public class DirectoryNavigator
    {
        private readonly IAuthenticator _authenticator;

        public DirectoryNavigator(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

        public async Task<OnedriveContainer<OnedriveDrive>> GetDrivesAsync()
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
                return JsonConvert.DeserializeObject<OnedriveContainer<OnedriveDrive>>(payload);
            }
        }

        public async Task<OnedriveContainer<OnedriveItem>> GetItems(OnedriveDrive drive)
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
                return payload
                    .Pipe(JsonConvert.DeserializeObject<OnedriveContainer<OnedriveItem>>);
            }
        }
    }
}