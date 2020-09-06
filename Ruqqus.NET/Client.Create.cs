using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Ruqqus.NET
{
    public partial class RuqqusClient
    {
        /// <summary>
        /// Creates and posts a new comment in reply to the specified <paramref name="parent"/> comment.
        /// </summary>
        /// <param name="parent">The parent comment to reply to.</param>
        /// <param name="text">The text body of the comment (Markdown supported).</param>
        /// <returns>The newly created <see cref="Comment"/> that was posted.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/> or <paramref name="text"/> is <c>null</c>.</exception>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Comment> ReplyToComment([NotNull] Comment parent, [NotNull] string text)
        {
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));
            return await SubmitComment(parent.FullName, parent.PostId, text);
        }
        
        /// <summary>
        /// Creates and posts a new comment in reply to the specified comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment to reply to.</param>
        /// <param name="text">The text body of the comment (Markdown supported).</param>
        /// <returns>The newly created <see cref="Comment"/> that was posted.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="commentId"/> is invalid.</exception>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Comment> ReplyToComment([NotNull] string commentId, [NotNull] string text)
        {
            var parent = await GetCommentAsync(commentId);
            if (parent is null)
                throw new ArgumentException($"Cannot find comment with ID \"{commentId}\".", nameof(commentId));
            return await SubmitComment(parent.FullName, parent.PostId, text);
        }

        /// <summary>
        /// Creates and posts a new comment in reply to the specified <paramref name="parent"/> post.
        /// </summary>
        /// <param name="parent">The parent post to reply to.</param>
        /// <param name="text">The text body of the comment (Markdown supported).</param>
        /// <returns>The newly created <see cref="Comment"/> that was posted.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/> or <paramref name="text"/> is <c>null</c>.</exception>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Comment> ReplyToPost([NotNull] Post parent, [NotNull] string text)
        {
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));
            return await SubmitComment(parent.FullName, parent.Id, text);
        }

        /// <summary>
        /// Creates and posts a new comment in reply to the specified post.
        /// </summary>
        /// <param name="postId">The ID of the post to reply to.</param>
        /// <param name="text">The text body of the comment (Markdown supported).</param>
        /// <returns>The newly created <see cref="Comment"/> that was posted.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="postId"/> is invalid.</exception>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Comment> ReplyToPost([NotNull] string postId, [NotNull] string text)
        {
            var parent = await GetPostAsync(postId);
            if (parent is null)
                throw new ArgumentException($"Cannot find post with ID \"{postId}\".", nameof(postId));
            return await SubmitComment(parent.FullName, parent.Id, text);
        }

        /// <summary>
        /// Creates a standard text post.
        /// </summary>
        /// <param name="guild">The <see cref="Guild"/> to create the post within.</param>
        /// <param name="title">The title of the post.</param>
        /// <param name="text">The text body of the post/</param>
        /// <returns>The newly created <see cref="Post"/> that was just created on Ruqqus.</returns>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Post> CreatePost([NotNull] Guild guild, [NotNull] string title, [NotNull] string text)
        {
            if (guild is null)
                throw new ArgumentNullException(nameof(guild));
            return await CreatePost(guild.Name, title, text, null, null);
        }
        
        /// <summary>
        /// Creates a standard text post.
        /// </summary>
        /// <param name="guildName">The name of the guild to post in.</param>
        /// <param name="title">The title of the post.</param>
        /// <param name="text">The text body of the post/</param>
        /// <returns>The newly created <see cref="Post"/> that was just created on Ruqqus.</returns>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Post> CreatePost([NotNull] string guildName, [NotNull] string title, [NotNull] string text)
        {
            return await CreatePost(guildName, title, text, null, null);
        }

        /// <summary>
        /// Creates a post with an uploaded image, and optional text.
        /// </summary>
        /// <param name="guild">The <see cref="Guild"/> to create the post within.</param>
        /// <param name="title">The title of the post.</param>
        /// <param name="imagePath">The path to an image file.</param>
        /// <param name="text">The text body of the post/</param>
        /// <returns>The newly created <see cref="Post"/> that was just created on Ruqqus.</returns>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Post> CreateImagePost([NotNull] Guild guild, [NotNull]  string title, [NotNull] string imagePath, [CanBeNull] string text = null)
        {
            if (guild is null)
                throw new ArgumentNullException(nameof(guild));
            return await CreatePost(guild.Name, title, text, null, imagePath);
        }
        
        /// <summary>
        /// Creates a post with an uploaded image, and optional text.
        /// </summary>
        /// <param name="guildName">The name of the guild to post in.</param>
        /// <param name="title">The title of the post.</param>
        /// <param name="imagePath">The path to an image file.</param>
        /// <param name="text">The text body of the post/</param>
        /// <returns>The newly created <see cref="Post"/> that was just created on Ruqqus.</returns>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Post> CreateImagePost([NotNull] string guildName, [NotNull]  string title, [NotNull] string imagePath, [CanBeNull] string text = null)
        {
            return await CreatePost(guildName, title, text, null, imagePath);
        }

        /// <summary>
        /// Creates a post with a shared link, and optional text.
        /// </summary>
        /// <param name="guild">The <see cref="Guild"/> to create the post within.</param>
        /// <param name="title">The title of the post.</param>
        /// <param name="url">The a URL to share.</param>
        /// <param name="text">The text body of the post/</param>
        /// <returns>The newly created <see cref="Post"/> that was just created on Ruqqus.</returns>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Post> CreateUrlPost([NotNull] Guild guild, [NotNull] string title, [NotNull] string url, [CanBeNull] string text = null)
        {
            if (guild is null)
                throw new ArgumentNullException(nameof(guild));
            return await CreatePost(guild.Name, title, text, url, null);
        }
        
        /// <summary>
        /// Creates a post with a shared link, and optional text.
        /// </summary>
        /// <param name="guildName">The name of the guild to post in.</param>
        /// <param name="title">The title of the post.</param>
        /// <param name="url">The a URL to share.</param>
        /// <param name="text">The text body of the post/</param>
        /// <returns>The newly created <see cref="Post"/> that was just created on Ruqqus.</returns>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Post> CreateUrlPost([NotNull] string guildName, [NotNull] string title, [NotNull] string url, [CanBeNull] string text = null)
        {
            return await CreatePost(guildName, title, text, url, null);
        }
        
        /// <summary>
        /// Creates a post with a shared link, and optional text.
        /// </summary>
        /// <param name="guild">The <see cref="Guild"/> to create the post within.</param>
        /// <param name="title">The title of the post.</param>
        /// <param name="url">The a URL to share.</param>
        /// <param name="text">The text body of the post/</param>
        /// <returns>The newly created <see cref="Post"/> that was just created on Ruqqus.</returns>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Post> CreateUrlPost([NotNull] Guild guild, [NotNull] string title, [NotNull] Uri url, [CanBeNull] string text = null)
        {
            if (guild is null)
                throw new ArgumentNullException(nameof(guild));
            return await CreatePost(guild.Name, title, text, url.ToString(), null);
        }
        
        /// <summary>
        /// Creates a post with a shared link, and optional text.
        /// </summary>
        /// <param name="guildName">The name of the guild to post in.</param>
        /// <param name="title">The title of the post.</param>
        /// <param name="url">The a URL to share.</param>
        /// <param name="text">The text body of the post/</param>
        /// <returns>The newly created <see cref="Post"/> that was just created on Ruqqus.</returns>
        [Authority(AuthorityKind.Required, OAuthScope.Create)]
        public async Task<Post> CreateUrlPost([NotNull] string guildName, [NotNull] string title, [NotNull] Uri url, [CanBeNull] string text = null)
        {
            return await CreatePost(guildName, title, text, url.ToString(), null);
        }
        
        private async Task<Post> CreatePost([NotNull] string guildName, string title, string text, string url, string imagePath)
        {
            await AssertAuthorizationAsync();
            
            if (string.IsNullOrEmpty(guildName))
                throw new ArgumentNullException(nameof(guildName));
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));

            if (!ValidGuildName.IsMatch(guildName))
                throw new FormatException($"Invalid guild name \"{guildName}\".");
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null, empty, or only whitespace.", nameof(title));
            
            if (url is null && imagePath is null && string.IsNullOrEmpty(text))
                throw new ArgumentException("Text body cannot be null or empty without specifying an image or URL.");
            
            if (url != null && !Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new UriFormatException($"Invalid URL \"{url}\"");
            
            
            var uri = new Uri("/api/v1/submit", UriKind.Relative);
            
            if (string.IsNullOrEmpty(imagePath))
            {
                return await PostForm<Post>(uri, new[]
                {
                    new KeyValuePair<string, string>("board", guildName),
                    new KeyValuePair<string, string>("title", title),
                    new KeyValuePair<string, string>("body", text ?? string.Empty),
                    new KeyValuePair<string, string>("url", url ?? string.Empty)
                });
            }

            await using var imageStream = File.OpenRead(imagePath);
            var content = new MultipartFormDataContent 
            { 
                { new StringContent(guildName), "board" },
                { new StringContent(title), "title"},
                { new StringContent(text ?? string.Empty), "body" },
                { new StringContent(url ?? string.Empty), "url" },
                { new StreamContent(imageStream), "file", imagePath }
            };
            
            var response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
            return JsonHelper.Load<Post>(await response.Content.ReadAsStreamAsync());
        }

        private async Task<Comment> SubmitComment(string parentFullname, string postId, string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Comment text body cannot be null or empty.", nameof(text));
            
            var uri = new Uri("/api/v1/comment", UriKind.Relative);
            return await PostForm<Comment>(uri, new[]
            {
                new KeyValuePair<string, string>("submission", postId), 
                new KeyValuePair<string, string>("parent_fullname", parentFullname),
                new KeyValuePair<string, string>("body", text),
            });
        }
    }
}