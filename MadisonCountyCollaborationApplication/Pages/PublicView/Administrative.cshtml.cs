using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadisonCountyCollaborationApplication.Pages.PublicView
{
    public class AdministrativeModel : PageModel
    {
        public void OnGet()
        {
        }


        private static readonly string StorageConnString = "DefaultEndpointsProtocol=https;AccountName=countyconnectstorage;AccountKey=IezvDEewuBWkAsIqls+LxrQUT3OJVxayH/hq4cNwbsP2bEAZDNPFDSQHScxCEZ2dRcBDw+b2PioZ+AStlmc6Xg==;EndpointSuffix=core.windows.net";
        private readonly string DocumentsContainerName = "documents";
        private readonly string DatasetsContainerName = "datasets"; // Assuming datasets are stored here

        private BlobClient GetBlobClient(string documentName)
        {
            // Create a new BlobServiceClient using the storage connection string directly.
            // This mirrors the approach used in your upload method.
            BlobServiceClient blobServiceClient = new BlobServiceClient(StorageConnString);

            // Assuming DocumentsContainerName is a class member that holds the name of your documents container.
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(DocumentsContainerName);

            // Return the BlobClient for the specified document.
            return containerClient.GetBlobClient(documentName);
        }

        public async Task<IActionResult> OnPostDownloadDocumentAsync(string documentName)
        {
            try
            {
                // Use the literal document name from the database, which includes the timestamp
                var blobClient = GetBlobClient(documentName);
                if (await blobClient.ExistsAsync())
                {
                    try
                    {
                        var download = await blobClient.DownloadAsync();
                        var stream = download.Value.Content;
                        var contentType = "application/octet-stream"; // Consider setting the actual content type if known.
                        return File(stream, contentType, documentName);
                    }
                    catch (Exception ex)
                    {
                        // Log the detailed exception
                        // Consider using a logging framework or storing the message in TempData
                        TempData["ErrorMessage"] = $"Error downloading the file: {ex.Message}";
                        return Page();
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Document not found in Blob Storage.";
                    return Page();
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return Page();
            }
        }

    }

}
