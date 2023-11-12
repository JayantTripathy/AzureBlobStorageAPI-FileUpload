using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace BlobStorageAPI.Services
{
    public class AzureBlobService
    {
        BlobServiceClient _blobClient;
        private readonly IConfiguration _configuration;
        BlobContainerClient _containerClient;
        public AzureBlobService(IConfiguration Configuration)
        {  
            _configuration = Configuration;
            _blobClient = new BlobServiceClient(_configuration.GetSection("BlobStorgaeInfo:ConnectionStrings").Value.ToString());
            _containerClient = _blobClient.GetBlobContainerClient(_configuration.GetSection("BlobStorgaeInfo:StorageName").Value.ToString());
        }

        public async Task<List<Azure.Response<BlobContentInfo>>> UploadFiles(List<IFormFile> files)
        {

            var azureResponse = new List<Azure.Response<BlobContentInfo>>();
            foreach (var file in files)
            {
                string fileName = file.FileName;
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    var client = await _containerClient.UploadBlobAsync(fileName, memoryStream, default);
                    azureResponse.Add(client);
                }
            };

            return azureResponse;
        }

        public async Task<List<BlobItem>> GetUploadedBlobs()
        {
            var items = new List<BlobItem>();
            var uploadedFiles = _containerClient.GetBlobsAsync();
            await foreach (BlobItem file in uploadedFiles)
            {
                items.Add(file);
            }

            return items;
        }
    }
}

