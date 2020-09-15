using System;
using System.Runtime.Serialization;

namespace Ruqqus.Security
{
    /// <summary>
    /// Simple container for storing OAuth client information. 
    /// </summary>
    [DataContract]
    public sealed class ClientInfo : JsonObject<ClientInfo>, IEquatable<ClientInfo>
    {
        /// <summary>
        /// The name of the environment variable containing the client ID value.
        /// </summary>
        public const string ENV_ID = "RUQQUS_CLIENT_ID";
        
        /// <summary>
        /// The name of the environment variable containing the client secret value.
        /// </summary>
        public const string ENV_SECRET = "RUQQUS_CLIENT_SECRET";

        /// <summary>
        /// The name of the environment variable containing the client redirect URL value.
        /// </summary>
        public const string ENV_REDIRECT = "RUQQUS_CLIENT_REDIRECT";
        
        /// <summary>
        /// Gets the unique identifier for the client.
        /// </summary>
        [DataMember(Name = "id", IsRequired = true)]
        public string Id { get; private set; }
        
        /// <summary>
        /// Gets the private code for the client.
        /// </summary>
        [DataMember(Name = "secret", IsRequired = true)]
        public string Secret { get; private set; }
        
        /// <summary>
        /// Gets the redirect URL to receive the confirmation code used when a user grants access to the client.
        /// </summary>
        [DataMember(Name = "redirect", IsRequired = true)]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Private constructor for serialization.
        /// </summary>
        private ClientInfo()
        {
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="ClientInfo"/> class.
        /// </summary>
        /// <param name="id">The unique identifier for the client.</param>
        /// <param name="secret">The private code for the client.</param>
        /// <param name="redirect">The redirect URL to receive the confirmation code.</param>
        public ClientInfo([NotNull] string id, [NotNull] string secret, [NotNull] string redirect) : this()
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Secret = secret ?? throw new ArgumentNullException(nameof(secret));
            RedirectUrl = redirect ?? throw new ArgumentNullException(nameof(redirect));
        }

        /// <summary>
        /// Gets a <see cref="ClientInfo"/> object by reading the values stored the host's environment variables.
        /// </summary>
        public static ClientInfo Environment =>
            new ClientInfo(GetEnvVariable(ENV_ID), GetEnvVariable(ENV_SECRET), GetEnvVariable(ENV_REDIRECT));

        /// <summary>
        /// Finds and returns the value of an environment variable on various platforms and locations.
        /// </summary>
        /// <param name="name">The name of the environment variable.</param>
        /// <returns>The value of the environment variable, or <c>null</c> if not found.</returns>
        private static string GetEnvVariable(string name)
        {
            // Windows handles variables differently than Unix-like systems, so we check all places..
            foreach (EnvironmentVariableTarget target in Enum.GetValues(typeof(EnvironmentVariableTarget)))
            {
                var value = System.Environment.GetEnvironmentVariable(name, target);
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return null;
        }

        /// <inheritdoc />
        public bool Equals(ClientInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Secret == other.Secret && RedirectUrl == other.RedirectUrl;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClientInfo) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 401) ^ (Secret != null ? Secret.GetHashCode() : 0);
                hashCode = (hashCode * 577) ^ (RedirectUrl != null ? RedirectUrl.GetHashCode() : 0);
                // ReSharper restore NonReadonlyMemberInGetHashCode
                return hashCode;
            }
        }

        /// <summary>
        /// Gets a value indicating if this object is equal to another.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if objects are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(ClientInfo left, ClientInfo right) => Equals(left, right);

        /// <summary>
        /// Gets a value indicating if this object is not equal to another.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if objects are not equal, otherwise <c>false</c>.</returns>
        public static bool operator !=(ClientInfo left, ClientInfo right) => !Equals(left, right);
    }
}