using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Ruqqus.Security;

namespace Ruqqus
{

    public partial class Client
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
                
                foreach (var guild in guilds)
                    yield return guild;
                
            } while (guilds.Items.Count >= ResultsPerPage);
        }

        /// <summary>
        /// Returns an enumerator for asynchronously iterating over posts with a <see cref="Guild"/>.
        /// </summary>
        /// <param name="guild">The guild instance whose posts will be retrieved.</param>
        /// <param name="filter">Determines the filter used for which results are returned.</param>
        /// <param name="sort">Determines the order in which results are returned.</param>
        /// <returns>A collection of <see cref="Post"/> instances.</returns>
        /// <remarks>
        ///     A new query must be performed after every 25 results returned, so brief pauses at these intervals is an
        ///     expected behavior.
        /// </remarks>
        public async IAsyncEnumerable<Post> GetGuildPosts([NotNull] Guild guild, PostFilter filter = PostFilter.All, PostSort sort = PostSort.New)
        {
            if (guild is null)
                throw new ArgumentNullException(nameof(guild));
            
            await foreach (var post in GetPosts($"/api/v1/guild/{guild.Name}/listing", filter, sort))
                yield return post;
        }
        
        /// <summary>
        /// Returns an enumerator for asynchronously iterating over posts with a <see cref="Guild"/>.
        /// </summary>
        /// <param name="guildName">The name of the guild whose posts will be retrieved.</param>
        /// <param name="filter">Determines the filter used for which results are returned.</param>
        /// <param name="sort">Determines the order in which results are returned.</param>
        /// <returns>A collection of <see cref="Post"/> instances.</returns>
        /// <remarks>
        ///     A new query must be performed after every 25 results returned, so brief pauses at these intervals is an
        ///     expected behavior.
        /// </remarks>
        public async IAsyncEnumerable<Post> GetGuildPosts([NotNull] string guildName, PostFilter filter = PostFilter.All, PostSort sort = PostSort.New)
        {
            if (string.IsNullOrEmpty(guildName))
                throw new ArgumentNullException(nameof(guildName));
            if (!ValidGuildName.IsMatch(guildName))
                throw new FormatException($"Invalid guild name \"{guildName}\".");
            
            await foreach (var post in GetPosts($"/api/v1/guild/{guildName}/listing", filter, sort))
                yield return post;
        }

        /// <summary>
        /// Returns an enumerator for asynchronously iterating over posts the specified <see cref="User"/>.
        /// </summary>
        /// <param name="user">The user whose posts will be retrieved.</param>
        /// <returns>A collection of <see cref="Post"/> instances.</returns>
        /// <remarks>
        ///     A new query must be performed after every 25 results returned, so brief pauses at these intervals is an
        ///     expected behavior.
        /// </remarks>
        public async IAsyncEnumerable<Post> GetUserPosts([NotNull] User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            await foreach (var post in GetPosts($"/api/v1/user/{user.Username}/listing", PostFilter.All, PostSort.New))
                yield return post;
        }
        
        /// <summary>
        /// Returns an enumerator for asynchronously iterating over posts the specified <see cref="User"/>.
        /// </summary>
        /// <param name="username">The name of the user whose posts will be retrieved.</param>
        /// <returns>A collection of <see cref="Post"/> instances.</returns>
        /// <remarks>
        ///     A new query must be performed after every 25 results returned, so brief pauses at these intervals is an
        ///     expected behavior.
        /// </remarks>
        public async IAsyncEnumerable<Post> GetUserPosts([NotNull] string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));
            if (!ValidUsername.IsMatch(username))
                throw new FormatException($"Invalid username \"{username}\".");

            await foreach (var post in GetPosts($"/api/v1/user/{username}/listing", PostFilter.All, PostSort.New))
                yield return post;
        }

        /// <summary>
        /// Returns an enumerator for asynchronously iterating over every post on Ruqqus.
        /// </summary>
        /// <param name="filter">Determines the filter used for which results are returned.</param>
        /// <param name="sort">Determines the order in which results are returned.</param>
        /// <returns>A collection of <see cref="Post"/> instances.</returns>
        /// <remarks>
        ///     A new query must be performed after every 25 results returned, so brief pauses at these intervals is an
        ///     expected behavior.
        /// </remarks>
        public async IAsyncEnumerable<Post> GetAllPosts(PostFilter filter = PostFilter.All, PostSort sort = PostSort.New)
        {
            await foreach (var post in GetPosts("/all/listing", filter, sort))
                yield return post;
        }

        /// <summary>
        /// Returns an enumerator for asynchronously iterating through the "home" page of Ruqqus..
        /// </summary>
        /// <returns>A collection of <see cref="Post"/> instances.</returns>
        /// <remarks>
        ///     A new query must be performed after every 25 results returned, so brief pauses at these intervals is an
        ///     expected behavior.
        /// </remarks>
        public async IAsyncEnumerable<Post> GetFrontPage()
        {
            PageResults<Post> posts;
            var page = 0;
            do
            {
                await AssertAuthorizationAsync();
                var uri = new Uri($"/api/v1/front/listing?{++page}", UriKind.Relative);
                var response = await httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                    break;
                
                posts = JsonHelper.Load<PageResults<Post>>(await response.Content.ReadAsStreamAsync());
                if (!string.IsNullOrEmpty(posts.ErrorMessage))
                    break;
                
                foreach (var post in posts)
                    yield return post;

            } while (posts.Items.Count >= ResultsPerPage);
        }

        /// <summary>
        /// Returns an enumerator for asynchronously iterating over comments of the specified <see cref="Guild"/>.
        /// </summary>
        /// <param name="guild">The guild whose comments will be retrieved.</param>
        /// <returns>A collection of <see cref="Comment"/> instances.</returns>
        public async IAsyncEnumerable<Comment> GetComments([NotNull] Guild guild)
        {
            if (guild is null)
                throw new ArgumentNullException(nameof(guild));
            await foreach (var comment in GetComments(guild.Name))
                yield return comment;
        }
        
        /// <summary>
        /// Returns an enumerator for asynchronously iterating over comments of the specified <see cref="Guild"/>.
        /// </summary>
        /// <param name="guildName">The name of the guild whose comments will be retrieved.</param>
        /// <returns>A collection of <see cref="Comment"/> instances.</returns>
        public async IAsyncEnumerable<Comment> GetComments([NotNull] string guildName)
        {
            PageResults<Comment> comments;
            var page = 0;
            do
            {
                var uri = new Uri($"/api/v1/guild/{guildName}/comments?page={++page}", UriKind.Relative);
                var response = await httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                    break;
                
                comments = JsonHelper.Load<PageResults<Comment>>(await response.Content.ReadAsStreamAsync());
                if (!string.IsNullOrEmpty(comments.ErrorMessage))
                    break;
                
                foreach (var post in comments)
                    yield return post;
            } while (comments.Items.Count >= 25);
        }

        /// <summary>
        /// Returns an enumerator for asynchronously iterating over comments of the specified <see cref="Post"/>.
        /// </summary>
        /// <param name="post">The post whose comments will be retrieved.</param>
        /// <returns>A collection of <see cref="Comment"/> instances.</returns>
        /// <remarks>
        ///     This is an expensive operation! There is no backend method to query the database directly, so this
        ///     method must iterate through all comments in a guild and return only those belonging to a single post.
        /// </remarks>
        public async IAsyncEnumerable<Comment> GetPostComments([NotNull] Post post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));
            
            await foreach (var comment in GetComments(post.GuildName))
            {
                if (comment.PostId == post.Id)
                    yield return comment;
            }
        }
        
        /// <summary>
        /// Returns an enumerator for asynchronously iterating over comments of the specified <see cref="Post"/>.
        /// </summary>
        /// <param name="postId">The ID of the post whose comments will be retrieved.</param>
        /// <returns>A collection of <see cref="Comment"/> instances.</returns>
        /// <remarks>
        ///     This is an expensive operation! There is no backend method to query the database directly, so this
        ///     method must iterate through all comments in a guild and return only those belonging to a single post.
        /// </remarks>
        public async IAsyncEnumerable<Comment> GetPostComments([NotNull] string postId)
        {
            var post = await GetPostAsync(postId);
            if (post is null)
                yield break;
            await foreach (var comment in GetPostComments(post))
                yield return comment;
        }
        
        /// <summary>
        /// Returns an enumerator for asynchronously iterating over posts the specified base <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The base URL of a GET endpoint that retrieves post results.</param>
        /// <param name="filter">Determines the filter used for which results are returned.</param>
        /// <param name="sort">Determines the order in which results are returned.</param>
        /// <returns>A collection of <see cref="Post"/> instances.</returns>
        private async IAsyncEnumerable<Post> GetPosts(string url, PostFilter filter, PostSort sort)
        {
            var s = Enum.GetName(typeof(PostSort), sort)?.ToLowerInvariant() ?? "all";
            var f = Enum.GetName(typeof(PostFilter), filter)?.ToLowerInvariant() ?? "new";
            PageResults<Post> posts;
            var page = 0;
            
            do
            {
                await AssertAuthorizationAsync();
                var uri = new Uri($"{url}?sort={s}&filter={f}&page={++page}", UriKind.Relative);
                var response = await httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                    break;
                
                posts = JsonHelper.Load<PageResults<Post>>(await response.Content.ReadAsStreamAsync());
                if (!string.IsNullOrEmpty(posts.ErrorMessage))
                    break;
                
                foreach (var post in posts)
                    yield return post;
                
            } while (posts.Items.Count >= 25);
        }
    }
}