using FluentAssertions;
using Swync.core.Authentication;

namespace Swync.integration
{
    public abstract class IntegrationTestBase
    {
        protected IntegrationTestBase()
        {
            var authenticator = new Authenticator();
            var codeTask = authenticator.GetAuthorizationCodeAsync();
            codeTask.Wait();
            var code = codeTask.Result;
            var refreshTokenTask = authenticator.GetRefreshTokenAsync(code);
            refreshTokenTask.Wait();
            true.Should().BeFalse($"Authorization code is ${code}, refresh token is ${refreshTokenTask.Result}");
        }
    }
}