using System;

namespace Ruqqus.Security
{
    /// <summary>
    /// Arguments used for when OAuth token events.
    /// </summary>
    public class TokenEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the token that was refreshed.
        /// </summary>
        public Token Token { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="TokenEventArgs"/>.
        /// </summary>
        /// <param name="token">The token that has raised the event.</param>
        public TokenEventArgs([NotNull] Token token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
    }
}