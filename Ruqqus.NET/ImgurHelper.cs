using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Ruqqus
{
    public static class ImgurHelper
    {
        /// <summary>
        /// Helper method to upload an image to Imgur and returns the direct image link to the file, which is suitable
        /// for image posts on Ruqqus. 
        /// </summary>
        /// <param name="imagePath">The path to an image file.</param>
        /// <param name="clientId">An application client ID issued by Imgur.</param>
        /// <returns>The direct link to the image, or <c>null</c> if the upload failed.</returns>
        /// <remarks>An application can be registered for free at https://imgur.com/account/settings/apps.</remarks>
        public static async Task<string> UploadAsync([NotNull] string imagePath, [NotNull] string clientId)
        {
            if (!File.Exists(imagePath ?? throw new ArgumentNullException(nameof(imagePath))))
                throw new FileNotFoundException($"Cannot locate file \"{imagePath}\"", imagePath);
            
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentNullException(nameof(clientId));

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd(Client.UserAgent);
            client.DefaultRequestHeaders.Add("Authorization", $"Client-ID {clientId}");

            await using var stream = File.OpenRead(imagePath);
            var content = new MultipartFormDataContent
            {
                { new StringContent("file"), "type" },
                { new StreamContent(stream), "image", imagePath }
            };

            var uri = new Uri("https://api.imgur.com/3/upload", UriKind.Absolute);
            var response = await client.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();

            var result = JsonHelper.Load<ImgurResult>(await response.Content.ReadAsStreamAsync());
            return result.Success ? result.Data.Link : null;
        }
        
        [DataContract]
        class ImgurData
        {
            [DataMember(Name = "link")]
            public string Link;
        }
        
        [DataContract, KnownType(typeof(ImgurData))]
        class ImgurResult
        {
            [DataMember(Name = "success")]
            public bool Success;

            [DataMember(Name = "data")]
            public ImgurData Data;
        }
    }
}