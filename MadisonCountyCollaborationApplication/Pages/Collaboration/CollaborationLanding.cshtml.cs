using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountyCollaborationApplication.Pages.DB;
using MadisonCountyCollaborationApplication.Pages.DataClasses;
using System.Data.SqlClient;


namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class CollaborationLandingModel : PageModel
    {
        [BindProperty]
        public int PlanID { get; set; }
        public string CollabName { get; set; }
        public int CollabID { get; set; }

        public List<Plans> PlansList { get; set; }

        public CollaborationLandingModel()
        {
            PlansList = new List<Plans>();
        }

        public IActionResult OnGet(int collabID)
        {

            if (HttpContext.Session.GetString("username") != null)
            {

                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                //plan table
                collabID = (int)HttpContext.Session.GetInt32("collaborationID");
                SqlDataReader PlansReader = DBClass.PlansReader(collabID);
                while (PlansReader.Read())
                {
                    PlansList.Add(new Plans
                    {
                        planID = Int32.Parse(PlansReader["planID"].ToString()),
                        planName = PlansReader["planName"].ToString(),
                        planDesc = PlansReader["planDesc"].ToString()
                        //dateCreated = PlansReader["dateCreated"].ToString()
                    });
                }

                // Close connection in DBClass
                DBClass.MainDBconnection.Close();
                // Add a method to fetch the collaboration details by ID

                SqlDataReader CollaborationGetName = DBClass.CollaborationGetName(collabID);
                while (CollaborationGetName.Read())
                {
                    CollabName = CollaborationGetName["collabName"].ToString();
                }
                DBClass.MainDBconnection.Close(); // Close the connection when done
                return Page();

            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/User/Login");
            }
        }

        public async Task<IActionResult> OnPostUploadAsync(IFormFile fileUpload)
        {
            // Check if there's a file to upload
            if (fileUpload != null && fileUpload.Length > 0)
            {
                // Retrieve CollaborationID from session
                int? collabID = HttpContext.Session.GetInt32("collaborationID");
                if (collabID == null)
                {
                    // Handle the case where CollaborationID is not found in session
                    // You might want to redirect the user or show an error message
                    ModelState.AddModelError("", "Collaboration ID is missing. Please select a collaboration first.");
                    return Page();
                }

                var fileName = Path.GetFileName(fileUpload.FileName);
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

                // Handle different file types based on extension
                if (fileExtension == ".csv")
                {

                    //var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\fileupload\" + fileName;

                    //using (var stream = new FileStream(filePath = filePath))
                    //{
                    //    fileName.CopyTo(stream);
                    //}
                    //return RedirectToPage("FileHandling", new {filePath = filePath })
                    
                }
                else if (fileExtension == ".pdf")
                {
                    // Retrieve CollaborationID from session
                    int CollabID = HttpContext.Session.GetInt32("collaborationID").Value;

                    // Define the folder names associated with collaboration IDs
                    var folders = new Dictionary<int, string>
                    {
                        { 3, "Admin" },
                        { 1, "Budgeting" },
                        { 5, "Economic" },
                        { 4, "Management" },
                        { 2, "Revenue" }
                    };

                    // Determine the correct folder name based on the collaboration ID
                    string folderName = folders[CollabID];

                    // Construct the path where the file should be saved
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Documents", folderName, fileName);

                    // Ensure the directory exists
                    var directory = Path.GetDirectoryName(filePath);
                    Directory.CreateDirectory(directory); // CreateDirectory is a no-op if the directory already exists

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.CopyToAsync(fileStream);
                    }

                    // Optionally, add a message or logic after successfully saving the file
                    // ...
                    return Page(); // Or redirect to another page as needed
                }
                else if (fileExtension == ".docx")
                {
                    // Handle DOCX file specific to collabID
                }
                else
                {
                    // Handle unsupported file types or set an error message
                    ModelState.AddModelError("", "Unsupported file type.");
                    return Page();
                }

                // Optionally save the file or process it as needed
                // Make sure to associate it with collabID in your storage or database
            }
            else
            {
                // Handle the case where no file was uploaded
                ModelState.AddModelError("", "Please select a file to upload.");
                return Page();
            }

            return RedirectToPage(); // Or redirect to another page as needed
        }


        public IActionResult OnPostHome()
        {
            return RedirectToPage("../Home");
        }
        public IActionResult OnPostAddPlan()
        {
            return RedirectToPage("AddCollabPlan");
        }
        public IActionResult OnPostAddUsers()
        {

            return RedirectToPage("AddEditUsers");
        }
        
        public IActionResult OnPostPlan()
        {
            HttpContext.Session.SetInt32("planID", PlanID);
            return RedirectToPage("ViewCollabPlan");

        }
        
        public IActionResult OnPostDatasets()
        {
            return RedirectToPage("ViewCollabData");
        }
        public IActionResult OnPostAutoPopulateValues()
        {
            PlanID = 1;
            return Page();
        }
        public IActionResult OnPostClearInputs()
        {
            PlanID = 0;
            return Page();
        }
        public IActionResult OnPostAsync(int planID)
        {
            HttpContext.Session.SetInt32("planID", PlanID);
            return RedirectToPage("ViewCollabPlan");

        }
        
    }
}
