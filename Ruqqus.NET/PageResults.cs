using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ruqqus
{
    /// <summary>
    /// Represents a "page" of results, where each page contains <see cref="Client.ResultsPerPage"/> results.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="InfoBase"/> that is contained on the pages.</typeparam>
    [DataContract, KnownType(typeof(Guild)), KnownType(typeof(Post)), KnownType(typeof(Comment))]
    internal class PageResults<T> : IEnumerable<T> where T : InfoBase
    {
        /// <summary>
        /// The number of results per page when enumerating over large collections.
        /// </summary>
        public const int ResultsPerPage = 25;

        /// <summary>
        /// Gets a message indicating if there was an error. 
        /// </summary>
        [DataMember(Name = "error")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the collection of data on this page.
        /// </summary>
        [DataMember(Name = "data")]
        public List<T> Items { get; set; }

        /// <summary>
        /// Gets the number of results on this page.
        /// </summary>
        public int Count => Items?.Count ?? 0;

        /// <summary>
        /// Gets a value indicating if there is a full number of results on this page.
        /// </summary>
        public bool IsFullPage => Count >= ResultsPerPage;

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Items).GetEnumerator();
    }
}