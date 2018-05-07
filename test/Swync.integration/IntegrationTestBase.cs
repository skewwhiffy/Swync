﻿using FluentAssertions;
using Newtonsoft.Json;
using Swync.core.Onedrive.Authentication;

namespace Swync.integration
{
    public abstract class IntegrationTestBase
    {
        protected IntegrationTestBase()
        {
            var authenticator = new Authenticator();
            var accessTokenTask = authenticator.GetAccessTokenAsync();
            accessTokenTask.Wait();
        }
    }
}