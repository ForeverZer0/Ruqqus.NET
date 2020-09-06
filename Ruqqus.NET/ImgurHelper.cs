using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Ruqqus.NET
{
    public static class ImgurHelper
    {


        public static async Task<string> UploadAsync([NotNull] string imagePath, [NotNull] string clientId)
        {
            if (!File.Exists(imagePath ?? throw new ArgumentNullException(nameof(imagePath))))
                throw new FileNotFoundException($"Cannot locate file \"{imagePath}\"", imagePath);
            
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentNullException(nameof(clientId));

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd(RuqqusClient.UserAgent);
                client.DefaultRequestHeaders.Add("Authorization", $"Client-ID {clientId}");
                
                await using var stream = File.OpenRead(imagePath);
                var content = new MultipartFormDataContent
                {
                    { new StringContent("file"), "type" },
                    { new StringContent("Test Title"), "title" },
                    { new StringContent("A Description"), "description" },
                    { new StreamContent(stream), "image", imagePath },
                };
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var uri = new Uri("https://api.imgur.com/3/upload", UriKind.Absolute);
                var response = await client.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();


                var str = await response.Content.ReadAsStringAsync();
                
                // Just cheat this instead of going through parsing the JSON all perfectly, we only need on value...
                // var regex = new Regex("", RegexOptions.IgnoreCase);


                return null;
            }
        }
        
        
        
    }
}