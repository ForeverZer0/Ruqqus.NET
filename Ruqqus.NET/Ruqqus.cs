using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

// ReSharper disable UnusedMember.Global

namespace Ruqqus.NET
{
    /// <summary>
    /// A client for interacting with the Ruqqus REST API.
    /// </summary>
    public class Ruqqus : IDisposable
    {
        /// <summary>
        /// The user-agent of Ruqqus.NET.NET.
        /// </summary>
        public static readonly string UserAgent;

        /// <summary>
        /// Static initializer.
        /// </summary>
        static Ruqqus()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var info = FileVersionInfo.GetVersionInfo(assembly.Location);
            UserAgent = $"Ruqqus.NET.NET/{info.FileVersion}";

            const RegexOptions opts = RegexOptions.Singleline | RegexOptions.CultureInvariant;
            validUsername = new Regex("^[a-zA-Z0-9_]{5,25}$", opts);
            validGuildName = new Regex("^.{8,100}$", opts);
            validSubmission = new Regex("^[A-Za-z0-9_]+$", opts);
        }

        /// <summary>
        /// Creates a new instance of the Ruqqus client.
        /// </summary>
        public Ruqqus() : this(UserAgent)
        {
        }
        
        /// <summary>
        /// Creates a new instance of the Ruqqus client with a custom user-agent.
        /// </summary>
        /// <param name="userAgent">A custom user-agent to identify the application.</param>
        public Ruqqus([NotNull] string userAgent) 
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            
            userSerializer = new DataContractJsonSerializer(typeof(User));
            guildSerializer = new DataContractJsonSerializer(typeof(Guild));
            commentSerializer = new DataContractJsonSerializer(typeof(Comment));
            postSerializer = new DataContractJsonSerializer(typeof(Post));
        }
        
        /// <summary>
        /// Asynchronously retrieves the <see cref="User"/> with the specified username.
        /// </summary>
        /// <param name="username">The username of the account to retrieve.</param>
        /// <returns>A <see cref="User"/> instance of <c>null</c> if not found.</returns>
        /// <exception cref="FormatException">Thrown when the <paramref name="username"/> is not well-formatted.</exception>
        /// <seealso cref="validUsername"/>
        public async Task<User> GetUserAsync([NotNull] string username)
        {
            if (!IsValidUsername(username))
                throw new FormatException("Invalid username.");
            return await QueryObjectAsync<User>($"{RUQQUS}{USER}{username}", userSerializer);
        }
        
        /// <summary>
        /// Asynchronously retrieves the <see cref="Comment"/> with the specified ID.
        /// </summary>
        /// <param name="commentId">The ID of the comment to retrieve.</param>
        /// <returns>A <see cref="Comment"/> instance of <c>null</c> if not found.</returns>
        /// <exception cref="FormatException">Thrown when the <paramref name="commentId"/> is not well-formatted.</exception>
        /// <seealso cref="validSubmission"/>
        public async Task<Comment> GetCommentAsync([NotNull] string commentId)
        {
            if (!IsValidSubmissionId(commentId))
                throw new FormatException("Invalid comment ID.");
            return await QueryObjectAsync<Comment>($"{RUQQUS}{COMMENT}{commentId}", commentSerializer);
        }
        
        /// <summary>
        /// Asynchronously retrieves the <see cref="Post"/> with the specified ID.
        /// </summary>
        /// <param name="postId">The ID of the post to retrieve.</param>
        /// <returns>A <see cref="Post"/> instance of <c>null</c> if not found.</returns>
        /// <exception cref="FormatException">Thrown when the <paramref name="postId"/> is not well-formatted.</exception>
        /// <seealso cref="validSubmission"/>
        public async Task<Post> GetPostAsync([NotNull] string postId)
        {
            if (!IsValidSubmissionId(postId))
                throw new FormatException("Invalid post ID.");
            return await QueryObjectAsync<Post>($"{RUQQUS}{POST}{postId}", postSerializer);
        }
        
        /// <summary>
        /// Asynchronously retrieves the <see cref="Guild"/> with the specified name.
        /// </summary>
        /// <param name="guildName">The name of the guild to retrieve.</param>
        /// <returns>A <see cref="Guild"/> instance of <c>null</c> if not found.</returns>
        /// <exception cref="FormatException">Thrown when the <paramref name="guildName"/> is not well-formatted.</exception>
        /// <seealso cref="validGuildName"/>
        public async Task<Guild> GetGuildAsync([NotNull] string guildName)
        {
            if (!IsValidGuildName(guildName))
                throw new FormatException("Invalid guild name.");
            return await QueryObjectAsync<Guild>($"{RUQQUS}{GUILD}{guildName}", guildSerializer);
        }

        /// <summary>
        /// Validates an input string and returns value indicating if it is a valid username.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if input string is valid, otherwise <c>false</c>.</returns>
        public bool IsValidUsername([CanBeNull] string username) => username != null && validUsername.IsMatch(username);
        
        /// <summary>
        /// Validates an input string and returns value indicating if it is a valid guild name.
        /// </summary>
        /// <param name="guildName">The name of the guild.</param>
        /// <returns><c>true</c> if input string is valid, otherwise <c>false</c>.</returns>
        public bool IsValidGuildName([CanBeNull] string guildName) => guildName != null && validGuildName.IsMatch(guildName);

        /// <summary>
        /// Validates an input string and returns value indicating if it is a valid ID for a post/comment.
        /// </summary>
        /// <param name="id">The ID for a post or comment.</param>
        /// <returns><c>true</c> if input string is valid, otherwise <c>false</c>.</returns>
        public bool IsValidSubmissionId([CanBeNull] string id) => id != null && validSubmission.IsMatch(id);

        /// <inheritdoc />
        public void Dispose() => client.Dispose();

        /// <summary>
        /// Invokes the REST API to get a JSON object to deserialize.  
        /// </summary>
        /// <param name="route">The API route where the GET method is invoked.</param>
        /// <param name="serializer">A JSON serializer for the given type.</param>
        /// <typeparam name="T">A type that inherits from <see cref="ItemBase"/>.</typeparam>
        /// <returns>The object instance, or <c>null</c> if not found.</returns>
        private async Task<T> QueryObjectAsync<T>([NotNull] string route, [NotNull] XmlObjectSerializer serializer) where T : ItemBase
        {
            try
            {
                await using var stream = await client.GetStreamAsync(route);
                return (T) serializer.ReadObject(stream);
            }
            catch (HttpRequestException)
            {
                // Invalid value sent
                return default;
            }
        }
        
        private const string RUQQUS  = "https://ruqqus.com";
        private const string USER    = "/api/v1/user/";
        private const string GUILD   = "/api/v1/guild/";
        private const string POST    = "/api/v1/post/";
        private const string COMMENT = "/api/v1/comment/";
        
        private static readonly Regex validUsername;
        private static readonly Regex validGuildName;
        private static readonly Regex validSubmission;
        
        private readonly HttpClient client;
        private readonly DataContractJsonSerializer userSerializer;
        private readonly DataContractJsonSerializer guildSerializer;
        private readonly DataContractJsonSerializer commentSerializer;
        private readonly DataContractJsonSerializer postSerializer;
    }
}