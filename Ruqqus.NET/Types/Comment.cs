using System.Runtime.Serialization;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ruqqus.Types
{
    /// <summary>
    /// Represents a single comment to a post or another comment.
    /// </summary>
    [DataContract][KnownType(typeof(Submission))][KnownType(typeof(Title))]
    public class Comment : Submission
    {
        /// <summary>
        /// Gets the level of "nesting" in the comment tree, starting at `1` when in direct reply to the post.
        /// </summary>
        [field: DataMember(Name = "level", IsRequired = true)]
        public int Level { get; }
        
        /// <summary>
        /// Gets the ID of the parent submission this comment is in reply to.
        /// <para>If nesting level is <c>1</c>, it will be the ID of <see cref="Post"/>, otherwise it is the ID of another <see cref="Comment"/>.</para>
        /// </summary>
        [field: DataMember(Name = "parent", IsRequired = true)]
        public string ParentId { get; }
        
        /// <summary>
        /// Gets the ID of the post this comment is contained within.
        /// </summary>
        [field: DataMember(Name = "post", IsRequired = true)]
        public string PostId { get; }
        
        /// <summary>
        /// Gets the text title of this post.
        /// </summary>
        [field: DataMember(Name = "title")]
        public Title Title { get; }

        /// <summary>
        /// Gets a value indicating if the parent to this comment is the post.
        /// </summary>
        public bool IsTopLevel => Level <= 1;

        /// <summary>
        /// Gets value indicating if the parent to this comment is another comment.
        /// </summary>
        public bool IsNested => Level > 1;
    }
}