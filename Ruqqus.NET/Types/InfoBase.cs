﻿using System;
using System.Runtime.Serialization;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ruqqus.NET
{
    /// <summary>
    /// Abstract base class for all major API entities.
    /// </summary>
    [DataContract]
    public abstract class InfoBase : IEquatable<InfoBase>
    {
        /// <summary>
        /// Gets a unique ID used for identifying this entity.
        /// </summary>
        [field: DataMember(Name = "id", IsRequired = true)]
        public string Id { get; }

        /// <summary>
        /// Gets a value indicating if entity has been banned.
        /// </summary>
        [field: DataMember(Name = "is_banned", EmitDefaultValue = true, IsRequired = false)]
        public bool IsBanned { get; }
        
        /// <summary>
        /// Gets the reason the user was banned, or <c>null</c> if entity has not been banned.
        /// </summary>
        [field: DataMember(Name = "ban_reason", IsRequired = false)]
        public string BanReason { get; }

        /// <summary>
        /// Gets the time this entity was created.
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                if (utc == 0)
                    return DateTime.MinValue;
                return DateTime.UnixEpoch + TimeSpan.FromSeconds(utc);
            }
        }
        
        /// <summary>
        /// Gets the permanent direct link to this entity.
        /// </summary>
        public Uri PermaLink => permalink is null ? null : new Uri(permalink, UriKind.Relative);

        /// <inheritdoc />
        public bool Equals(InfoBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((InfoBase) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => (Id != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Id) : 0);

        /// <summary>
        /// Gets a value indicating if this object is equal to another.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if objects are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(InfoBase left, InfoBase right) => Equals(left, right);

        /// <summary>
        /// Gets a value indicating if this object is not equal to another.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if objects are not equal, otherwise <c>false</c>.</returns>
        public static bool operator !=(InfoBase left, InfoBase right) => !Equals(left, right);

        [DataMember(Name = "created_utc", IsRequired = false, EmitDefaultValue = true)]
        private long utc ;

        [DataMember(Name = "permalink",  IsRequired = false)]
        private string permalink;
    }
}