namespace Ruqqus
{
    /// <summary>
    /// Describes filters that can be applied to post queries to limit the results.
    /// </summary>
    public enum PostFilter
    {
        /// <summary>
        /// All posts, from now until the beginning of time.
        /// </summary>
        All,

        /// <summary>
        /// Posts from the within the past day.
        /// </summary>
        Day,

        /// <summary>
        /// Posts from the within the past week.
        /// </summary>
        Week,

        /// <summary>
        /// Posts from the within the past month.
        /// </summary>
        Month,

        /// <summary>
        /// Posts from the within the past year.
        /// </summary>
        Year
    }
}