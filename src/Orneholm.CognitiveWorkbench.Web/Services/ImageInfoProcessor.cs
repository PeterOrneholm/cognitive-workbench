using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Orneholm.CognitiveWorkbench.Web.Models.Generic;

namespace Orneholm.CognitiveWorkbench.Web.Services
{
    public static class ImageInfoProcessor
    {
        public static async Task<ImageInfo> GetImageInfoFromImageUrlAsync(string url, IHttpClientFactory httpClientFactory)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "CognitiveWorkbench"); // Allow to GET some images returning error when no user-agent is set

            var stream = await httpClient.GetStreamAsync(url);
            using var image = Image.FromStream(stream);

            return new ImageInfo
            {
                Src = url,

                Width = image.Width,
                Height = image.Height,

                Description = $"Image from {url}"
            };
        }

        public static ImageInfo GetImageInfoFromStream(MemoryStream imageStream, string fileContentType)
        {
            using var image = Image.FromStream(imageStream);

            // Get image for display
            var fileBytes = imageStream.ToArray();
            var imageData = $"data:{fileContentType};base64,{Convert.ToBase64String(fileBytes)}";

            return new ImageInfo
            {
                Src = imageData,

                Width = image.Width,
                Height = image.Height,

                Description = $"Image from file"
            };
        }
    }
}
