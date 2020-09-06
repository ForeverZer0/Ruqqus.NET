using System;
using System.Drawing;
using System.Runtime.Serialization;
using Ruqqus.Helpers;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ruqqus
{
    /// <summary>
    /// Represents an earned title that can be displayed with the username of an account.
    /// </summary>
    [DataContract]
    public class Title : IEquatable<Title>
    {
        /// <summary>
        /// Gets a unique ID used for identifying this entity.
        /// </summary>
        [field: DataMember(Name = "id", IsRequired = true)]
        public string Id { get; }

        /// <summary>
        /// Gets the text displayed for this title.
        /// </summary>
        [field: DataMember(Name = "text", IsRequired = true)]
        public string Text { get; }

        /// <summary>
        /// Gets the color of the text for the displayed title.
        /// </summary>
        public Color Color => ColorHelper.FromHtml(color);

        [field: DataMember(Name = "kind", IsRequired = true)]
        public int Kind { get; } // todo enum

        /// <inheritdoc />
        public bool Equals(Title other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Title) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => Id != null ? Id.GetHashCode() : 0;

        /// <summary>
        /// Gets a value indicating if this object is equal to another.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if objects are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(Title left, Title right) => Equals(left, right);

        /// <summary>
        /// Gets a value indicating if this object is not equal to another.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if objects are not equal, otherwise <c>false</c>.</returns>
        public static bool operator !=(Title left, Title right) => !Equals(left, right);

        [DataMember(Name = "color", IsRequired = true)]
        private string color;
    }
}