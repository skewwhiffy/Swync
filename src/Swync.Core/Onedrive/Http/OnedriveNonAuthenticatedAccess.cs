using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Swync.Core.Functional;

namespace Swync.Core.Onedrive.Http
{
    public class OnedriveNonAuthenticatedAccess : OnedriveAccessBase, IOnedriveNonAuthenticatedAccess
    {
        public OnedriveNonAuthenticatedAccess(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        protected override Task<HttpRequestMessage> ApplyAuthenticationAsync(
            HttpRequestMessage request,
            CancellationToken ct)
        {
            return request.Pipe(Task.FromResult);
        }
    }

    public interface IOnedriveNonAuthenticatedAccess : IOnedriveAccess
    {
    }
}