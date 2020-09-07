using System;
using System.Threading.Tasks;
using Ruqqus.Helpers;

namespace Ruqqus
{
    public partial class Client
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
            return await SubmitVote("/api/v1/vote/comment", comment.Id, direction, false);
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
            return await SubmitVote("/api/v1/vote/comment", commentId, direction, false);
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
            return await SubmitVote("/api/v1/vote/post", post.Id, direction, true);
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
            return await SubmitVote("/api/v1/vote/post", postId, direction, true);
        }

        /// <summary>
        /// Places a vote on a submission.
        /// </summary>
        /// <param name="url">The base URL of the POST endpoint for voting.</param>
        /// <param name="id">The ID of the post or comment.</param>
        /// <param name="direction">The type of vote to place.</param>
        /// <param name="isPost"><c>true</c> if this  submission is a post, otherwise <c>false</c> if a comment.</param>
        /// <returns><c>true</c> if vote was submitted successfully, otherwise <c>false</c> if an error occured.</returns>
        private async Task<bool> SubmitVote(string url, string id, VoteDirection direction, bool isPost)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));
            if (!ValidSubmission.IsMatch(id))
                throw new FormatException($"Invalid submission ID \"{id}\".");

            await RefreshTokenAsync();
            var uri = new Uri($"{url}/{id}/{(int) direction}", UriKind.Relative);
            try
            {
                var response = await httpClient.PostAsync(uri, null);
                var result = JsonHelper.Load<GenericResult>(await response.Content.ReadAsStreamAsync());
                if (result.IsError)
                    return false;
                
                OnVoteSubmitted(id, direction, isPost);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}