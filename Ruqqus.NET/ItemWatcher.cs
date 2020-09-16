using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ruqqus
{
    /// <summary>
    /// Class used for event arguments pertaining to Ruqqus types.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="ItemBase"/>.</typeparam>
    public class ItemEventArgs<T> : EventArgs where T : ItemBase
    {
        /// <summary>
        /// Gets the object that raised the event.
        /// </summary>
        [CanBeNull]
        public T Item { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="ItemEventArgs{T}"/> class.
        /// </summary>
        /// <param name="item">The object that raised the event.</param>
        public ItemEventArgs([CanBeNull] T item)
        {
            Item = item;
        }
    }
    
    /// <summary>
    /// Abstract base class for objects capable asynchronous monitoring for new content on Ruqqus.
    /// </summary>
    /// <typeparam name="T">A type derived from <see cref="ItemBase"/>.</typeparam>
    public abstract class Watcher<T> where T : ItemBase
    {
        /// <summary>
        /// Gets the <see cref="Client"/> used for interacting with Ruqqus.
        /// </summary>
        [NotNull]
        public Client Client { get; }

        /// <summary>
        /// Occurs when a new item has been detected in the area being monitored.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemCreated;

        /// <summary>
        /// Occurs when monitoring begins.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Occurs when monitoring stops.
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Initializer for the <see cref="Watcher{T}"/> class.
        /// </summary>
        /// <param name="client">A valid <see cref="Client"/> that will be used for queries.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="client"/> is <c>null</c>.</exception>
        protected Watcher([NotNull] Client client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            cancelSource = new CancellationTokenSource();
            cancelToken = cancelSource.Token;
        }

        /// <summary>
        /// Begins asynchronously monitoring for new content.
        /// </summary>
        /// <param name="delay">The amount of time in between consecutive queries checking for new content.</param>
        /// <returns>An awaitable task for this operation.</returns>
        /// <seealso cref="Started"/>
        /// <seealso cref="Stopped"/>
        public async Task StartAsync(TimeSpan delay)
        {
            if (delay <= TimeSpan.Zero)
                throw new ArgumentException(Strings.InvalidDelay, nameof(delay));
            
            OnStarted();
            await Task.Run(async () =>
            {
                do
                {
                    var time = DateTime.UtcNow;
                    cancelToken.ThrowIfCancellationRequested();
                    await Task.Delay(delay, cancelToken);
                    await CheckForContent(time, cancelToken);
                } while (!cancelToken.IsCancellationRequested);
            }, cancelToken);
            OnStopped();
        }

        /// <summary>
        /// When overriden in a derived class, performs class-specific operations to check for new content since the
        /// specified <paramref name="time"/>.
        /// </summary>
        /// <param name="time">The time determining the threshold where new content should be ignored.</param>
        /// <param name="token">A token for cancelling any asynchronous tasks the method may implement.</param>
        /// <returns>An awaitable task to perform this operation asynchronously.</returns>
        protected abstract Task CheckForContent(DateTime time, [NotNull] CancellationToken token);

        /// <summary>
        /// Stops asynchronous operations and cancels monitoring.
        /// </summary>
        /// <seealso cref="Stopped"/>
        public void Stop() => cancelSource.Cancel();

        /// <summary>
        /// Invokes the <see cref="ItemCreated"/> event.
        /// </summary>
        /// <param name="item"></param>
        protected void OnItemCreated(T item) => ItemCreated?.Invoke(this, new ItemEventArgs<T>(item));

        /// <summary>
        /// Raises the <see cref="Started"/> event.
        /// </summary>
        protected virtual void OnStarted()
        {
            Client.Disposing += OnClientDisposing;
            Started?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="Stopped"/> event.
        /// </summary>
        protected virtual void OnStopped()
        {
            Client.Disposing -= OnClientDisposing;
            Stopped?.Invoke(this, EventArgs.Empty);
        }
        
        private void OnClientDisposing(Client client, EventArgs args) => cancelSource?.Cancel();

        private readonly CancellationTokenSource cancelSource;
        private readonly CancellationToken cancelToken;
    }
}