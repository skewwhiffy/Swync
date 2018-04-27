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
            true.Should().BeFalse(code);
        }
    }
}