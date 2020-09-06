namespace Ruqqus
{
    /// <summary>
    /// Determines the sorting order when querying for collections of guilds.
    /// </summary>
    public enum PostSort
    {
        /// <summary>
        /// Sort by the creation date of the post, with most recent first.
        /// </summary>
        New,

        /// <summary>
        /// Sort by the users recent posts who have been popular.
        /// </summary>
        Hot,

        /// <summary>
        /// Sort by the user's highest voted posts.
        /// </summary>
        Top,

        /// <summary>
        /// Sort by the recent posts that have a close ratio of up/down votes.
        /// </summary> 
        Disputed, // TODO

        /// <summary>
        /// Sort by posts that have been received the most activity (comments/votes) from other users.
        /// </summary>
        Activity
    }
}