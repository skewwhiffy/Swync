using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Swync.Core.Functional;

namespace Swync.Core.Onedrive.Authentication
{
    public class Authenticator : IAuthenticator
    {
        private static readonly SemaphoreSlim FileLock = new SemaphoreSlim(1, 1);
        private const string FileName = ".accessToken";
        private const string ClientId = "21133f26-e5d8-486b-8b27-0801db6496a9";
        private const string ClientSecret = "gcyhkJZK73!$:zqHNBE243}";
        private static readonly int[] PortsRegisteredWithMicrosoft = {80, 8080, 38080};

        private static readonly string[] Scopes = {
            "files.readwrite",
            "offline_access"
        };

        private int? _port;
        private int Port => _port ?? (_port = GetFreePort()).Value;
        private string Callback => $"http://localhost:{Port}";
       
        private const string ResponseType = "code";
        

        public async Task<RefreshTokenDetails> GetAccessTokenAsync()
        {
            var queryVariables = new Dictionary<string, string>
            {
                {"client_id", ClientId},
                {"redirect_uri", Callback},
                {"client_secret", ClientSecret}
            };
            RefreshTokenDetails token;
            try
            {
                await FileLock.WaitAsync();
                using (var reader = new StreamReader(FileName))
                {
                    var content = await reader.ReadToEndAsync();
                    token = RefreshTokenDetails.FromTokenResponse(content);
                    if (token.ExpiryTime > DateTime.UtcNow.AddMinutes(1))
                    {
                        return token;
                    }
                }

                queryVariables["grant_type"] = "refresh_token";
                queryVariables["refresh_token"] = token.RefreshToken;
            }
            catch (FileNotFoundException)
            {
                var authToken = await GetAuthorizationCodeAsync();
                queryVariables["grant_type"] = "authorization_code";
                queryVariables["code"] = authToken;
            }
            finally
            {
                FileLock.Release();
            }

            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(queryVariables);
                var response = await client.PostAsync("https://login.live.com/oauth20_token.srf", content);
                var payload = await response.Content.ReadAsStringAsync();
                token = RefreshTokenDetails.FromTokenResponse(payload);
            }

            try
            {
                await FileLock.WaitAsync();
                using (var writer = new StreamWriter(FileName))
                {
                    await writer.WriteAsync(JsonConvert.SerializeObject(token));
                }
            }
            finally
            {
                FileLock.Release();
            }

            return token;
        }

        private async Task<string> GetAuthorizationCodeAsync()
        {
            var listenTask = ListenForAuthorizationCode(Port);
            var queryVariables = new Dictionary<string, string>
            {
                {"client_id", ClientId},
                {"scope", Scopes.Join(" ")},
                {"redirect_uri", Callback},
                {"response_type", ResponseType}
            };
            var queryVariablesString = queryVariables
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Pipe(HttpUtility.UrlEncode))
                .Select(kvp => $"{kvp.Key}={kvp.Value}")
                .Join("&");
            OpenBrowser($"https://login.live.com/oauth20_authorize.srf?{queryVariablesString}");
            
            return await listenTask;
        }
        
        private int GetFreePort()
        {
            TcpListener tcp = null;
            foreach (var port in PortsRegisteredWithMicrosoft)
            {
                try
                {
                    tcp = new TcpListener(IPAddress.Any, port);
                    tcp.Start();
                    return port;
                }
                catch
                {
                    // Never mind, try the next one
                }
                finally
                {
                    tcp?.Stop();
                }
            }

            throw new NotImplementedException("Need another port");
        }
        
        private async Task<string> ListenForAuthorizationCode(int port)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{port}/");
            listener.Start();
            var context = await listener.GetContextAsync();
            var buffer = "<HTML><BODY><SCRIPT>close()</SCRIPT></BODY></HTML>"
                .Pipe(Encoding.UTF8.GetBytes);
            context
                .Response
                .ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
            listener.Stop();
            return context.Request.QueryString["code"];
        }
        
        private static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet.Corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("sensible-browser", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}