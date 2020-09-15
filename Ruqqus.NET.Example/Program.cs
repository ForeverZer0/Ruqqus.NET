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
                var user = await client.GetUserAsync("foreverzer0");
                Console.WriteLine(user.ToJson());
                Console.WriteLine(user);
            }
        }
    }
}