using System;
using System.Runtime.Serialization;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ruqqus
{
    /// <summary>
    /// Represents a post submission on Ruqqus.NET.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Submission))]
    public class Post : Submission
    {
        /// <summary>
        /// Gets the <see cref="Title"/> of the submitter, or <c>null</c> if none is defined. 
        /// </summary>
        [field: DataMember(Name = "author_title")]
        public Title AuthorTitle { get; }

        /// <summary>
        /// Gets the number of comments that have been made within this post.
        /// </summary>
        [field: DataMember(Name = "comment_count")]
        public int CommentCount { get; }

        /// <summary>
        /// Gets the name of the guild this post was originally submitted to.
        /// </summary>
        [field: DataMember(Name = "original_guild_name")]
        public string OriginalGuildName { get; }

        /// <summary>
        /// Gets the link the post is sharing, or <c>null</c> if none was supplied.
        /// </summary>
        public Uri Url =>
            string.IsNullOrEmpty(url) ? null : new Uri(url, UriKind.Absolute); // TODO test with links to other posts

        /// <summary>
        /// Gets the link to the post's thumbnail image, or <c>null</c> if it has none.
        /// </summary>
        public Uri ThumbUrl => string.IsNullOrEmpty(url) ? null : new Uri(thumbUrl, UriKind.Absolute);

        /// <summary>
        /// Gets a domain name for the shared link or image host. 
        /// </summary>
        /// <remarks>Returns <c>"text post"</c> for plain text posts without a link.</remarks>
        [field: DataMember(Name = "domain")]
        public string Domain { get; }

        /// <summary>
        /// Gets the text title of this post.
        /// </summary>
        [field: DataMember(Name = "title")]
        public string Title { get; }

        /// <summary>
        /// Gets the embed link for this post.
        /// </summary>
        public Uri EmbedUrl => string.IsNullOrEmpty(embedUrl) ? null : new Uri(embedUrl, UriKind.RelativeOrAbsolute);

        [DataMember(Name = "url")] private string url;

        [DataMember(Name = "thumb_url")] private string thumbUrl;

        [DataMember(Name = "embed_url")] private string embedUrl;
    }
}