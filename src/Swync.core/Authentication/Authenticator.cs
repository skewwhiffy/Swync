using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using Swync.core.Functional;

namespace Swync.core.Authentication
{
    public class Authenticator
    {
        private const string ClientId = "23aec4fb-a025-46f7-beba-02caeac8b791";
        private static readonly int[] PortsRegisteredWithMicrosoft = {80, 8080, 38080};

        private static readonly string[] Scopes = {
            "files.readwrite",
            "offline_access"
        };
        
        private const string ResponseType = "code";
        
        public async Task<string> GetAuthorizationCodeAsync()
        {
            var port = GetFreePort();
            var listenTask = ListenForAuthorizationCode(port);
            var callback = $"http://localhost:{port}";
            var queryVariables = new Dictionary<string, string>
            {
                {"client_id", ClientId},
                {"scope", Scopes.Join(" ")},
                {"redirect_uri", callback},
                {"response_type", ResponseType}
            };
            var queryVariablesString = queryVariables
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Pipe(HttpUtility.UrlEncode))
                .Select(kvp => $"{kvp.Key}={kvp.Value}")
                .Join("&");
            OpenBrowser($"https://login.microsoftonline.com/common/oauth2/v2.0/authorize?{queryVariablesString}");
            
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
                .Pipe(System.Text.Encoding.UTF8.GetBytes);
            context
                .Response
                .ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
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
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
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