using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ruqqus.Security
{

    public class Response
    {
        public string Code { get; set; }
        
        public OAuthScope Scope { get; set; }
    }
    
    public static class OAuth
    {

        public static async Task<OAuthToken> GrantAsync(string clientId, string clientSecret, string code)
        {
            return null;
        }

        public static async Task<OAuthToken> RefreshAsync(OAuthToken token)
        {
            return null;
        }
        
        public static OAuthToken Grant(string clientId, string clientSecret, string code) =>
            GrantAsync(clientId, clientSecret, code).Result;

        public static OAuthToken Refresh(OAuthToken token) => RefreshAsync(token).Result;

        public static Uri GenerateGrantUri(string clientId, string redirect, OAuthScope scope, bool persist, out string csrf)
        {
            csrf = GenerateCsrf();
            return GenerateGrantUri(clientId, redirect, scope, persist, csrf);
        }
        
        public static Uri GenerateGrantUri(string clientId, string redirect, OAuthScope scope, bool persist, string csrf)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));
            if (redirect is null)
                throw new ArgumentNullException(nameof(redirect));
            if (string.IsNullOrEmpty(csrf))
                throw new ArgumentNullException(nameof(csrf));
            
            var buffer = new StringBuilder("https://ruqqus.com/oauth/authorize");
            buffer.AppendFormat("?client_id={0}", clientId);
            buffer.AppendFormat("&redirect_uri={0}", redirect);
            buffer.AppendFormat("&scope={0}", ScopeString(scope));
            buffer.AppendFormat("&state={0}", csrf);
            if (persist)
                buffer.Append("&permanent=persist");
            
            return new Uri(buffer.ToString(), UriKind.Absolute);
        }
        
        private static string ScopeString(OAuthScope scope)
        {
            var scopeNames = (from OAuthScope value in Enum.GetValues(typeof(OAuthScope)) 
                where value != OAuthScope.None && value != OAuthScope.All 
                where scope.HasFlag(value) 
                select Enum.GetName(typeof(OAuthScope), value)?.ToLowerInvariant()).ToList();
            
            if (scopeNames.Count < 1)
                throw new ArgumentException("Must set at least one scope.");

            return string.Join(',', scopeNames);
        }

        private static string GenerateCsrf()
        {
            var guid = Guid.NewGuid();
            var bytes = guid.ToByteArray();
            return Convert.ToBase64String(bytes);
        }
        
        
        public static OAuthToken AuthorizeWizard(string clientId, string clientSecret, OAuthScope scope = OAuthScope.All)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("HTTP listener is not supported on host machine.");
                return null;
            }

            const string redirect = "localhost:1776/ruqqus/net/oauth/";
            var uri = GenerateGrantUri(clientId, redirect, scope, true, out var csrf);

            var listener = new HttpListener();
            try
            {
                listener.Prefixes.Add("http://" + redirect);
                listener.Start();
                OpenInBrowser(uri.ToString());
 
                // The GetContext method blocks while waiting for a request.
                var context = listener.GetContext();
                SendResponse(context.Response);
                var response = ParseQueryString(context.Request.QueryString);
                ;
                Console.WriteLine(context.Request.QueryString);
                
                


                var code = response.Code;
                Console.WriteLine(code);
                
                return Grant(clientId, clientSecret, code);
            }
            catch (HttpListenerException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                listener.Stop();
            }

            return null;
        }
        

        private static Response ParseQueryString(NameValueCollection query)
        {
            var scope = OAuthScope.None;
            foreach (var name in query["scope"].Split(','))
            {
                if (Enum.TryParse<OAuthScope>(name, true, out var value))
                    scope |= value;
            }
            return new Response { Code = query["code"], Scope = scope };
        }

        private static void SendResponse(HttpListenerResponse response)
        {
            // Construct a response.
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }

        public static void OpenInBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", url);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", url);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else
            {
                // Let's hope for the best!
                Process.Start(url);
            }
        }
    }
}