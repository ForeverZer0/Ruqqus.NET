using System;
using System.IO;
using System.Threading.Tasks;
using Ruqqus;
using Ruqqus.Security;

namespace Ruqbot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var tokenPath = Path.Combine(desktop, "token.json");
            
            using var client = new Client(ClientInfo.Environment, Token.LoadJson(tokenPath));
            var all = new AllWatcher(client);
            all.ItemCreated += OnNewPostCreated;
            await all.StartAsync(TimeSpan.FromSeconds(15));
        }

        private static void OnNewPostCreated(object sender, ItemEventArgs<Post> e)
        {
            Console.WriteLine($"[NEW POST] {e.Item.Title}");
        }
    }
}