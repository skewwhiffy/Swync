using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Swync.Core.Onedrive.Authentication;

namespace Swync.Core.Onedrive.Http
{
    public class OnedriveAuthenticatedAccess : OnedriveAccessBase, IOnedriveAuthenticatedAccess
    {
        private readonly IAuthenticator _authenticator;

        public OnedriveAuthenticatedAccess(
            IAuthenticator authenticator,
            IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _authenticator = authenticator;
        }

        protected override async Task<HttpRequestMessage> ApplyAuthenticationAsync(
            HttpRequestMessage request,
            CancellationToken ct)
        {
            var code = await _authenticator.GetAccessTokenAsync(ct);
            request.Headers.Add("Authorization", $"bearer {code.AccessToken}");
            return request;
        }
    }

    public interface IOnedriveAuthenticatedAccess : IOnedriveAccess
    {
    }
}