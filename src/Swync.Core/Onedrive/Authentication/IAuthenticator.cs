using System.Threading.Tasks;

namespace Swync.Core.Onedrive.Authentication
{
    public interface IAuthenticator
    {
        Task<RefreshTokenDetails> GetAccessTokenAsync();
    }
}