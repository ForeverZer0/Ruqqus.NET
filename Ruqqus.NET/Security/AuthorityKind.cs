namespace Ruqqus.Security
{
    /// <summary>
    /// Indicates the level of authority required to perform certain functionality.
    /// </summary>
    public enum AuthorityKind
    {
        /// <summary>
        /// None required.
        /// </summary>
        None,

        /// <summary>
        /// Desired by the API, but will not fail if application does not have valid access token.
        /// </summary>
        Desired,

        /// <summary>
        /// Requires a valid access token, and will fail otherwise.
        /// </summary>
        Required
    }
}