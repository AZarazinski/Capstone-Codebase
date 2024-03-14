using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadisonCountyCollaborationApplication.Pages.User
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if (Request.Form["submitAction"] == "LogOut")
            {
                HttpContext.Session.Clear();
                return RedirectToPage("Login");
            }
            else if (Request.Form["submitAction"] == "Stay")
            {
                return RedirectToPage("../Home");
            }

            return Page(); // Default in case of unexpected errors
        }
        
    }
}
