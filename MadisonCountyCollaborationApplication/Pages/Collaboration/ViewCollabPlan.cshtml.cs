using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class ViewCollabPlanModel : PageModel
    {
        [BindProperty]
        public int PlanID { get; set; }
        [BindProperty]
        public int UserID { get; set; }
        [BindProperty]
        public int CollaborationID { get; set; }
        public List<Contents> ContentList { get; set; }

        public ViewCollabPlanModel()
        {
            ContentList = new List<Contents>();
        }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                int PlanID = (int)HttpContext.Session.GetInt32("planID");
                SqlDataReader ContentsReader = DBClass.ContentsReader(PlanID);
                while (ContentsReader.Read())
                {
                    ContentList.Add(new Contents
                    {
                        planStep = Int32.Parse(ContentsReader["step"].ToString()),
                        planContents = ContentsReader["contents"].ToString(),
                    });
                }

                // Close your connection in DBClass
                DBClass.MainDBconnection.Close();
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/User/Login");
            }
        }

        public IActionResult OnPostAddContents()
        {
            return RedirectToPage("/PlanContents");
        }
    }
}
