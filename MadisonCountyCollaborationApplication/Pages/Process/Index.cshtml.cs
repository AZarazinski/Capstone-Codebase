using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System;
using Azure.Storage.Blobs;
using System.Globalization;

namespace MadisonCountyCollaborationApplication.Pages.Process
{
    public class IndexModel : PageModel
    {
        public string ProcessName { get; set; }
        [BindProperty]
        public int ProcessID { get; set; }

        public string? CurrentProcessFolderName { get; set; }
        [BindProperty]
        public IFormFile File { get; set; }

        [BindProperty]
        public string FileTypeOptions { get; set; }
        private readonly WhiteListService _whitelistService;
        private readonly BlobServiceClient _blobServiceClient;


        private static readonly string StorageConnString = "DefaultEndpointsProtocol=https;AccountName=upstreamconsultingblob;AccountKey=9PC0UyBVwsYKQyVlDeJ9fLBoKYa7M55cHuDEE6nJ0Ra9t6ON80ydPqRlPnwSvfGgvCeFReUuKg0k+AStZBX4bg==;EndpointSuffix=core.windows.net";
        private static readonly string StorageContainerName = "documents";

        // Single constructor that takes both dependencies
        public IndexModel(WhiteListService whitelistService, BlobServiceClient blobServiceClient)
        {
            _whitelistService = whitelistService;
            _blobServiceClient = blobServiceClient;
        }

        public List<string> Whitelist { get; private set; }
        public IActionResult OnGet(int? processID)
        {
            // Attempt to get ProcessID from the route first.
            if (processID.HasValue)
            {
                ProcessID = processID.Value;
                HttpContext.Session.SetInt32("processID", ProcessID);
                Whitelist = _whitelistService.GetWhitelist();
                foreach (var element in Whitelist)
                {
                    Console.WriteLine(element.ToString());
                }
            }
            else
            {
                // Attempt to get ProcessID from the session.
                ProcessID = HttpContext.Session.GetInt32("processID") ?? 0;
            }

            // If no valid ProcessID is found, return an error message.
            if (ProcessID <= 0)
            {
                ViewData["ErrorMessage"] = "Invalid Process ID. Please select a process.";
                return Page();
            }

            // Ensure the user is logged in by checking the session.
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
            {
                HttpContext.Session.SetString("LoginError", "You must log in to access that page!");
                return RedirectToPage("/User/Login");
            }

            // Fetch the process name using the updated DBClass method that manages connections safely.
            ProcessName = DBClass.ProcessGetName(ProcessID);

            if (string.IsNullOrEmpty(ProcessName))
            {
                ViewData["ErrorMessage"] = "Process not found.";
                return Page();
            }

            // Optionally, store the current process name in the session for later use if needed.
            HttpContext.Session.SetString("processName", ProcessName);

            // No errors, so return the page to the user.
            ViewData["LoginMessage"] = "Login successful!";
            CurrentProcessFolderName = ProcessName.Replace(" ", "_"); // Modify as needed for your folder naming convention.
            return Page();
        }

        







        public async Task<IActionResult> OnPostUploadAsync(IFormFile fileUpload)
        {
            var fileTypeOption = Request.Form["FileTypeOptions"];
            Console.WriteLine($"Selected File Type Option: {fileTypeOption}");
            // Check if there's a file to upload
            if (fileUpload == null || fileUpload.Length <= 0)
            {
                ModelState.AddModelError("", "Please select a file to upload.");
                return Page();
            }

            // Retrieve CollaborationID from session
            int? processID = HttpContext.Session.GetInt32("processID");
            if (processID == null)
            {
                ModelState.AddModelError("", "Collaboration ID is missing. Please select a collaboration first.");
                return Page();
            }

            var fileName = Path.GetFileName(fileUpload.FileName);
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            int userID = Convert.ToInt32(DBClass.UserNameIDConverter(HttpContext.Session.GetString("username")));
            // Inserting FileUpload info into the Document and ProcessDocument Tables HERE
            if (fileExtension == ".csv")
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fileupload", fileName);
                // Insert into the DB Document and ProcessDocument tables
                DBClass.MainDBconnection.Close();
                string queryName = Path.GetFileNameWithoutExtension(fileName);



                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                    return RedirectToPage("FileHandling", new { filePath = filePath });
                }
            }
            // Handle PDF, DOCX, PNG files
            else if (fileExtension == ".pdf" || fileExtension == ".docx" || fileExtension == ".png")
            {
                string dateTimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string newFileName = dateTimeString + "_" + fileName;

                DBClass.InsertIntoDocumentTable(newFileName, fileTypeOption, userID, processID);
                DBClass.MainDBconnection.Close();


                await UploadToBlobStorage(containerName: "documents", localFileName: newFileName, fileStream: fileUpload.OpenReadStream());

                return RedirectToPage("./Index", new { ProcessID = ProcessID });
            }
            else
            {
                // Handle unsupported file types
                ModelState.AddModelError("", "Unsupported file type. Only .csv, .pdf, .docx, and .png files are supported.");
                return Page();
            }
        }

        private async Task UploadToBlobStorage(string containerName, string localFileName, Stream fileStream)
        {
            // Construct blob client
            BlobServiceClient blobServiceClient = new BlobServiceClient(StorageConnString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);


            // Get blob reference
            BlobClient blobClient = containerClient.GetBlobClient(localFileName);

            // Upload file with metadata
            await blobClient.UploadAsync(fileStream);
        }


        private BlobClient GetBlobClient(string fullDocumentName)
        {
            // Assuming _blobServiceClient is already set up with your Azure Storage connection string
            var containerClient = _blobServiceClient.GetBlobContainerClient(StorageContainerName);
            return containerClient.GetBlobClient(fullDocumentName);
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

                TempData["ErrorMessage"] = "Document not found in Blob Storage.";
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return Page();
            }
        }


        public async Task<IActionResult> OnPostDownloadDatasetAsync(string datasetName)
        {
            try
            {
                // Use the literal document name from the database, which includes the timestamp
                var blobClient = GetBlobClient(datasetName);
                if (await blobClient.ExistsAsync())
                {
                    try
                    {
                        var download = await blobClient.DownloadAsync();
                        var stream = download.Value.Content;
                        var contentType = "application/octet-stream"; // Consider setting the actual content type if known.
                        return File(stream, contentType, datasetName);
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

                TempData["ErrorMessage"] = "Document not found in Blob Storage.";
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return Page();
            }
        }

    }
}