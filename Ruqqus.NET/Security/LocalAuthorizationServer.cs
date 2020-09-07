using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ruqqus.Security
{

    public class Response
    {
        public string Code { get; set; }
        
        public OAuthScope Scope { get; set; }
    }
    
    public class LocalAuthorizationServer
    {
        // TODO
        public static Uri GenerateAuthorizationUri(string clientId, string redirect, OAuthScope scope, bool persist, out string csrf)
        {

            var scopeNames = (from OAuthScope value in Enum.GetValues(typeof(OAuthScope)) 
                where value != OAuthScope.None && value != OAuthScope.All 
                where scope.HasFlag(value) 
                select Enum.GetName(typeof(OAuthScope), value)?.ToLowerInvariant()).ToList();

            if (scopeNames.Count < 1)
                throw new ArgumentException("Must set at least one scope.");
            
            csrf = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var buffer = new StringBuilder("https://ruqqus.com/oauth/authorize");
            buffer.AppendFormat("?client_id={0}", clientId);
            buffer.AppendFormat("&redirect_url={0}", redirect);
            buffer.AppendFormat("&scope={0}", string.Join(',', scopeNames));
            buffer.AppendFormat("&state={0}", csrf);
            if (persist)
                buffer.Append("&permanent=persist");
            
            return new Uri(buffer.ToString(), UriKind.Absolute);
        }
        
        
        // This example requires the System and System.Net namespaces.
        public static Response SimpleListenerExample(string redirectUrl)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("HTTP listener is not supported on host machine.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                Console.WriteLine("A redirect URL is required.");
                return null;
            }
            
            if (!redirectUrl.EndsWith('/'))
                redirectUrl = redirectUrl + '/';
            var listener = new HttpListener();
            
            try
            {
                var uri = new Uri(redirectUrl);
                listener.Prefixes.Add(redirectUrl);

                listener.Start();
                Console.Write($"Listening on port {uri.Port}... ");
                
                // Note: The GetContext method blocks while waiting for a request.
                var context = listener.GetContext();
                Console.WriteLine($"Connection established");
                Console.WriteLine($"User-Agent: {context.Request.UserAgent}");
                var response = ParseQueryString(context.Request.QueryString);
                SendResponse(context.Response);
                return response;
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
    }
}