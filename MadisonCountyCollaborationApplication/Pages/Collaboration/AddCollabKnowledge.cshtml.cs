using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using MadisonCountyCollaborationApplication.Pages.DataClasses;

namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class AddCollabKnowledgeModel : PageModel
    {
        [BindProperty]
        public MadisonCountyCollaborationApplication.Pages.DataClasses.Assists NewA { get; set; }
        public List<DataClasses.Collaboration> CollaborationList { get; set; }


        public AddCollabKnowledgeModel()
        {
            CollaborationList = new List<DataClasses.Collaboration>();
        }
        //displaying collab table
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
        //selecting collab
        public IActionResult OnPostSelect()
        {
            if (NewA.collabID != null && NewA.collabID > 0)
            {
                if (DBClass.CollaborationExist(NewA.collabID))
                {
                    DBClass.MainDBconnection.Close();
                    HttpContext.Session.SetInt32("collaborationID", NewA.collabID);
                    DBClass.MainDBconnection.Close();
                    NewA.knowledgeID = (int)HttpContext.Session.GetInt32("knowledgeItemID");
                    DBClass.CreateAssist(NewA);
                    DBClass.MainDBconnection.Close();
                    return RedirectToPage("/Collaboration");

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
    }
}
