using System;
using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ruqqus.NET
{
    /// <summary>
    /// Arguments used for when an access token is refreshed.
    /// </summary>
    public class TokenRefreshedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="Client"/> who refreshed the token.
        /// </summary>
        public RuqqusClient Client { get; }
        
        /// <summary>
        /// Gets the token that was refreshed.
        /// </summary>
        public OAuthToken Token { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="TokenRefreshedEventArgs"/>.
        /// </summary>
        /// <param name="client">The <see cref="Client"/> who refreshed the token.</param>
        /// <param name="token">The token that was refreshed.</param>
        public TokenRefreshedEventArgs([NotNull] RuqqusClient client, [NotNull] OAuthToken token)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
    }
}