using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace Ruqqus.NET
{
    /// <summary>
    /// A client for interacting and perfomring operations with the Ruqqus API.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public partial class RuqqusClient : IDisposable
    {
        /// <summary>
        /// The user-agent of Ruqqus.NET.
        /// </summary>
        public static readonly string UserAgent;

        /// <summary>
        /// The number of results per page when enumerating over large collections.
        /// </summary>
        public const int ResultsPerPage = 25;

        /// <summary>
        /// Static initializer.
        /// </summary>
        static RuqqusClient()
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
        [Authorization(AuthorityKind.None, OAuthScope.None)]
        public static async Task<bool> IsUsernameAvailable([CanBeNull] string username)
        {
            return await IsAvailable("https://ruqqus.com/api/is_available/", ValidUsername, username);
        }
        
        /// <summary>
        /// Checks if the specified guild name is both valid and available.
        /// </summary>
        /// <param name="guildName">A username to query.</param>
        /// <returns><c>true</c> if the guild name is both valid and available, otherwise <c>false</c>.</returns>
        [Authorization(AuthorityKind.None, OAuthScope.None)]
        public static async Task<bool> IsGuildNameAvailable([CanBeNull] string guildName)
        {
            return await IsAvailable("https://ruqqus.com/api/board_available/", ValidGuildName, guildName);
        }

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

            return Regex.IsMatch(result, @":true\b");

        }
        
        
        /// <summary>
        /// Validates an input string and returns value indicating if it is a valid username.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if input string is valid, otherwise <c>false</c>.</returns>
        public static bool IsValidUsername([CanBeNull] string username) => username != null && ValidUsername.IsMatch(username);
        
        /// <summary>
        /// Validates an input string and returns value indicating if it is a valid guild name.
        /// </summary>
        /// <param name="guildName">The name of the guild.</param>
        /// <returns><c>true</c> if input string is valid, otherwise <c>false</c>.</returns>
        public static bool IsValidGuildName([CanBeNull] string guildName) => guildName != null && ValidGuildName.IsMatch(guildName);

        /// <summary>
        /// Validates an input string and returns value indicating if it is a valid ID for a post/comment.
        /// </summary>
        /// <param name="id">The ID for a post or comment.</param>
        /// <returns><c>true</c> if input string is valid, otherwise <c>false</c>.</returns>
        public static bool IsValidSubmissionId([CanBeNull] string id) => id != null && ValidSubmission.IsMatch(id);
        
        /// <summary>
        /// Occurs when the client's current access token is refreshed.
        /// </summary>
        public event EventHandler<TokenRefreshedEventArgs> TokenRefreshed;

        /// <summary>
        /// Gets or sets the authorization token granting access to the client/
        /// </summary>
        public OAuthToken Token
        {
            get => token;
            set
            {
                token = value;
                httpClient.DefaultRequestHeaders.Authorization = value is null 
                    ? null 
                    : new AuthenticationHeaderValue(value.Type, value.AccessToken);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RuqqusClient"/> class.
        /// </summary>
        /// <param name="clientId">The client ID of the application requesting access.</param>
        /// <param name="clientSecret">The client secret of the application requesting access.</param>
        /// <exception cref="ArgumentNullException">Thrown when the ID/secret is null or empty.</exception>
        public RuqqusClient(string clientId, string clientSecret)
        {
            ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            ClientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));

            httpClient = new HttpClient { BaseAddress = new Uri("https://ruqqus.com") };
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="RuqqusClient"/> class with the specified <see cref="OAuthToken"/>.
        /// </summary>
        /// <param name="clientId">The client ID of the application requesting access.</param>
        /// <param name="clientSecret">The client secret of the application requesting access.</param>
        /// <param name="token">A previously authorized access token.</param>
        /// <exception cref="ArgumentNullException">Thrown when the ID/secret is null or empty.</exception>
        public RuqqusClient([JetBrains.Annotations.NotNull] string clientId, [JetBrains.Annotations.NotNull] string clientSecret, [JetBrains.Annotations.NotNull] OAuthToken token) : this(clientId, clientSecret)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
        
        /// <summary>
        /// Authorizes the application to perform actions as a user, using the authorization code from the redirect URL
        /// when the user made approval.
        /// </summary>
        /// <param name="code">The authorization code from the redirect URL.</param>
        /// <param name="persist">Flag indicating if this token will persist and be reused more than once.</param>
        /// <returns>A newly created access token.</returns>
        /// <remarks>This action can only be performed once per code, and will fail otherwise.</remarks>
        [Authorization(AuthorityKind.None)]
        public async Task<OAuthToken> GrantTokenAsync([JetBrains.Annotations.NotNull] string code, bool persist = true)
        {
            var uri = new Uri("/oauth/grant", UriKind.Relative);
            return await PostForm<OAuthToken>(uri, new [] 
            {
                new KeyValuePair<string, string>("grant_type", "code"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("code", code), 
                new KeyValuePair<string, string>("permenant", persist ? "persist" : "nope"), 
            });
        }

        /// <summary>
        /// Refreshes a stale access token as needed. 
        /// </summary>
        /// <returns>A task indicating <c>true</c> if access token was refreshed, otherwise <c>false</c>.</returns>
        [Authorization(AuthorityKind.None)]
        public async Task<bool> RefreshTokenAsync()
        {
            if (Token?.RefreshToken is null || Token.RemainingSeconds > OAuthToken.RefreshMargin)
                return false;

            // Can't call PostForm here, would call a stack overflow
            var uri = new Uri("/oauth/grant", UriKind.Relative);
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("refresh_token", Token.RefreshToken), 
            });

            var response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            var refreshToken = JsonHelper.Load<OAuthToken>(await response.Content.ReadAsStreamAsync());
            token.Update(refreshToken);
            OnTokenRefreshed();
            return true;
        }

        /// <summary>
        /// Updates the client's authorization header and invokes the <see cref="TokenRefreshed"/> event.
        /// </summary>
        protected virtual void OnTokenRefreshed()
        {
            var header = new AuthenticationHeaderValue(token.Type, token.AccessToken);
            httpClient.DefaultRequestHeaders.Authorization = header;
            TokenRefreshed?.Invoke(this, new TokenRefreshedEventArgs(this, Token));
        }

        private async Task AssertAuthorizationAsync(bool required = true)
        {
            if (token is null)
            {
                if (required)
                    throw new AuthenticationException("Application authentication is required for this action.");
                return;
            }
            await RefreshTokenAsync();
        }
        
        private readonly HttpClient httpClient;
        protected readonly string ClientId;
        protected readonly string ClientSecret;
        private OAuthToken token;
        
        public void Dispose()
        {
            httpClient.Dispose();
        }

        private async Task<T> PostForm<T>(Uri uri, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            await AssertAuthorizationAsync();
            
            var content = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            return JsonHelper.Load<T>(stream);
        }
    }
}