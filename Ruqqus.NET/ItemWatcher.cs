using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ruqqus
{

    public class NewPostWatcher : ItemWatcher<Post>
    {
        private readonly string guildName;
        
        public NewPostWatcher(Client client, int refreshInterval, Guild guild) : this(client, refreshInterval, guild.Name)
        {
        }
        
        public NewPostWatcher(Client client, int refreshInterval, string guildName) : this(client, refreshInterval)
        {
            if (!Client.IsGuildNameAvailable(guildName).Result)
                throw new ArgumentException($"A guild with the name \"{guildName}\" does not exist.", nameof(guildName));
            this.guildName = guildName;
        }
        
        public NewPostWatcher(Client client, int refreshInterval) : base(client, refreshInterval)
        {
        }
        
        protected override async Task CheckForContent(DateTime lastCheck)
        {
            await foreach (var post in Client.GetAllPosts())
            {
                Console.WriteLine(lastCheck);
                if (post.CreationTime >= lastCheck)
                {
                    Console.WriteLine(post.Title);
                    OnNewItemFound(post);
                    continue;
                }
                break;
            }
        }
    }
    
    
    public class ItemEventArgs<T> : EventArgs where T : ItemBase
    {
        public T Item { get; }

        public ItemEventArgs(T item)
        {
            Item = item;
        }
    }
    
    public abstract class ItemWatcher<T> where T : ItemBase
    {
        /// <summary>
        /// Gets the number of seconds between the content watcher queries for new content.
        /// </summary>
        public int Interval { get; }

        /// <summary>
        /// Gets the <see cref="Client"/> used for interacting with Ruqqus.
        /// </summary>
        public Client Client { get; }

        public event EventHandler Stopped;

        public event EventHandler<ItemEventArgs<T>> NewItemFound;

        private bool cancelToken;
        
        public ItemWatcher(Client client, int refreshInterval)
        {
            if (refreshInterval < 1)
                throw new ArgumentException("Refresh interval must be greater than 1.", nameof(refreshInterval));

            Interval = refreshInterval * 1000;
        }

        protected virtual void OnNewItemFound(T item)
        {
            NewItemFound?.Invoke(this, new ItemEventArgs<T>(item));
        }
        
        public void Start()
        {

            // Already running, return
            if (cancelToken)
                return;
            
            cancelToken = false;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var time = DateTime.Now;
                    Thread.Sleep(Interval);
                    if (cancelToken)
                    {
                        cancelToken = false;
                        Stopped?.Invoke(this, EventArgs.Empty);
                        break;
                    }
                    Console.WriteLine("POOP");
                    CheckForContent(time);
                }
            });
        }

        public void Stop()
        {
            cancelToken = true;
        }

        protected abstract Task CheckForContent(DateTime lastCheck);
    }
}