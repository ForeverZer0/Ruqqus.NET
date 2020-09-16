using System;
using System.Threading.Tasks;
using Ruqqus.Security;

namespace Ruqqus
{
    public partial class Client
    {
        /// <summary>
        /// Deletes a previously created comment.
        /// </summary>
        /// <param name="comment">A <see cref="Comment"/> instance that was created by this user.</param>
        /// <returns><c>true</c> if operation completed successfully, otherwise <c>false</c>.</returns>
        [Scope(OAuthScope.Delete)]
        public async Task<bool> DeleteComment([NotNull] Comment comment)
        {
            if (comment is null)
                throw new ArgumentNullException(nameof(comment));
            return await DeleteComment(comment.Id);
        }

        /// <summary>
        /// Deletes a previously created comment.
        /// </summary>
        /// <param name="commentId">The ID of a <see cref="Comment"/> that was created by this user.</param>
        /// <returns><c>true</c> if operation completed successfully, otherwise <c>false</c>.</returns>
        [Scope(OAuthScope.Delete)]
        public async Task<bool> DeleteComment([NotNull] string commentId)
        {
            if (string.IsNullOrEmpty(commentId))
                throw new ArgumentNullException(nameof(commentId));
            if (!ValidSubmission.IsMatch(commentId))
                throw new FormatException($"Invalid comment ID \"{commentId}\".");

            await RefreshTokenAsync();
            var uri = new Uri($"/api/v1/delete/comment/{commentId}", UriKind.Relative);
            var response = await httpClient.PostAsync(uri, null);
            return response.IsSuccessStatusCode;
        }
        
        /// <summary>
        /// Deletes a previously created post.
        /// </summary>
        /// <param name="post">A <see cref="Post"/> instance that was created by this user.</param>
        /// <returns><c>true</c> if operation completed successfully, otherwise <c>false</c>.</returns>
        [Scope(OAuthScope.Delete)]
        public async Task<bool> DeletePost([NotNull] Post post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));
            return await DeleteComment(post.Id);
        }

        /// <summary>
        /// Deletes a previously created post.
        /// </summary>
        /// <param name="postId">The ID of a <see cref="Post"/> that was created by this user.</param>
        /// <returns><c>true</c> if operation completed successfully, otherwise <c>false</c>.</returns>
        [Scope(OAuthScope.Delete)]
        public async Task<bool> DeletePost([NotNull] string postId)
        {
            if (string.IsNullOrEmpty(postId))
                throw new ArgumentNullException(nameof(postId));
            if (!ValidSubmission.IsMatch(postId))
                throw new FormatException($"Invalid post ID \"{postId}\".");

            await RefreshTokenAsync();
            var uri = new Uri($"/api/v1/delete_post/{postId}", UriKind.Relative);
            var response = await httpClient.PostAsync(uri, null);
            return response.IsSuccessStatusCode;
        }
    }
}