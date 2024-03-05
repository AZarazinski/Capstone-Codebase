using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages
{
    public class HomeModel : PageModel
    {
        [BindProperty]
        [Required()]
        public int CollaborationID { get; set; }

        public IActionResult OnPostHome()
        {
            return RedirectToPage("/HomePage");
        }
        public IActionResult OnPostCreate()
        {
            return RedirectToPage("/Collaboration/CreateCollaboration");
        }
        public IActionResult OnPostSelect()
        {
            if (CollaborationID != null && CollaborationID > 0)
            {
                if (DBClass.CollaborationExist(CollaborationID))
                {
                    HttpContext.Session.SetInt32("collaborationID", CollaborationID);
                    DBClass.MainDBconnection.Close();
                    return RedirectToPage("/Collaboration/CollaborationLanding");

                }
                else
                {
                    DBClass.MainDBconnection.Close();
                    ViewData["CollabNotExistMessage"] = "That collaboration does not exist.";
                    return OnGet();
                }

            }
            else
            {
                ViewData["CollabNotExistMessage"] = "That collaboration does not exist.";
                return OnGet();
            }
        }
       

        
        

        public List<DataClasses.Collaboration> CollaborationList { get; set; }

        public HomeModel()
        {
            CollaborationList = new List<DataClasses.Collaboration>();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                //collab table
                SqlDataReader CollaborationReader = DBClass.CollaborationReader();
                while (CollaborationReader.Read())
                {
                    CollaborationList.Add(new DataClasses.Collaboration
                    {
                        collabID = Int32.Parse(CollaborationReader["collabID"].ToString()),
                        collabName = CollaborationReader["collabName"].ToString(),
                        notesAndInfo = CollaborationReader["notesAndInfo"].ToString(),
                        collabType = CollaborationReader["collabType"].ToString()
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
    }
}
