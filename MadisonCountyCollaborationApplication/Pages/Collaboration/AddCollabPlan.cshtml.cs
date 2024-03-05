using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class AddCollabPlanModel : PageModel
    {
        [BindProperty]
        public MadisonCountyCollaborationApplication.Pages.DataClasses.Plans NewP { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/User/Login");
            }
        }
        //creating new plan
        public IActionResult OnPostCreate()
        {
            NewP.collabID = (int)HttpContext.Session.GetInt32("collaborationID");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            DBClass.CreatePlan(NewP);
            DBClass.MainDBconnection.Close();
            return RedirectToPage("CollaborationLanding");
        }
        public IActionResult OnPostPopulate()
        {
            NewP = new DataClasses.Plans
            {
                planName = "Testing Title",
                planDesc = "Subject1",
                dateCreated = DateTime.Now
            };

            return Page();
        }
        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            NewP = new MadisonCountyCollaborationApplication.Pages.DataClasses.Plans();
            return Page();
        }
    }
}
