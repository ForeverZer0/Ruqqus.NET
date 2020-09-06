using System;
using System.Drawing;
using System.Runtime.Serialization;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable UnusedMember.Global

namespace Ruqqus.NET
{
    /// <summary>
    /// Represents a Ruqqus.NET guild, which a board where posts can be posted.
    /// </summary>
    [DataContract, KnownType(typeof(InfoBase))]
    public class Guild : InfoBase
    {
        /// <summary>
        /// Gets the name of the guild.
        /// </summary>
        [field: DataMember(Name = "name", IsRequired = true)]
        public string Name { get; }

        /// <summary>
        /// Gets the number of guildmasters who moderate the guild.
        /// </summary>
        [field: DataMember(Name = "mods_count")]
        public int GuildmasterCount { get; }

        /// <summary>
        /// Gets the total number of members subscribed to the guild.
        /// </summary>
        [field: DataMember(Name = "subscriber_count")]
        public int MemberCount { get; }

        /// <summary>
        /// Gets the description of the guild.
        /// </summary>
        [field: DataMember(Name = "description")]
        public string Description { get; }
        
        /// <summary>
        /// Gets the description of the guild in HTML format.
        /// </summary>
        [field: DataMember(Name = "description_html")]
        public string DescriptionHtml { get; }
        
        /// <summary>
        /// Gets a value indicating if guild contains adult content.
        /// </summary>
        [field: DataMember(Name = "over_18")]
        public bool IsNsfw { get; }
        
        /// <summary>
        /// Gets a value indicating if see the posts of the guild requires membership.
        /// </summary>
        [field: DataMember(Name = "is_private")]
        public bool IsPrivate { get; }
        
        /// <summary>
        /// Gets a value indicated if posting is restricted in this guild pending guildmaster approval.
        /// </summary>
        [field: DataMember(Name = "is_restricted")]
        public bool IsRestricted { get; }
        
        /// <summary>
        /// Gets the full-length ID of this guild.
        /// </summary>
        [field: DataMember(Name = "fullname")]
        public string FullName { get; }

        /// <summary>
        /// Gets the accent color the guild uses for buttons, highlight, etc.
        /// </summary>
        public Color Color => ColorTranslator.FromHtml(color);

        /// <summary>
        /// Gets the URL to the guild's profile image.
        /// </summary>
        public Uri ProfileUrl => string.IsNullOrEmpty(profileUrl) ? null : new Uri(profileUrl, UriKind.RelativeOrAbsolute);
        
        /// <summary>
        /// Gets the URL to the guild's banner image.
        /// </summary>
        public Uri BannerUrl => string.IsNullOrEmpty(bannerUrl) ? null : new Uri(bannerUrl, UriKind.RelativeOrAbsolute);

        /// <inheritdoc />
        public override string ToString() => Name;

        [DataMember(Name = "profile_url")]
        private string profileUrl;
        
        [DataMember(Name = "banner_url")]
        private string bannerUrl;

        [DataMember(Name = "color")]
        private string color;
    }
}
