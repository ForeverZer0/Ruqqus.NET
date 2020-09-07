using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Ruqqus
{
    /// <summary>
    /// Represents a Ruqqus.NET user account.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ItemBase)), KnownType(typeof(Badge)), KnownType(typeof(Title))]
    public class User : ItemBase
    {
        /// <summary>
        /// Gets the username associated with this account.
        /// </summary>
        [field: DataMember(Name = "username", IsRequired = true)]
        public string Username { get; }

        /// <summary>
        /// Gets a value indicating is this user account has been deleted.
        /// </summary>
        [field: DataMember(Name = "is_deleted")]
        public bool IsDeleted { get; }

        /// <summary>
        /// Gets the amount of rep the user has earned from posts.
        /// </summary>
        [field: DataMember(Name = "post_rep")]
        public int PostRep { get; }

        /// <summary>
        /// Gets the amount of rep the user has earned from comments.
        /// </summary>
        [field: DataMember(Name = "comment_rep")]
        public int CommentRep { get; }

        /// <summary>
        /// Gets the total rep for this user.
        /// </summary>
        public int TotalRep => PostRep + CommentRep;

        /// <summary>
        /// Gets the number of posts this user has created.
        /// </summary>
        [field: DataMember(Name = "post_count")]
        public int PostCount { get; }

        /// <summary>
        /// Gets the number of comments this user has created.
        /// </summary>
        [field: DataMember(Name = "comment_count")]
        public int CommentCount { get; }

        /// <summary>
        /// Gets the URI of the banner image associated with this user.
        /// </summary>
        public Uri BannerUri => banner is null ? null : new Uri(banner, UriKind.RelativeOrAbsolute);

        /// <summary>
        /// Gets the URI of the profile image associated with this user.
        /// </summary>
        public Uri ProfileUri => profile is null ? null : new Uri(profile, UriKind.RelativeOrAbsolute);

        /// <summary>
        /// Gets the current title of the user, or <c>null</c> if none is selected. 
        /// </summary>
        [field: DataMember(Name = "title", IsRequired = false)]
        public Title Title { get; }

        /// <summary>
        /// Gets a brief user-submitted summary/biography associated with the user. 
        /// </summary>
        [field: DataMember(Name = "bio")]
        public string Bio { get; }

        /// <summary>
        /// Gets a brief user-submitted summary/biography associated with the user in HTML format.
        /// </summary>
        [field: DataMember(Name = "bio_html")]
        public string BioHtml { get; }

        /// <summary>
        /// Gets a collection of badges the user has earned.
        /// </summary>
        public IEnumerable<Badge> Badges
        {
            get
            {
                if (badges is null)
                    yield break;
                foreach (var badge in badges)
                    yield return badge;
            }
        }

        /// <summary>
        /// Gets the number of badges the user has earned.
        /// </summary>
        public int BadgeCount => badges?.Count ?? 0;

        /// <inheritdoc />
        public override string ToString() => Username;

        [DataMember(Name = "banner_url")] private string banner;

        [DataMember(Name = "profile_url")] private string profile;

        [DataMember(Name = "badges")] private List<Badge> badges;
    }
}