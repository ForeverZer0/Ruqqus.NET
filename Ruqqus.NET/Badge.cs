using System;
using System.Runtime.Serialization;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ruqqus.NET
{
    /// <summary>
    /// Represents a earned trophy that is displayed on the user's account page.
    /// </summary>
    [DataContract]
    public class Badge : IEquatable<Badge>
    {
        /// <summary>
        /// Gets the name of this badge.
        /// </summary>
        [field: DataMember(Name = "name", IsRequired = false)]
        public string Name { get; }
        
        /// <summary>
        /// Gets the description of this badge.
        /// </summary>
        [field: DataMember(Name = "text", IsRequired = false)]
        public string Text { get; }

        /// <summary>
        /// Gets a relative URL for that is associated with this object, or <c>null</c> if not defined.
        /// </summary>
        public Uri Uri => url is null ? null : new Uri(url, UriKind.Relative);
        
        /// <summary>
        /// Gets the time this badge was created.
        /// </summary>
        public DateTime CreationTime => DateTime.UnixEpoch + TimeSpan.FromSeconds(utc ?? 0);

        /// <inheritdoc />
        public override string ToString() => Text ?? GetType().ToString();

        /// <inheritdoc />
        public bool Equals(Badge other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Badge) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => (Name != null ? Name.GetHashCode() : 0);

        /// <summary>
        /// Gets a value indicating if this object is equal to another.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if objects are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(Badge left, Badge right) => Equals(left, right);

        /// <summary>
        /// Gets a value indicating if this object is not equal to another.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if objects are not equal, otherwise <c>false</c>.</returns>
        public static bool operator !=(Badge left, Badge right) => !Equals(left, right);
        
        [DataMember(Name = "url", IsRequired = false)]
        private string url;

        [DataMember(Name = "created_utc", IsRequired = false, EmitDefaultValue = true)]
        private int? utc ;
    }
}