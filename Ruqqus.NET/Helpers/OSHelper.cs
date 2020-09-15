using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ruqqus.Helpers
{
    public static class OSHelper
    {
        public static void OpenInBrowser([NotNull] Uri uri) => OpenInBrowser(uri.ToString());
        
        public static void OpenInBrowser([NotNull] string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", url);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", url);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Because as usual Windows is has to be stupid about everything...
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else
            {
                // Cross your fingers and hope for the best.
                Process.Start(url);
            }
        }

        

    }
}