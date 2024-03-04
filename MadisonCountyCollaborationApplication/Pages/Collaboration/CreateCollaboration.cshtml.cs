using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class CreateCollaborationModel : PageModel
    {
        [BindProperty]
        public MadisonCountyCollaborationApplication.Pages.DataClasses.Collaboration NewC { get; set; }
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
        public IActionResult OnPostCreate()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            DBClass.CreateCollab(NewC);
            DBClass.MainDBconnection.Close();
            return RedirectToPage("../Home");
        }

        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            NewC = new MadisonCountyCollaborationApplication.Pages.DataClasses.Collaboration();
            return Page();
        }
        public IActionResult OnPostPopulate()
        {
            NewC = new DataClasses.Collaboration
            {
                collabName = "Test Name for New Collab",
                notesAndInfo = "Test notes for New Collab",
                collabType = "Public",
                dateCreated = DateTime.Now
            };
            return Page();
        }
    }
}
