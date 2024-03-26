using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages.Process
{
    public class IndexModel : PageModel
    {
      
        public string CollabName { get; set; }
        [BindProperty]
        public int collabID { get; set; }

        public string? currentCollabFolderName { get; set; }

        

        public IActionResult OnGet()
        {
            // Attempt to get collabID from the session.
            collabID = HttpContext.Session.GetInt32("collaborationID") ?? 0;
            if (collabID <= 0)
            {
                // Handle the case where collabID is not found or invalid.
                ViewData["ErrorMessage"] = "Invalid Collaboration ID. Please select a collaboration.";
                return Page(); // Or consider redirecting to an error page or listing page.
            }

            if (HttpContext.Session.GetString("username") == null)
            {
                // If not logged in, redirect to the login page.
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/User/Login");
            }

            ViewData["LoginMessage"] = "Login for " + HttpContext.Session.GetString("username") + " successful!";

            try
            {
                // Fetch the collaboration name
                SqlDataReader CollaborationGetName = DBClass.CollaborationGetName(collabID);
                if (CollaborationGetName.Read())
                {
                    CollabName = CollaborationGetName["collabName"].ToString();
                }
                CollaborationGetName.Close(); // Ensure you close the reader to free up resources

                
                

            }
            finally
            {
                DBClass.MainDBconnection.Close(); // Ensure the database connection is closed after operation
            }

            


            return Page();
        }


        public async Task<IActionResult> OnPostUploadAsync(IFormFile fileUpload)
        {
            // Check if there's a file to upload
            if (fileUpload == null || fileUpload.Length <= 0)
            {
                ModelState.AddModelError("", "Please select a file to upload.");
                return Page();
            }

            // Retrieve CollaborationID from session
            int? collabID = HttpContext.Session.GetInt32("collaborationID");
            if (collabID == null)
            {
                ModelState.AddModelError("", "Collaboration ID is missing. Please select a collaboration first.");
                return Page();
            }

            var fileName = Path.GetFileName(fileUpload.FileName);
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

            // Handle CSV files separately
            if (fileExtension == ".csv")
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fileupload", fileName);
                string queryName = Path.GetFileNameWithoutExtension(fileName);

                string sqlQuery = $"SELECT COUNT(*) FROM DataSets WHERE dataSetName = '{queryName}';";
                using (var reader = DBClass.GeneralReaderQuery(sqlQuery))
                {
                    if (reader.Read() && (int)reader[0] > 0)
                    {
                        ViewData["DatasetError"] = "This Dataset already exists. Please upload a new one";
                        return Page();
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

                string folderName = folders[collabID.Value]; // Assuming collabID is always valid here
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", folderName, fileName);

                var directory = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(directory); // CreateDirectory is a no-op if the directory already exists

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(fileStream);
                }
                //Area to add SUCCESS Message!!

                return RedirectToPage("./Index", new { collabID = collabID.Value });
            }
            else
            {
                // Handle unsupported file types
                ModelState.AddModelError("", "Unsupported file type. Only .csv, .pdf, .docx, and .png files are supported.");
                return Page();
            }
        }








        public IActionResult OnPostHome()
        {
            return RedirectToPage("../Home");
        }

        public IActionResult OnPostAddUsers()
        {

            return RedirectToPage("AddEditUsers");
        }



        public IActionResult OnPostDatasets()
        {
            return RedirectToPage("ViewCollabData");
        }
        //public IActionResult OnPostAutoPopulateValues()
        //{
        //    PlanID = 1;
        //    return Page();
        //}
        //public IActionResult OnPostClearInputs()
        //{
        //    PlanID = 0;
        //    return Page();
        //}

    }
}
