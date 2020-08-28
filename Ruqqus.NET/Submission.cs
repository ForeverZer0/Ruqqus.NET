using System;
using System.Runtime.Serialization;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ruqqus.NET
{
    /// <summary>
    /// Abstract base class for <see cref="Post"/> and <see cref="Comment"/> types which defined common functionality.
    /// </summary>
    [DataContract][KnownType(typeof(ItemBase))]
    public abstract class Submission : ItemBase
    {
        /// <summary>
        /// Gets the name of the submission's author, or <c>null</c> if it has been deleted/banned.
        /// </summary>
        [field: DataMember(Name = "author")]
        public string AuthorName { get; }
        
        /// <summary>
        /// Gets the text content of the submission.
        /// </summary>
        [field: DataMember(Name = "body")]
        public string Body { get; }
        
        /// <summary>
        /// Gets the text content of the submission in HTML format.
        /// </summary>
        [field: DataMember(Name = "body_html")]
        public string BodyHtml { get; }

        /// <summary>
        /// Gets the time of the most recent edit performed on this submission.
        /// </summary>
        public DateTime LastEdit => IsEdited ? (DateTime.UnixEpoch + TimeSpan.FromSeconds(lastEdit)).ToUniversalTime() : DateTime.MinValue;

        /// <summary>
        /// Gets a value indicating if submission has been edited.
        /// </summary>
        public bool IsEdited => lastEdit != 0L;

        /// <summary>
        /// Gets the number of upvotes this submission has received.
        /// </summary>
        [field: DataMember(Name = "upvotes")]
        public int UpvoteCount { get; }

        /// <summary>
        /// Gets the number of downvotes this submission has received.
        /// </summary>
        [field: DataMember(Name = "downvotes")]
        public int DownvoteCount { get; }
        
        /// <summary>
        /// Gets the score calculated by adding upvotes and subtracting downvotes.
        /// </summary>
        [field: DataMember(Name = "score")]
        public int Score { get; }
        
        /// <summary>
        /// Gets value indicating if this submission has been flagged as adult content and NSFW.
        /// </summary>
        [field: DataMember(Name = "is_nsfw")]
        public bool IsNsfw { get; }
        
        /// <summary>
        /// Gets value indicating if this submission has been flagged as adult content and NSFL.
        /// </summary>
        [field: DataMember(Name = "is_nsfl")]
        public bool IsNsfl { get; }
        
        /// <summary>
        /// Gets a value indicating if this submission has been archived.
        /// </summary>
        [field: DataMember(Name = "is_archived")]
        public bool IsArchived { get; }
        
        /// <summary>
        /// Gets a value indicating if this submission has been deleted.
        /// </summary>
        [field: DataMember(Name = "is_deleted")]
        public bool IsDeleted { get; }
        
        /// <summary>
        /// Gets a value indicating if this submission has been flagged as being offensive.
        /// </summary>
        [field: DataMember(Name = "is_offensive")]
        public bool IsOffensive { get; }
        
        /// <summary>
        /// Gets the text title of this submission.
        /// </summary>
        [field: DataMember(Name = "title")]
        public string Title { get; }
        
        /// <summary>
        /// Gets the name of the guild this submission is contained within.
        /// </summary>
        [field: DataMember(Name = "guild_name")]
        public string GuildName { get; }
        
        /// <summary>
        /// Gets the full-length ID of this submission.
        /// </summary>
        [field: DataMember(Name = "fullname")]
        public string FullName { get; }
        
        [DataMember(Name = "last_edit_utc")]
        private long lastEdit;
    }
}

