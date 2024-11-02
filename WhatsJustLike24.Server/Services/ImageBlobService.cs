using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using WhatsJustLike24.Server.Data;

namespace WhatsJustLike24.Server.Services
{
    public class ImageBlobService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly HttpClient _httpClient;

        public ImageBlobService(
            IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:STORAGE_ACCOUNT"];
            _containerName = configuration["BlobStorage:ImageContainer"];
            _httpClient = new HttpClient();
        }

        public async Task<string> UploadImageFromUrlAsync(string ImageUrl)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl);
            var blobClient = containerClient.GetBlobClient(blobName);

            var imageBytes = await _httpClient.GetByteArrayAsync(ImageUrl);

            using (var stream = new MemoryStream(imageBytes))
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobName;
        }
    }
}
