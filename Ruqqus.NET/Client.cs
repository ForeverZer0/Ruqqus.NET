using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ruqqus.Helpers;
using Ruqqus.Security;

[assembly: CLSCompliant(true)]

namespace Ruqqus
{
    /// <summary>
    /// A client for interacting and performing operations with the Ruqqus API.
    /// </summary>
    public partial class Client : IDisposable
    {
        /// <summary>
        /// The user-agent of Ruqqus.NET.
        /// </summary>
        public static readonly string UserAgent;

        /// <summary>
        /// Static initializer.
        /// </summary>
        static Client()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var info = FileVersionInfo.GetVersionInfo(assembly.Location);
            UserAgent = $"Ruqqus.NET/{info.FileVersion}";

            const RegexOptions opts = RegexOptions.Singleline | RegexOptions.CultureInvariant;
            ValidUsername = new Regex("^[a-zA-Z0-9_]{5,25}$", opts);
            ValidGuildName = new Regex("^[a-zA-Z0-9][a-zA-Z0-9_]{2,24}$", opts);
            ValidSubmission = new Regex("^[A-Za-z0-9_]+$", opts);

            UserSerializer = new DataContractJsonSerializer(typeof(User));
            GuildSerializer = new DataContractJsonSerializer(typeof(Guild));
            CommentSerializer = new DataContractJsonSerializer(typeof(Comment));
            PostSerializer = new DataContractJsonSerializer(typeof(Post));
        }

        private static readonly Regex ValidUsername;
        private static readonly Regex ValidGuildName;
        private static readonly Regex ValidSubmission;
        private static readonly DataContractJsonSerializer UserSerializer;
        private static readonly DataContractJsonSerializer GuildSerializer;
        private static readonly DataContractJsonSerializer CommentSerializer;
        private static readonly DataContractJsonSerializer PostSerializer;

        /// <summary>
        /// Checks if the specified username is both valid and available.
        /// </summary>
        /// <param name="username">A username to query.</param>
        /// <returns><c>true</c> if the username is both valid and available, otherwise <c>false</c>.</returns>
        [Scope(OAuthScope.None)]
        public static async Task<bool> IsUsernameAvailable([CanBeNull] string username)
        {
            return await IsAvailable("https://ruqqus.com/api/is_available/", ValidUsername, username);
        }

        /// <summary>
        /// Checks if the specified guild name is both valid and available.
        /// </summary>
        /// <param name="guildName">A username to query.</param>
        /// <returns><c>true</c> if the guild name is both valid and available, otherwise <c>false</c>.</returns>
        [Scope(OAuthScope.None)]
        public static async Task<bool> IsGuildNameAvailable([CanBeNull] string guildName)
        {
            return await IsAvailable("https://ruqqus.com/api/board_available/", ValidGuildName, guildName);
        }
        
        [Scope(OAuthScope.None)]
        private static async Task<bool> IsAvailable(string route, Regex validator, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            if (!validator.IsMatch(name))
                return false;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            var uri = new Uri($"{route}{name}", UriKind.Absolute);
            var response = await client.GetAsync(uri);

            var result = await response.Content.ReadAsStringAsync();
            // Just cheating this, not going to parse the JSON. It will only contain a true value when available.
            return Regex.IsMatch(result, @":true\b");
        }

        /// <summary>
        /// Validates an input string and returns value indicating if it is a valid username.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if input string is valid, otherwise <c>false</c>.</returns>
        [Scope(OAuthScope.None)]
        public static bool IsValidUsername([CanBeNull] string username) =>
            username != null && ValidUsername.IsMatch(username);

        /// <summary>
        /// Validates an input string and returns value indicating if it is a valid guild name.
        /// </summary>
        /// <param name="guildName">The name of the guild.</param>
        /// <returns><c>true</c> if input string is valid, otherwise <c>false</c>.</returns>
        [Scope(OAuthScope.None)]
        public static bool IsValidGuildName([CanBeNull] string guildName) =>
            guildName != null && ValidGuildName.IsMatch(guildName);

        /// <summary>
        /// Validates an input string and returns value indicating if it is a valid ID for a post/comment.
        /// </summary>
        /// <param name="id">The ID for a post or comment.</param>
        /// <returns><c>true</c> if input string is valid, otherwise <c>false</c>.</returns>
        [Scope(OAuthScope.None)]
        public static bool IsValidSubmissionId([CanBeNull] string id) => id != null && ValidSubmission.IsMatch(id);

        /// <summary>
        /// Gets or sets the authorization accessToken granting access to the client/
        /// </summary>
        public Token Token
        {
            get => accessToken;
            set
            {
                accessToken = value;
                httpClient.DefaultRequestHeaders.Authorization = value is null
                    ? null
                    : new AuthenticationHeaderValue(value.Type, value.AccessToken);
            }
        }

        /// <summary>
        /// Private constructor to initialize the internal HTTP client.
        /// </summary>
        private Client()
        {
            httpClient = new HttpClient { BaseAddress = new Uri("https://ruqqus.com") };
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="clientInfo">A <see cref="ClientInfo"/> object describing the application.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="clientInfo"/> is <c>null</c>.</exception>
        public Client([NotNull] ClientInfo clientInfo) : this()
        {
            Info = clientInfo ?? throw new ArgumentNullException(nameof(clientInfo));
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="clientInfo">A <see cref="ClientInfo"/> object describing the application.</param>
        /// <param name="token">A previously authorized access token.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="clientInfo"/> is <c>null</c>.</exception>
        public Client([NotNull] ClientInfo clientInfo, Token token) : this(clientInfo)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="clientId">The client ID of the application requesting access.</param>
        /// <param name="clientSecret">The client secret of the application requesting access.</param>
        /// <param name="redirectUrl">The redirect URL to receive the confirmation code used when a user grants access to the client.</param>
        /// <exception cref="ArgumentNullException">Thrown when the ID/secret/redirect is <c>null</c> or empty.</exception>
        public Client([NotNull] string clientId, [NotNull] string clientSecret, string redirectUrl) : this()
        {
            Info = new ClientInfo(clientId, clientSecret, redirectUrl);
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="clientId">The client ID of the application requesting access.</param>
        /// <param name="clientSecret">The client secret of the application requesting access.</param>
        /// <param name="redirectUrl">The redirect URL to receive the confirmation code used when a user grants access to the client.</param>
        ///  <param name="token">A previously authorized access token.</param>
        /// <exception cref="ArgumentNullException">Thrown when the ID/secret/redirect is <c>null</c> or empty.</exception>
        public Client([NotNull] string clientId, [NotNull] string clientSecret, [NotNull] string redirectUrl, [NotNull] Token token) 
            : this(clientId, clientSecret, redirectUrl)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
        
        
        
        public async Task<bool> RefreshTokenAsync()
        {
            if (await OAuth.RefreshAsync(Info, accessToken))
            {
                var header = new AuthenticationHeaderValue(accessToken.Type, accessToken.AccessToken);
                httpClient.DefaultRequestHeaders.Authorization = header;
                return true;
            }
            return false;
        }

        public ClientInfo Info { get; }
        
        private readonly HttpClient httpClient;
        private Token accessToken;

        /// <inheritdoc />
        public void Dispose() => httpClient.Dispose();

        private async Task<T> PostForm<T>(Uri uri, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            await RefreshTokenAsync();

            var content = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            return JsonHelper.Load<T>(stream);
        }
    }
}