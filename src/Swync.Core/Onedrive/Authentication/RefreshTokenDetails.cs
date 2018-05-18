using System;
using Newtonsoft.Json;

namespace Swync.core.Onedrive.Authentication
{
    public class RefreshTokenDetails
    {
        private RefreshTokenDetails()
        {
        }
        
        private DateTime? _expiryTime;

        public static RefreshTokenDetails FromTokenResponse(string content)
        {
            var details = JsonConvert.DeserializeObject<RefreshTokenDetails>(content);
            details.CreationTime = DateTime.UtcNow;
            return details;
        }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; private set; }
        
        [JsonProperty("access_token")]
        public string AccessToken { get; private set; }
        
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; private set; }
        
        private DateTime CreationTime { get; set; }
        
        [JsonProperty("expiry_time")]
        public DateTime ExpiryTime
        {
            get => _expiryTime ?? CreationTime.AddSeconds(ExpiresIn);
            set => _expiryTime = value;
        }
    }
}