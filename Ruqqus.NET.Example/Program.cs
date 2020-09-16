using System;
using System.IO;
using System.Threading.Tasks;
using Ruqqus.Security;

namespace Ruqqus
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            // Load a token
            var tokenPath = Path.Combine(Directory.GetCurrentDirectory(), "token.json");
            var token = Token.LoadJson(tokenPath);
            
            // Load a client
            var info = ClientInfo.Environment;

            using (var client = new Client(info, token))
            {
                var watcher = new AllWatcher(client);
                watcher.Started += (sender, eventArgs) => Console.WriteLine("Watching \"All\" for new posts...");
                watcher.Stopped += (sender, eventArgs) => Console.WriteLine("Monitoring stopped");
                watcher.ItemCreated += (sender, eventArgs) => Console.WriteLine(eventArgs.Item.Title);
                
                await watcher.StartAsync(TimeSpan.FromSeconds(15));
            }
        }
    }
}