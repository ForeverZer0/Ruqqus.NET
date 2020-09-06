using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Ruqqus.NET
{
    public partial class RuqqusClient
    {
        /// <summary>
        /// Submits a vote on a <see cref="Comment"/>.
        /// </summary>
        /// <param name="comment">A <see cref="Comment"/> instance to vote on.</param>
        /// <param name="direction">The type of vote to place.</param>
        /// <returns><c>true</c> if vote was submitted successfully, otherwise <c>false</c> if an error occured.</returns>
        public async Task<bool> VoteComment([NotNull] Comment comment, VoteDirection direction = VoteDirection.Up)
        {
            if (comment is null)
                throw new ArgumentNullException(nameof(comment));
            return await SubmitVote("/api/v1/vote/comment", comment.Id, direction);
        }
     
        /// <summary>
        /// Submits a vote on a <see cref="Comment"/>.
        /// </summary>
        /// <param name="commentId">The ID of the comment to vote on.</param>
        /// <param name="direction">The type of vote to place.</param>
        /// <returns><c>true</c> if vote was submitted successfully, otherwise <c>false</c> if an error occured.</returns>
        public async Task<bool> VoteComment([NotNull] string commentId, VoteDirection direction = VoteDirection.Up)
        {
            if (string.IsNullOrEmpty(commentId))
                throw new ArgumentNullException(nameof(commentId));
            return await SubmitVote("/api/v1/vote/comment", commentId, direction);
        }
        
        /// <summary>
        /// Submits a vote on a <see cref="Post"/>.
        /// </summary>
        /// <param name="post">A <see cref="Post"/> instance to vote on.</param>
        /// <param name="direction">The type of vote to place.</param>
        /// <returns><c>true</c> if vote was submitted successfully, otherwise <c>false</c> if an error occured.</returns>
        public async Task<bool> VotePost([NotNull] Post post, VoteDirection direction = VoteDirection.Up)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));
            return await SubmitVote("/api/v1/vote/post", post.Id, direction);
        }
     
        /// <summary>
        /// Submits a vote on a <see cref="Post"/>.
        /// </summary>
        /// <param name="postId">The ID of the post to vote on.</param>
        /// <param name="direction">The type of vote to place.</param>
        /// <returns><c>true</c> if vote was submitted successfully, otherwise <c>false</c> if an error occured.</returns>
        public async Task<bool> VotePost([NotNull] string postId, VoteDirection direction = VoteDirection.Up)
        {
            if (string.IsNullOrEmpty(postId))
                throw new ArgumentNullException(nameof(postId));
            return await SubmitVote("/api/v1/vote/post", postId, direction);
        }
        
        /// <summary>
        /// Places a vote on a submission.
        /// </summary>
        /// <param name="url">The base URL of the POST endpoint for voting.</param>
        /// <param name="id">The ID of the post or comment.</param>
        /// <param name="direction">The type of vote to place.</param>
        /// <returns><c>true</c> if vote was submitted successfully, otherwise <c>false</c> if an error occured.</returns>
        private async Task<bool> SubmitVote(string url, string id, VoteDirection direction)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            if (!ValidSubmission.IsMatch(id))
                throw new FormatException($"Invalid submission ID \"{id}\".");
            
            await AssertAuthorizationAsync();
            var uri = new Uri($"{url}/{id}/{(int) direction}", UriKind.Relative);
            try
            {
                var response = await httpClient.PostAsync(uri, null);
                var result = JsonHelper.Load<GenericResult>(await response.Content.ReadAsStreamAsync());
                return !result.IsError;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}