using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Swync.core.Authentication
{
    public class TokenDetails
    {
        private TokenDetails()
        {
        }

        public static TokenDetails FromTokenResponse(string content)
        {
            var details = JsonConvert.DeserializeObject<TokenDetails>(content);
            details.CreationTime = DateTime.UtcNow;
            return details;
        }
        
        [JsonProperty("access_token")]
        public string AccessToken { get; private set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; private set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; private set; }
        private DateTime CreationTime { get; set; }
        [JsonProperty("expiry_time")]
        public DateTime ExpiryTime => CreationTime.AddSeconds(ExpiresIn);
    }
}