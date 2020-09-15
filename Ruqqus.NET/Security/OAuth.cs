using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Ruqqus.Helpers;

namespace Ruqqus.Security
{

    /// <summary>
    /// A delegate type used for grant/refresh events with OAuth tokens.
    /// </summary>
    /// <param name="token"></param>
    public delegate void TokenEventHandler(Token token);

    /// <summary>
    /// Static class for managing OAuth2 authentication. 
    /// </summary>
    public static class OAuth
    {
        /// <summary>
        /// The endpoint for token grant/refresh method. 
        /// </summary>
        private const string GRANT = "https://ruqqus.com/oauth/grant";
        
        /// <summary>
        /// The endpoint for the user authorization method.
        /// </summary>
        private const string AUTHORIZE = "https://ruqqus.com/oauth/authorize";

        /// <summary>
        /// Occurs when a new token is created and granted user access.
        /// </summary>
        public static event TokenEventHandler TokenGranted;


        /// <summary>
        /// Occurs when an access token is refreshed;
        /// </summary>
        public static event TokenEventHandler TokenRefreshed;

        /// <summary>
        /// Authorizes the application to perform actions as a user, using the authorization code from the redirect URL
        /// when the user made approval.
        /// </summary>
        /// <param name="info">A <see cref="ClientInfo"/> object describing the authorized application.</param>
        /// <param name="code">The authorization code from the redirect URL.</param>
        /// <returns>A newly created access token.</returns>
        /// <remarks>This action can only be performed once per code, and will fail otherwise.</remarks>
        public static Token Grant(ClientInfo info, string code) => GrantAsync(info, code).Result;
        
