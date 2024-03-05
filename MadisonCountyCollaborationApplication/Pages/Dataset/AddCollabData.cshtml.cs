using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class AddCollabDataModel : PageModel
    {
        [BindProperty]
        public DataClasses.DatasetAssist NewD { get; set; }
        public List<DataClasses.Collaboration> CollaborationList { get; set; }


        public AddCollabDataModel()
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
                        collabID = int.Parse(CollaborationReader["collabID"].ToString()),
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
            if (NewD.collabID != null && NewD.collabID > 0)
            {
                if (DBClass.CollaborationExist(NewD.collabID))
                {
                    DBClass.MainDBconnection.Close();
                    HttpContext.Session.SetInt32("collaborationID", NewD.collabID);
                    DBClass.MainDBconnection.Close();
                    NewD.datasetID = (int)HttpContext.Session.GetInt32("datasetID");
                    DBClass.CreateDataAssist(NewD);
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
