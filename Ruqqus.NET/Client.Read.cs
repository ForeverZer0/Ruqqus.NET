using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Ruqqus.NET
{

    public partial class RuqqusClient
    {
        /// <summary>
        /// Asynchronously retrieves the <see cref="User"/> with the specified username.
        /// </summary>
        /// <param name="username">The username of the account to retrieve.</param>
        /// <returns>A <see cref="User"/> instance of <c>null</c> if not found.</returns>
        /// <exception cref="FormatException">Thrown when the <paramref name="username"/> is not well-formatted.</exception>
        [Authorization(AuthorityKind.Required, OAuthScope.Read)]
        public async Task<User> GetUserAsync([NotNull] string username)
        {
            if (!IsValidUsername(username))
                throw new FormatException("Invalid username.");
            return await QueryObjectAsync<User>($"/api/v1/user/{username}", UserSerializer);
        }
        
        /// <summary>
        /// Asynchronously retrieves the <see cref="Comment"/> with the specified ID.
        /// </summary>
        /// <param name="commentId">The ID of the comment to retrieve.</param>
        /// <returns>A <see cref="Comment"/> instance of <c>null</c> if not found.</returns>
        /// <exception cref="FormatException">Thrown when the <paramref name="commentId"/> is not well-formatted.</exception>
        [Authorization(AuthorityKind.Required, OAuthScope.Read)]
        public async Task<Comment> GetCommentAsync([NotNull] string commentId)
        {
            if (!IsValidSubmissionId(commentId))
                throw new FormatException("Invalid comment ID.");
            return await QueryObjectAsync<Comment>($"/api/v1/comment/{commentId}", CommentSerializer);
        }
        
        /// <summary>
        /// Asynchronously retrieves the <see cref="Post"/> with the specified ID.
        /// </summary>
        /// <param name="postId">The ID of the post to retrieve.</param>
        /// <returns>A <see cref="Post"/> instance of <c>null</c> if not found.</returns>
        /// <exception cref="FormatException">Thrown when the <paramref name="postId"/> is not well-formatted.</exception>
        [Authorization(AuthorityKind.Required, OAuthScope.Read)]
        public async Task<Post> GetPostAsync([NotNull] string postId)
        {
            if (!IsValidSubmissionId(postId))
                throw new FormatException("Invalid post ID.");
            return await QueryObjectAsync<Post>($"/api/v1/post/{postId}", PostSerializer);
        }

        /// <summary>
        /// Asynchronously retrieves the <see cref="Guild"/> with the specified name.
        /// </summary>
        /// <param name="guildName">The name of the guild to retrieve.</param>
        /// <returns>A <see cref="Guild"/> instance of <c>null</c> if not found.</returns>
        /// <exception cref="FormatException">Thrown when the <paramref name="guildName"/> is not well-formatted.</exception>
        [Authorization(AuthorityKind.Required, OAuthScope.Read)]
        public async Task<Guild> GetGuildAsync([NotNull] string guildName)
        {
            if (!IsValidGuildName(guildName))
                throw new FormatException("Invalid guild name.");
            return await QueryObjectAsync<Guild>($"/api/v1/guild/{guildName}", GuildSerializer);
        }
        
        /// <summary>
        /// Invokes the REST API to get a JSON object to deserialize.  
        /// </summary>
        /// <param name="endpoint">The relative route where the GET method is invoked.</param>
        /// <param name="serializer">A JSON serializer for the given type.</param>
        /// <typeparam name="T">A type that inherits from <see cref="InfoBase"/>.</typeparam>
        /// <returns>The object instance, or <c>null</c> if not found.</returns>
        private async Task<T> QueryObjectAsync<T>([NotNull] string endpoint, [NotNull] XmlObjectSerializer serializer) where T : InfoBase
        {
            await AssertAuthorizationAsync();
            try
            {
                var uri = new Uri(endpoint, UriKind.Relative);
                var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return JsonHelper.Load<T>(await response.Content.ReadAsStreamAsync());
            }
            catch (HttpRequestException)
            {
                // Invalid value sent
                return default;
            }
        }

        /// <summary>
        /// Enumerates through each existing guild using the specified sorting method.
        /// </summary>
        /// <param name="sorting">The sorting method determining the order in which results are yielded.</param>
        /// <returns>A collection of <see cref="Guild"/> instances.</returns>
        /// <remarks>
        ///     A new query must be performed after every 25 results returned, so brief pauses at these intervals is an
        ///     expected behavior.
        /// </remarks>
        public async IAsyncEnumerable<Guild> GetGuilds(GuildSort sorting = GuildSort.Subs)
        {
            var sort = Enum.GetName(typeof(GuildSort), sorting)?.ToLowerInvariant() ?? "subs";
            PageResults<Guild> guilds;
            var page = 0;
            
            do
            {
                await AssertAuthorizationAsync();
                var uri = new Uri($"/api/v1/guilds?sort={sort}&page={++page}", UriKind.Relative);
                var response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                guilds = JsonHelper.Load<PageResults<Guild>>(await response.Content.ReadAsStreamAsync());

                if (!string.IsNullOrEmpty(guilds.ErrorMessage))
                    break;
                
                foreach (var guild in guilds.Items)
                    yield return guild;
                
            } while (guilds.Items.Count >= ResultsPerPage);
        }
    }
}