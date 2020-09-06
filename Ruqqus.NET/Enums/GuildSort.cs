namespace Ruqqus.NET
{
    /// <summary>
    /// Determines the sorting order when querying for collections of guilds.
    /// </summary>
    public enum GuildSort
    {
        /// <summary>
        /// Sort by the number of members in the guild.
        /// </summary>
        Subs,
        
        /// <summary>
        /// Sort by the guilds that are gaining in popularity at an increased rate.
        /// </summary>
        Trending,
        
        /// <summary>
        /// Sort by the creation date of the guild, with most recent first.
        /// </summary>
        New
    }
}