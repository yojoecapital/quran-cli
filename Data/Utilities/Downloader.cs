using System.IO;
using System.Net.Http;

namespace QuranCli.Data.Utilities
{
    internal static class Downloader
    {
        public static void Download(this HttpClient client, string url, string destinationPath)
        {
            Logger.Message($"Downloading resource from '{url}'.");
            using var response = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result;
            response.EnsureSuccessStatusCode();

            // Get content length for progress calculation
            var totalBytes = response.Content.Headers.ContentLength;
            using var contentStream = response.Content.ReadAsStream();
            using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
            byte[] buffer = new byte[8192];
            int bytesRead;
            long totalBytesRead = 0;
            while ((bytesRead = contentStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
                if (totalBytes.HasValue) Logger.Percent(totalBytesRead, totalBytes.Value);
            }
        }
    }

}