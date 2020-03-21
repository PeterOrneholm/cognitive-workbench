using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;

using Orneholm.CognitiveWorkbench.Web.Models;

namespace Orneholm.CognitiveWorkbench.Web.Services
{
    public static class ImageInfoProcessor
    {
        public static async Task<ImageInfo> GetImageInfo(string url, IHttpClientFactory httpClientFactory)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "CognitiveWorkbench"); // Allow to GET some images returning error when no user-agent is set

            var stream = await httpClient.GetStreamAsync(url);
            using var image = Image.FromStream(stream);

            return new ImageInfo
            {
                Url = url,

                Width = image.Width,
                Height = image.Height,

                Description = $"Image from {url}"
            };
        }
    }
}
