using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbnailGenerate
{
    public class ComputerVision
    {

        public void Run(string subscriptionKey, string remoteImageUrl, string localImagePath, string endPoint = "https://westcentralus.api.cognitive.microsoft.com", bool storeToDisk = false, int Width = 100, int Height = 100)
        {
            ComputerVisionClient computerVisionClientt = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });
            computerVisionClientt.Endpoint = endPoint;

            var x1 = GetRemoteThumbnailTask(computerVisionClientt, remoteImageUrl, Width, Height, storeToDisk);
            var x2 = GetLocalThumbnailTask(computerVisionClientt, localImagePath, Width, Height, storeToDisk, localImagePath);
            Task.WhenAll(x1, x2).Wait(5000);
        }
        private static async Task GetRemoteThumbnailTask(ComputerVisionClient computerVision, string imageUrl, int Width, int Height, bool storeToDisk)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                return;
            Stream thumbnail = await computerVision.GenerateThumbnailAsync(
                Width, Height, imageUrl, true);

            string path = Environment.CurrentDirectory;
            string imageName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);
            string thumbnailFilePath =
                path + "\\" + imageName.Insert(imageName.Length - 4, "_thumb");
            Save(thumbnail, thumbnailFilePath, storeToDisk);
        }
        private static async Task GetLocalThumbnailTask(ComputerVisionClient computerVisionClient, string imagePath, int Width, int Height, bool storeToDisk, string localImagePath)
        {
            if (!File.Exists(imagePath))
                return;
            using (Stream stream = File.OpenRead(imagePath))
            {
                Stream thumbnail = await computerVisionClient.GenerateThumbnailInStreamAsync(
                    Width, Height, stream, true);

                string thumbnailFilePath =
                    localImagePath.Insert(localImagePath.Length - 4, "_thumb");
                Save(thumbnail, thumbnailFilePath, storeToDisk);
            }
        }
        private static void Save(Stream thumbnail, string thumbnailFilePath, bool storeToDisk)
        {
            if (storeToDisk)
            {
                using (Stream file = File.Create(thumbnailFilePath))
                {
                    thumbnail.CopyTo(file);
                }
            }
        }
    }

}
