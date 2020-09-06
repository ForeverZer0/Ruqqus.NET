using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ruqqus.NET
{
    /// <summary>
    /// Represents a "page" of results, where each page contains <see cref="RuqqusClient.ResultsPerPage"/> results.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="InfoBase"/> that is contained on the pages.</typeparam>
    [DataContract, KnownType(typeof(Guild))]
    internal class PageResults<T> : IEnumerable<T> where T :InfoBase
    {
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

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Items).GetEnumerator();
    }
}