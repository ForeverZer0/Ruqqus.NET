using System;

namespace Ruqqus.Security
{
    /// <summary>
    /// Flags describing various degrees of functionality the application has been granted to perform.
    /// </summary>
    [Flags]
    public enum OAuthScope
    {
        /// <summary>
        /// No scope defined.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Can see username of the accessing account.
        /// </summary>
        Identity = 0x01,

        /// <summary>
        /// Create comments and posts as a user.
        /// </summary>
        Create = 0x02 | Identity,

        /// <summary>
        /// View Ruqqus as a user, including private guilds and restricted content.
        /// </summary>
        Read = 0x04,

        /// <summary>
        /// Edit posts and comments.
        /// </summary>
        Update = 0x08 | Identity,

        /// <summary>
        /// Delete posts and comments.
        /// </summary>
        Delete = 0x10,

        /// <summary>
        /// Place votes on posts and comments.
        /// </summary>
        Vote = 0x20,

        /// <summary>
        /// Perform guildmaster operations.
        /// </summary>
        Guildmaster = 0x40 | Identity,

        /// <summary>
        /// All features enabled.
        /// </summary>
        All = Identity | Create | Read | Update | Delete | Vote | Guildmaster
    }
}