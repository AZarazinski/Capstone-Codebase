using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class AddPlanContentsModel : PageModel
    {
        [BindProperty]
        public MadisonCountyCollaborationApplication.Pages.DataClasses.Contents NewC { get; set; }

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
                return RedirectToPage("../User/Login");
            }
        }
        public IActionResult OnPostCreate()
        {
            NewC.planID = (int)HttpContext.Session.GetInt32("planID");
            NewC.planStep = NewC.sequenceNumber;

            if (NewC.planContents == null || NewC.sequenceNumber == 0)
            {
                return Page();
            }

            DBClass.CreatePlanContents(NewC);
            DBClass.MainDBconnection.Close();
            return RedirectToPage("ViewCollabPlan");
        }
    }
}
