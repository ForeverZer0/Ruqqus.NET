namespace Ruqqus
{
    /// <summary>
    /// Indicates the value of a vote on a comment or post.
    /// </summary>
    public enum VoteDirection
    {
        /// <summary>
        /// Downvote a submission, as everyone on Ruqqus loves to do.
        /// </summary>
        Down = -1,

        /// <summary>
        /// Set a vote to no vote.
        /// </summary>
        None = 0,

        /// <summary>
        /// Upvote a submission.
        /// </summary>
        Up = 1
    }
}