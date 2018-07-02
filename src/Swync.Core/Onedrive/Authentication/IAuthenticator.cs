using System.Threading;
using System.Threading.Tasks;

namespace Swync.Core.Onedrive.Authentication
{
    public interface IAuthenticator
    {
        Task<RefreshTokenDetails> GetAccessTokenAsync(CancellationToken ct);
    }
}