using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Swync.core.Credentials;
using Swync.core.Functional;

namespace Swync.integration.Config
{
    public class AppSettings
    {
        public AppSettings()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            OnedriveCredentials = GetCredentials(appSettings);
        }

        public Maybe<OnedriveCredentials> OnedriveCredentials { get; }

        private Maybe<OnedriveCredentials> GetCredentials(IConfigurationRoot config)
        {
            var username = config["onedrive:username"];
            var password = config["onedrive:password"];
            if (username.IsNullOrWhiteSpace() != password.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException();
            }

            return new OnedriveCredentials(username, password);
        }
    }
}