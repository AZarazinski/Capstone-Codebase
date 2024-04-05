using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

public class BlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _documentContainerName;
    private readonly string _datasetContainerName;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _blobServiceClient = new BlobServiceClient(connectionString);
        _documentContainerName = configuration["AzureBlobStorage:DocumentContainerName"];
        _datasetContainerName = configuration["AzureBlobStorage:DatasetContainerName"];
    }

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
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_documentContainerName);
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
