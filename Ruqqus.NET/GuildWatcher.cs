using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ruqqus
{
    /// <summary>
    /// Asynchronously monitors for new posts created in a specific Ruqqus guild.
    /// </summary>
    public class GuildWatcher : Watcher<Post>
    {
        /// <summary>
        /// Gets the name of the guild being monitored.
        /// </summary>
        public string GuildName { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="GuildWatcher"/> class.
        /// </summary>
        /// <param name="client">A valid <see cref="Client"/> that will be used for queries.</param>
        /// <param name="guildName">The name of the guild to monitor.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="guildName"/> if <c>null</c> or empty.</exception>
        public GuildWatcher([NotNull] Client client, [NotNull] string guildName) : base(client)
        {
            if (string.IsNullOrEmpty(guildName))
                throw new ArgumentNullException(nameof(guildName));

            GuildName = guildName;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="GuildWatcher"/> class.
        /// </summary>
        /// <param name="client">A valid <see cref="Client"/> that will be used for queries.</param>
        /// <param name="guild">The guild to monitor.</param>
        public GuildWatcher([NotNull] Client client, [NotNull] Guild guild) : base(client)
        {
            GuildName = guild?.Name ?? throw new ArgumentNullException(nameof(guild));
        }

        /// <inheritdoc />
        protected override async Task CheckForContent(DateTime time, [NotNull] CancellationToken token)
        {
            await foreach (var post in Client.GetGuildPosts(GuildName).WithCancellation(token))
            {
                if (post.CreationTime < time)
                    break;
                OnItemCreated(post);
            }
        }
    }
}