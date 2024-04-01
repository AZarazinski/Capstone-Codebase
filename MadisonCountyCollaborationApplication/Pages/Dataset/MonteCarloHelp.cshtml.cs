using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadisonCountyCollaborationApplication.Pages
{
    public class MonteCarloHelpModel : PageModel
    {

        [BindProperty]
        public string ProcessName { get; set; }
        [BindProperty]
        public string DatasetName { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                //get process name
                ProcessName = HttpContext.Session.GetString("processName");

                //get dataset name
                DatasetName = HttpContext.Session.GetString("datasetName");

                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login");
            }
        }
    }
}
