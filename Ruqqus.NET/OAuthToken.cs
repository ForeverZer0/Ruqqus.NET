using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Ruqqus.NET
{

    /// <summary>
    /// Represents an OAuth2 token for the Ruqqus.
    /// </summary>
    [DataContract]
    public class OAuthToken
    {
        /// <summary>
        /// The threshold (in seconds) of remaining time for the access token before it will refresh.
        /// </summary>
        public const long RefreshMargin = 15L;
        
        /// <summary>
        /// Gets or sets the token used to refresh stale access tokens for persistent access.
        /// </summary>
        [DataMember(Name = "refresh_token", IsRequired = false, EmitDefaultValue = false), CanBeNull]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the token the server uses for authentication.
        /// </summary>
        [DataMember(Name = "access_token", IsRequired = true), NotNull]
        // ReSharper disable once NotNullMemberIsNotInitialized
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the type to indicate in the HTTP "Authorization" header.
        /// </summary>
        [DataMember(Name = "token_type"), CanBeNull]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the time the access token expires, measured in seconds since the UNIX epoch.
        /// </summary>
        public DateTime ExpirationTime
        {
            get => DateTime.UnixEpoch + TimeSpan.FromSeconds(expiresUtc);
            set => expiresUtc = Convert.ToInt64((DateTime.UtcNow - value.ToUniversalTime()).TotalSeconds);
        }

        /// <summary>
        /// Gets a value indicating if the access token has expired.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= ExpirationTime;

        /// <summary>
        /// Gets the number of seconds remaining until the access token is invalid and will need refreshed.
        /// </summary>
        public long RemainingSeconds
        {
            get
            {
                var sinceEpoch = (long) (DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
                var diff = expiresUtc - sinceEpoch;
                return diff < 0 ? 0 : diff;
            }
        }
        
        /// <summary>
        /// Gets the scope of the features the application has been authorized to perform.
        /// </summary>
        public OAuthScope Scope
        {
            get
            {
                if (scope == OAuthScope.None && !string.IsNullOrEmpty(scopeList))
                {
                    foreach (var name in scopeList.Split(','))
                    {
                        if (Enum.TryParse<OAuthScope>(name, true, out var value))
                            scope |= value;
                    }
                }
                return scope;
            }
        }

        /// <summary>
        /// Updates this instance with the <see cref="AccessToken"/> and <see cref="ExpirationTime"/> values of
        /// <paramref name="token"/>.
        /// </summary>
        /// <param name="token">A refreshed token containing the updated values.</param>
        public void Update([NotNull] OAuthToken token)
        {
            AccessToken = token.AccessToken;
            expiresUtc = token.expiresUtc;
        }
        
        [DataMember(Name = "scopes", IsRequired = false, EmitDefaultValue = false)]
        private string scopeList;
        
        [DataMember(Name = "expires_at", IsRequired = true)]
        private long expiresUtc;

        private OAuthScope scope;
    }
}