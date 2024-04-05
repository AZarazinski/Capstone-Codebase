using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

public class BlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private static readonly string StorageConnString = "DefaultEndpointsProtocol=https;AccountName=upstreamconsultingblob;AccountKey=9PC0UyBVwsYKQyVlDeJ9fLBoKYa7M55cHuDEE6nJ0Ra9t6ON80ydPqRlPnwSvfGgvCeFReUuKg0k+AStZBX4bg==;EndpointSuffix=core.windows.net";
    private static readonly string StorageContainerName = "documents";

    public BlobStorageService(string connectionString)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    // Uploads a file to Blob Storage
    public async Task<bool> UploadFileAsync(string fileName, Stream content, DateTime timestamp)
    {
        var blobClient = GetBlobClient(fileName, timestamp);
        await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = "application/octet-stream" });
        return await blobClient.ExistsAsync();
    }

    // Downloads a file from Blob Storage
    public async Task<Stream> DownloadFileAsync(string fileName, DateTime timestamp)
    {
        var blobClient = GetBlobClient(fileName, timestamp);
        if (await blobClient.ExistsAsync())
        {
            var download = await blobClient.DownloadAsync();
            return download.Value.Content;
        }
        return null;
    }

    // Constructs a BlobClient for a given filename and timestamp
    private BlobClient GetBlobClient(string fileName, DateTime timestamp)
    {
        string blobName = ConstructBlobName(fileName, timestamp);
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(StorageContainerName);
        return containerClient.GetBlobClient(blobName);
    }

    // Constructs a blob name based on a naming convention
    private string ConstructBlobName(string fileName, DateTime timestamp)
    {
        // Example structure: "2023/04/example_document.pdf"
        // You can adjust the structure according to your needs
        string blobName = $"{timestamp:yyyy}/{timestamp:MM}/{fileName}";
        return blobName;
    }
}
