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

        public IActionResult OnPostHome()
        {
            return RedirectToPage("/HomePage");
        }
        public IActionResult OnPostAddPlan()
        {
            return RedirectToPage("/AddCollabPlan");
        }
        public IActionResult OnPostAddUsers()
        {

            return RedirectToPage("/AddEditUsers");
        }
        public IActionResult OnPostMessage()
        {
            return RedirectToPage("/Message");
        }
        public IActionResult OnPostPlan()
        {
            HttpContext.Session.SetInt32("planID", PlanID);
            return RedirectToPage("/ViewCollabPlan");

        }
        public IActionResult OnPostKnowledge()
        {
            return RedirectToPage("/ViewCollabKnowledge");
        }
        public IActionResult OnPostDatasets()
        {
            return RedirectToPage("/ViewCollabData");
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
            return RedirectToPage("/ViewCollabPlan");

        }
        public IActionResult OnPostGenerateReport()
        {

            return RedirectToPage("/GenerateReport");
        }
    }
}
