using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Ruqqus.Security;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Ruqqus
{

    /// <summary>
    /// Event handler for all <see cref="Ruqqus.Client"/> events.
    /// </summary>
    /// <param name="client">The <see cref="Client"/> instance raising the event.</param>
    /// <param name="args">Arguments to be supplied with the event.</param>
    /// <typeparam name="T">A type derived from <see cref="EventArgs"/>.</typeparam>
    public delegate void ClientEventHandler<in T>(Client client, T args) where T : EventArgs;

    /// <summary>
    /// Base class for event arguments that pertain to posts and/or comments.
    /// </summary>
    public class SubmissionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the unique ID of the submission being voted on, either a post or comment ID.
        /// </summary>
        [CanBeNull]
        public string Id { get; }
        
        /// <summary>
        /// Gets a value indicating if this vote was cast on a <see cref="Post"/>.
        /// </summary>
        public bool IsPost { get; }

        /// <summary>
        /// Gets a value indicating if this vote was cast on a <see cref="Comment"/>.
        /// </summary>
        public bool IsComment => !IsPost;

        /// <summary>
        /// Creates a new instance of the <see cref="SubmissionEventArgs"/> class.
        /// </summary>
        /// <param name="id">The unique ID of the submission being voted on, either a post or comment ID.</param>
        /// <param name="isPost">A value indicating if this vote was cast on a post or comment.></param>
        public SubmissionEventArgs([CanBeNull] string id, bool isPost)
        {
            Id = id;
            IsPost = isPost;
        }
    }
    
    /// <summary>
    /// Arguments supplied with vote submission events.
    /// </summary>
    /// <seealso cref="Client.VoteSubmitted"/>
    public class VoteEventArgs : SubmissionEventArgs
    {
        /// <summary>
        /// Gets the direction of the vote that was cast.
        /// </summary>
        public VoteDirection VoteDirection { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="VoteEventArgs"/> class.
        /// </summary>
        /// <param name="id">The unique ID of the submission being voted on, either a post or comment ID.</param>
        /// <param name="direction">The direction of the vote that was cast.</param>
        /// <param name="isPost">A value indicating if this vote was cast on a post or comment.></param>
        public VoteEventArgs([CanBeNull] string id, VoteDirection direction, bool isPost) : base(id, isPost)
        {
            VoteDirection = direction;
        }
    }
    
    /// <summary>
    /// Arguments that pertain to <see cref="Post"/> events.
    /// </summary>
    public class PostEventArgs : SubmissionEventArgs
    {
        /// <summary>
        /// Gets the post that raised the event.
        /// </summary>
        [CanBeNull]
        public Post Post { get; }
        
        /// <summary>
        /// Creates a new instance of the <see cref="PostEventArgs"/> class.
        /// </summary>
        /// <param name="post">The <see cref="Post"/> instance raising the event.</param>
        public PostEventArgs([CanBeNull] Post post) : base(post?.Id, true)
        {
            Post = post;
        }
    }
    
    /// <summary>
    /// Arguments that pertain to <see cref="Post"/> events.
    /// </summary>
    public class CommentEventArgs : SubmissionEventArgs
    {
        /// <summary>
        /// Gets the post that raised the event.
        /// </summary>
        [CanBeNull]
        public Comment Comment { get; }
        
        /// <summary>
        /// Creates a new instance of the <see cref="PostEventArgs"/> class.
        /// </summary>
        /// <param name="comment">The <see cref="Comment"/> instance raising the event.</param>
        public CommentEventArgs([CanBeNull] Comment comment) : base(comment?.Id, false)
        {
            Comment = comment;
        }
    }

    public partial class Client
    {
        /// <summary>
        /// Occurs when the <see cref="Client"/> is about to be disposed.
        /// </summary>
        public event ClientEventHandler<EventArgs> Disposing;

        /// <summary>
        /// Occurs when the <see cref="Client"/> has been disposed.
        /// </summary>
        public event ClientEventHandler<EventArgs> Disposed;

        /// <summary>
        /// Occurs when a post or comment is voted on.
        /// </summary>
        public event ClientEventHandler<VoteEventArgs> VoteSubmitted;
        
        /// <summary>
        /// Occurs when a post is created with the API.
        /// </summary>
        public event ClientEventHandler<PostEventArgs> PostCreated;

        /// <summary>
        /// Occurs when a comment is created with the API.
        /// </summary>
        public event ClientEventHandler<CommentEventArgs> CommentCreated;
        
        /// <summary>
        /// Invokes the <see cref="VoteSubmitted"/> event.
        /// </summary>
        /// <param name="id">The unique ID of the submission (post or comment)</param>
        /// <param name="direction">The direction of the vote that was cast.</param>
        /// <param name="isPost"><c>true</c> if <paramref name="id"/> refers to a post, otherwise <c>false</c> for a comment.</param>
        protected virtual void OnVoteSubmitted([CanBeNull] string id, VoteDirection direction, bool isPost)
        {
            VoteSubmitted?.Invoke(this, new VoteEventArgs(id, direction, isPost));
        }

        /// <summary>
        /// Invokes the <see cref="PostCreated"/> event.
        /// </summary>
        /// <param name="post">A <see cref="Post"/> instance to supply as the argument.</param>
        protected virtual void OnPostCreated([CanBeNull] Post post)
        {
            PostCreated?.Invoke(this, new PostEventArgs(post));
        }

        /// <summary>
        /// Invokes the <see cref="CommentCreated"/> event.
        /// </summary>
        /// <param name="comment">A <see cref="Comment"/> instance to supply as the argument.</param>
        protected virtual void OnCommentCreated([CanBeNull] Comment comment)
        {
            CommentCreated?.Invoke(this, new CommentEventArgs(comment));
        }
    }
}