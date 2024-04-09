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
        [BindProperty(SupportsGet = true)]
        public int DocumentID { get; set; }
        [BindProperty(SupportsGet = true)]
        public string DocumentName { get; set; }
        [BindProperty(SupportsGet = true)]
        public string UserFullName { get; set; }
        [BindProperty(SupportsGet = true)]
        public string DocumentType { get; set; }
        [BindProperty(SupportsGet = true)]
        public string UserName { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? DateFrom { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime? DateTo { get; set; }
        [BindProperty]
        public List<string> Types { get; set; }
        public List<Document> Documents { get; set; } = new List<Document>();
        public string ProcessName { get; set; }
        [BindProperty]
        public int ProcessID { get; set; }

        public string? CurrentProcessFolderName { get; set; }
        [BindProperty]
        public IFormFile File { get; set; }

        [BindProperty]
        public string FileTypeOptions { get; set; }
        public bool isPublic { get; set; }

        [BindProperty]
        public List<string> WhiteList { get; private set; } = new List<string>
        {
            ".csv", // Comma-Separated Values
            ".pdf", // Portable Document Format
            ".docx", // Microsoft Word Document
            ".doc", // Microsoft Word Document
            ".png", // Portable Network Graphics
            ".jpg",  // JPEG Image
            ".jpeg", // JPEG Image
        };

        private readonly WhiteListService _whitelistService;
        private readonly BlobServiceClient _blobServiceClient;



        private static readonly string StorageConnString = "DefaultEndpointsProtocol=https;AccountName=countyconnectstorage;AccountKey=IezvDEewuBWkAsIqls+LxrQUT3OJVxayH/hq4cNwbsP2bEAZDNPFDSQHScxCEZ2dRcBDw+b2PioZ+AStlmc6Xg==;EndpointSuffix=core.windows.net";
        private readonly string DocumentsContainerName = "documents";
        private readonly string DatasetsContainerName = "datasets"; // Assuming datasets are stored here


        // Single constructor that takes both dependencies
        public IndexModel(WhiteListService whitelistService, BlobServiceClient blobServiceClient)
        {
            _whitelistService = whitelistService;
            _blobServiceClient = blobServiceClient;
        }

        public List<string> Whitelist { get; private set; }
        public IActionResult OnGet(int? processID, string? DocumentName, string? DocumentType, string? UserFullName, DateTime? DateFrom, DateTime? DateTo)
        {
            // Attempt to get ProcessID from the route first.
            if (processID.HasValue)
            {

                ProcessID = processID.Value;

                Types = DBClass.GetAllUniqueDocumentTypesForProcess(ProcessID);
                DBClass.MainDBconnection.Close();

                HttpContext.Session.SetInt32("processID", ProcessID); // Store ProcessID in session.

                // Assuming Whitelist is a property of your PageModel and you want to load it here.
                Whitelist = _whitelistService.GetWhitelist(); // Load some whitelist information.
                foreach (var element in Whitelist)
                {
                    Console.WriteLine(element.ToString()); // Example processing.
                }
            }
            else
            {
                // Attempt to get ProcessID from the session if not in route.
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

            // Fetch the process name using the updated DBClass method that safely manages connections.
            ProcessName = DBClass.ProcessGetName(ProcessID);

            if (string.IsNullOrEmpty(ProcessName))
            {
                ViewData["ErrorMessage"] = "Process not found.";
                return Page();
            }
            Types = DBClass.GetAllUniqueDocumentTypesForProcess(ProcessID);
            DBClass.MainDBconnection.Close();
            LoadDocuments(DocumentName, DocumentType, UserFullName, DateFrom, DateTo);
            DBClass.MainDBconnection.Close();
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

                string dateTimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string newFileName = dateTimeString + "_" + fileName;

                await UploadToBlobStorage(containerName: "datasets", localFileName: newFileName, fileStream: fileUpload.OpenReadStream());

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                    return RedirectToPage("FileHandling", new { filePath = filePath, newFileName = newFileName });
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

                return RedirectToPage("./Index", new { ProcessID = processID });
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


        public async Task<IActionResult> OnPostDownloadDatasetAsync(string datasetName)
        {
            try
            {
                // Directly use the dataset container name instead of fetching from Configuration
                string datasetContainerName = "datasets"; // This is statically defined, similar to your documents approach
                var containerClient = _blobServiceClient.GetBlobContainerClient(datasetContainerName);
                var blobClient = containerClient.GetBlobClient(datasetName);

                if (await blobClient.ExistsAsync())
                {
                    var download = await blobClient.DownloadAsync();
                    var stream = download.Value.Content;
                    var contentType = "application/octet-stream"; // Consider setting the actual content type if known.
                    return File(stream, contentType, datasetName); // Successfully return the file for download
                }
                else
                {
                    TempData["ErrorMessage"] = "Dataset not found in Blob Storage.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return Page();
            }
        }



        public IActionResult OnPostSearch()
        {
            // Assuming you have validated your input and it's safe to use
            return RedirectToPage(new
            {
                DocumentName = DocumentName,
                DocumentType = DocumentType,
                UserName = UserName,
                DateFrom = DateFrom?.ToString("yyyy-MM-dd"),
                DateTo = DateTo?.ToString("yyyy-MM-dd"),
            });
        }
        public IActionResult OnPostClear()
        {
            return RedirectToPage();
        }

        public IActionResult OnPostPublish(int documentID)
        {
            DBClass.SetDocumentPublic(documentID);
            return RedirectToPage();
        }
        public IActionResult OnPostUnpublish(int documentID)
        {
            // Potentially the same logic here, or different, depending on your application's needs
            DBClass.SetDocumentPrivate(documentID);
            return RedirectToPage();
        }

        private void LoadDocuments(string? documentName, string? documentType, string? userFullName, DateTime? dateFrom, DateTime? dateTo)
        {
            Documents.Clear(); // Clear existing items

            var parameters = new Dictionary<string, object>
    {
        { "@DocumentName", string.IsNullOrEmpty(documentName) ? DBNull.Value : documentName.ToUpper() },
        { "@DocumentType", string.IsNullOrEmpty(documentType) ? DBNull.Value : documentType },
        { "@userFullName", string.IsNullOrEmpty(userFullName) ? DBNull.Value : userFullName.ToUpper() }, // Convert to upper case here if needed
        { "@DateFrom", dateFrom.HasValue ? (object)dateFrom.Value : DBNull.Value },
        { "@DateTo", dateTo.HasValue ? (object)dateTo.Value : DBNull.Value },
    };

            string sqlQuery = @"
                        SELECT
                            d.documentID,
                            d.documentName,
                            STUFF(d.documentName, 1, CHARINDEX('_', d.documentName, CHARINDEX('_', d.documentName) + 1), '') AS displayDocName,
                            d.documentType,
                            d.dateCreated,
                            u.userName,
                            u.lastName + ', ' + u.firstName AS userFullName,
                            d.isPublic
                        FROM
                            Document d
                            JOIN Users u ON d.userID = u.userID
                            JOIN DocumentProcess docP ON d.documentID = docP.documentID
                            JOIN Process p ON p.processID = docP.processID
                        WHERE
                            (@DocumentName IS NULL OR UPPER(d.documentName) LIKE '%' + UPPER(@DocumentName) + '%')
                            AND (@DocumentType IS NULL OR d.documentType = @DocumentType)
                            AND (@DateFrom IS NULL OR d.dateCreated >= @DateFrom)
                            AND (@DateTo IS NULL OR d.dateCreated <= @DateTo)
                            AND ((@userFullName IS NULL OR UPPER(u.lastName) LIKE '%' + UPPER(@userFullName) + '%')
                            OR (@userFullName IS NULL OR UPPER(u.firstName) LIKE '%' + UPPER(@userFullName) + '%'));";


            using (var reader = DBClass.GeneralReaderQueryWithParameters(sqlQuery, parameters))
            {
                while (reader.Read())
                {
                    var document = new Document
                    {
                        documentID = reader.GetInt32(reader.GetOrdinal("documentID")), // "d." prefix not needed here
                        documentName = reader.IsDBNull(reader.GetOrdinal("documentName")) ? null : reader.GetString(reader.GetOrdinal("documentName")),
                        displayDocName = reader.IsDBNull(reader.GetOrdinal("displayDocName")) ? null : reader.GetString(reader.GetOrdinal("displayDocName")),
                        documentType = reader.IsDBNull(reader.GetOrdinal("documentType")) ? null : reader.GetString(reader.GetOrdinal("documentType")),
                        userFullName = reader.IsDBNull(reader.GetOrdinal("userFullName")) ? null : reader.GetString(reader.GetOrdinal("userFullName")),
                        isPublic = reader.GetBoolean(reader.GetOrdinal("isPublic")),
                        dateCreated = reader.GetDateTime(reader.GetOrdinal("dateCreated")),
                        //userID = reader.GetInt32(reader.GetOrdinal("userID")) // Assuming you have a UserID field
                    };

                    Documents.Add(document);
                }
            }
        }
    }

}