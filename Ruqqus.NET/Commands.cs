using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Ruqqus.Security;

namespace Ruqqus
{
    public partial class Client
    {
        [Obsolete("Experimental features, do not use.")]
        [Scope(OAuthScope.None)]
        public async Task<bool> LoginAsync(string username, string password)
        {
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });
            
            var uri = new Uri("/login", UriKind.Relative);
            var response = await httpClient.PostAsync(uri, form);
            return response.IsSuccessStatusCode;
        }

        [Obsolete("Experimental features, do not use.")]
        public async Task<bool> SubscribeAsync(Guild guild)
        {
            if (guild is null)
                return false;
            return await SubscribeAsync(guild.Name);
        }

        [Obsolete("Experimental features, do not use.")]
        [Scope(OAuthScope.None)]
        public async Task<bool> SubscribeAsync(string guildName)
        {
            if (string.IsNullOrWhiteSpace(guildName) || !ValidGuildName.IsMatch(guildName))
                return false;
            
            var uri = new Uri($"/api/subscribe/{guildName}", UriKind.Relative);
            var response = await httpClient.PostAsync(uri, null);
            return response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Conflict;
        }
    }
}