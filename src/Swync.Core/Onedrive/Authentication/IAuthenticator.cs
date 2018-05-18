using System.Threading.Tasks;

namespace Swync.core.Onedrive.Authentication
{
    public interface IAuthenticator
    {
        Task<RefreshTokenDetails> GetAccessTokenAsync();
    }
}