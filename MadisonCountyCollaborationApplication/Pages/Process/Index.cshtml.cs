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
        public IndexModel(WhiteListService whitelistService)
        {
            _whitelistService = whitelistService;
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

                string sqlQuery = $"SELECT COUNT(*) FROM DataSet WHERE dataSetName = '{queryName}';";
                using (var reader = DBClass.GeneralReaderQuery(sqlQuery))
                {
                    if (reader.Read() && (int)reader[0] > 0)
                    {
                        ViewData["DatasetError"] = "This Dataset already exists. Please upload a new one";
                        return RedirectToPage("Index", new { processID = processID });
                    }
                    else
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileUpload.CopyToAsync(stream);
                        }

                        return RedirectToPage("FileHandling", new { filePath = filePath });
                    }
                }
            }
            // Handle PDF, DOCX, PNG files
            else if (fileExtension == ".pdf" || fileExtension == ".docx" || fileExtension == ".png")
            {
                var folders = new Dictionary<int, string>
                {
                    { 3, "Admin" },
                    { 1, "Budgeting" },
                    { 5, "Economic" },
                    { 4, "Management" },
                    { 2, "Revenue" }
                };

                string folderName = folders[processID.Value]; // Assuming collabID is always valid here
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", folderName, fileName);
                // Insert into the DB Document and ProcessDocument tables
                DBClass.InsertIntoDocumentTable(Path.GetFileNameWithoutExtension(fileUpload.FileName), fileTypeOption, userID, processID);
                DBClass.MainDBconnection.Close();


                var directory = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(directory); // CreateDirectory is a no-op if the directory already exists

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(fileStream);
                }
                //Area to add SUCCESS Message!!
                Console.WriteLine($"Selected File Type Option: {FileTypeOptions}");


                return RedirectToPage("./Index", new { processID = processID.Value });
            }
            else
            {
                // Handle unsupported file types
                ModelState.AddModelError("", "Unsupported file type. Only .csv, .pdf, .docx, and .png files are supported.");
                return Page();
            }
        }


    }
}