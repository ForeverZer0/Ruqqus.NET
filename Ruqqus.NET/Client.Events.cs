using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Ruqqus.Security;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Ruqqus
{

    public class FrontPageWatcher
    {
        
        
    }
    


    
    public partial class Client
    {
        
        public event EventHandler PostCreated;

        public event EventHandler CommentCreated;

        public event EventHandler VoteSubmitted;

        public Uri GenerateAuthorizationUri(string redirect, OAuthScope scope, bool persist, out string csrf)
        {

            var scopeNames = (from OAuthScope value in Enum.GetValues(typeof(OAuthScope)) 
                where value != OAuthScope.None && value != OAuthScope.All 
                where scope.HasFlag(value) 
                select Enum.GetName(typeof(OAuthScope), value)?.ToLowerInvariant()).ToList();

            if (scopeNames.Count < 1)
                throw new ArgumentException("Must set at least one scope.");
            
            csrf = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var buffer = new StringBuilder("https://ruqqus.com/oauth/authorize");
            buffer.AppendFormat("?client_id={0}", ClientId);
            buffer.AppendFormat("&redirect_url={0}", redirect);
            buffer.AppendFormat("&scope={0}", string.Join(',', scopeNames));
            buffer.AppendFormat("&state={0}", csrf);
            if (persist)
                buffer.Append("&permanent=persist");
            
            return new Uri(buffer.ToString(), UriKind.Absolute);
        }
        
        /// <summary>
        /// Occurs when the client's current access token is refreshed.
        /// </summary>
        public event EventHandler<TokenRefreshedEventArgs> TokenRefreshed;
        
        /// <summary>
        /// Updates the client's authorization header and invokes the <see cref="TokenRefreshed"/> event.
        /// </summary>
        protected virtual void OnTokenRefreshed()
        {
            var header = new AuthenticationHeaderValue(token.Type, token.AccessToken);
            httpClient.DefaultRequestHeaders.Authorization = header;
            TokenRefreshed?.Invoke(this, new TokenRefreshedEventArgs(this, Token));
        }
        
        
        
    }
}