using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ruqqus
{
    /// <summary>
    /// Asynchronously monitors for new posts on Ruqqus.
    /// </summary>
    public class AllWatcher : Watcher<Post>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GuildWatcher"/> class.
        /// </summary>
        /// <param name="client">A valid <see cref="Client"/> that will be used for queries.</param>
        public AllWatcher([NotNull] Client client) : base(client)
        {
        }

        /// <inheritdoc />
        protected override async Task CheckForContent(DateTime time, CancellationToken token)
        {
            await foreach (var post in Client.GetAllPosts().WithCancellation(token))
            {
                if (post.CreationTime < time)
                    break;
                OnItemCreated(post);
            }
        }
    }
}