        /// <summary>
        /// Authorizes the application to perform actions as a user, using the authorization code from the redirect URL
        /// when the user made approval.
        /// </summary>
        /// <param name="info">A <see cref="ClientInfo"/> object describing the authorized application.</param>
        /// <param name="code">The authorization code from the redirect URL.</param>
        /// <returns>A task for creating a new access token.</returns>
        /// <remarks>This action can only be performed once per code, and will fail otherwise.</remarks>
        public static async Task<Token> GrantAsync(ClientInfo info, string code)
        {
            var uri = new Uri(GRANT, UriKind.Absolute);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "code"),
                new KeyValuePair<string, string>("client_id", info.Id),
                new KeyValuePair<string, string>("client_secret", info.Secret),
                new KeyValuePair<string, string>("code", code)
            });
            
            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd(Client.UserAgent);
            var response = await http.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            var token = JsonHelper.Load<Token>(await response.Content.ReadAsStreamAsync());
            TokenGranted?.Invoke(token);
            return token;
        }
        
        /// <summary>
        /// Refreshes a stale access token as needed. 
        /// </summary>
        /// <param name="info">A <see cref="ClientInfo"/> object describing the authorized application.</param>
        /// <param name="token">The token to refresh.</param>
        /// <returns>A value indicating <c>true</c> if token was refreshed, otherwise <c>false</c>.</returns>
        /// <remarks>This is a no-op if the token does not yet require ti ve refreshed.</remarks>
        public static bool Refresh(ClientInfo info, Token token) => RefreshAsync(info, token).Result;
        
        /// <summary>
        /// Refreshes a stale access token as needed. 
        /// </summary>
        /// <param name="info">A <see cref="ClientInfo"/> object describing the authorized application.</param>
        /// <param name="token">The token to refresh.</param>
        /// <returns>A task indicating <c>true</c> if token was refreshed, otherwise <c>false</c>.</returns>
        /// <remarks>This is a no-op if the token does not yet require ti ve refreshed.</remarks>
        public static async Task<bool> RefreshAsync(ClientInfo info, Token token)
        {
            if (token is null || !token.NeedRefresh)
                return false;

            // Can't call PostForm here, would cause a stack overflow (ask me how I know)
            var uri = new Uri(GRANT, UriKind.Absolute);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh"),
                new KeyValuePair<string, string>("client_id", info.Id),
                new KeyValuePair<string, string>("client_secret", info.Secret),
                new KeyValuePair<string, string>("refresh_token", token.RefreshToken),
            });

            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd(Client.UserAgent);
            var response = await http.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
                
            var fresh = JsonHelper.Load<Token>(await response.Content.ReadAsStreamAsync());
            token.Update(fresh);
            
            TokenRefreshed?.Invoke(token);
            return true;
        }
        
        /// <summary>
        /// Generates a Ruqqus URL where the user will grant permission for the application, generating a new CSRF token
        /// to be validated in the response.
        /// </summary>
        /// <param name="info">A <see cref="ClientInfo"/> object describing the authorized application.</param>
        /// <param name="scope">A set of flags indicating the permissions the application requires.</param>
        /// <param name="persist">Flag indicating if this access token will persist and be reused more than once.</param>
        /// <returns>A full URL where a user can authorize the application in a browser.</returns>
        /// <seealso href="https://en.wikipedia.org/wiki/Cross-site_request_forgery">Cross-Site Request Forgery</seealso>
        public static string AuthorizationUrl(ClientInfo info, OAuthScope scope, bool persist)
        {
            var guid = Guid.NewGuid();
            var bytes = guid.ToByteArray();
            var csrf = Convert.ToBase64String(bytes);
            
            return AuthorizationUrl(info, scope, persist, csrf);
        }

        /// <summary>
        /// Generates a Ruqqus URL where the user will grant permission for the application.
        /// </summary>
        /// <param name="info">A <see cref="ClientInfo"/> object describing the authorized application.</param>
        /// <param name="scope">A set of flags indicating the permissions the application requires.</param>
        /// <param name="persist">Flag indicating if this access token will persist and be reused more than once.</param>
        /// <param name="csrf">A unique token to mitigate cross-site request forgery (CSRF) attacks.</param>
        /// <returns>A full URL where a user can authorize the application in a browser.</returns>
        /// <seealso href="https://en.wikipedia.org/wiki/Cross-site_request_forgery">Cross-Site Request Forgery</seealso>
        public static string AuthorizationUrl(ClientInfo info, OAuthScope scope, bool persist, string csrf)
        {
            AssertCredentials(info);
            
            var buffer = new StringBuilder(AUTHORIZE);
            buffer.AppendFormat("?client_id={0}", info.Id);
            buffer.AppendFormat("&redirect_uri={0}", info.RedirectUrl);
            buffer.AppendFormat("&scope={0}", ScopeString(scope));
            buffer.AppendFormat("&state={0}", csrf);
            if (persist)
                buffer.Append("&permanent=1");
            
            return buffer.ToString();
        }
        
        /// <summary>
        /// <para>Helper method to make authorizing desktop users a more convenient, using a localhost redirect URL.</para>
        /// <para>
        /// The default browser window will automatically be opened for the user to confirm permissions on Ruqqus while
        /// the application listens for the response and fetches the confirmation code. This code is then used to grant
        /// access and generate a new <see cref="Token"/>, which is returned.
        /// </para>
        /// <para>This only works with HTTP schemes that redirect to the IPv4 loopback (127.0.0.1 or localhost).</para>
        /// </summary>
        /// <param name="info">A <see cref="ClientInfo"/> object describing the authorized application.</param>
        /// <param name="scope">A set of flags indicating the permissions the application requires.</param>
        /// <param name="persist">Flag indicating if this access token will persist and be reused more than once.</param>
        /// <returns>A newly created access token.</returns>
        /// <exception cref="UriFormatException">Thrown when the redirect URL is not to the the localhost or a HTTP scheme.</exception>
        public static Token UserAuthorize(ClientInfo info, OAuthScope scope = OAuthScope.All, bool persist = true)
        {
            var uri = new Uri(info.RedirectUrl);
            if (uri.Scheme != Uri.UriSchemeHttp || !uri.IsLoopback)
                throw new UriFormatException("Redirect URL must be a localhost address with HTTP scheme.");
            
            var authorizeUrl = AuthorizationUrl(info, scope, persist);
            var listener = new HttpListener();
            
            try
            {
                var redirectUrl = info.RedirectUrl.EndsWith('/') ? info.RedirectUrl : info.RedirectUrl + '/';
                listener.Prefixes.Add(redirectUrl);
                listener.Start();
                OSHelper.OpenInBrowser(authorizeUrl);

                // The GetContext method blocks while waiting for a request.
                var context = listener.GetContext();

                var code = context.Request.QueryString["code"];
                if (string.IsNullOrEmpty(code))
                {
                    SendResponse(context.Response, false);
                    return null;
                }
                
                SendResponse(context.Response, true);
                
                Console.WriteLine(code);

                return Grant(info, code);
            }
            catch (HttpListenerException e)
            {
                Console.Error.Write(e.Message);
            }
            finally
            {
                listener.Close();
            }

            return null;
        }


        public static string GetResponseText(bool success)
        {
            var buffer = new StringBuilder("<html>");
            buffer.Append(Strings.ResponseHead);
            if (success)
                buffer.AppendFormat(Strings.ResponseBody, "#339966", Strings.Confirmed);
            else
                buffer.AppendFormat(Strings.ResponseBody, "#ff0000", Strings.Failed);
            buffer.Append("</html>");
            
            return buffer.ToString();
        }
        
        private static void SendResponse(HttpListenerResponse response, bool success)
        {
            
            // Construct a response.
            var responseString = GetResponseText(success);
            var buffer = Encoding.UTF8.GetBytes(responseString);

            // Write the payload to the response stream and close it.
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        private static void AssertCredentials(ClientInfo info)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));
            
            if (string.IsNullOrEmpty(info.Id) || string.IsNullOrEmpty(info.Secret))
                throw new InvalidCredentialException("Client ID and secret have not been set.");
            
            if (Uri.TryCreate(info.RedirectUrl, UriKind.Absolute, out var test))
            {
                if (test.IsLoopback && test.Scheme != Uri.UriSchemeHttp)
                    throw new FormatException("HTTPS scheme cannot be used for local authentication.");
                if (!test.IsLoopback && test.Scheme != Uri.UriSchemeHttps)
                    throw new FormatException("HTTPS scheme required for non-local connections.");
            }
            else
                throw new FormatException("Well-formed and absolute URL is required.");
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
    }
}