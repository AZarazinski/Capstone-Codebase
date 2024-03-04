using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountyCollaborationApplication.Pages.DB;
using MadisonCountyCollaborationApplication.Pages.DataClasses;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class AddEditUsersModel : PageModel
    {
        [BindProperty]
        public MadisonCountyCollaborationApplication.Pages.DataClasses.Contribute NewA { get; set; }
        public String? firstName { get; set; }
        public String? lastName { get; set; }
        public String? email { get; set; }
        public String? userName { get; set; }
        public int userID { get; set; }
        public int collabID { get; set; }
        public String? collabName { get; set; }

        public List<DataClasses.Users> UserList { get; set; }
        public AddEditUsersModel()
        {
            UserList = new List<DataClasses.Users>();
        }
        //reading users
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";
                collabID = (int)HttpContext.Session.GetInt32("collaborationID");

                SqlDataReader CollaborationGetName = DBClass.CollaborationGetName(collabID);
                while (CollaborationGetName.Read())
                {
                    collabName = CollaborationGetName["collabName"].ToString();
                }
                DBClass.MainDBconnection.Close(); // Close the connection when done
                SqlDataReader AddUsersReader = DBClass.AddUsersReader();
                UserList.Clear();
                while (AddUsersReader.Read())
                {
                    UserList.Add(new DataClasses.Users
                    {
                        firstName = AddUsersReader["firstName"].ToString(),
                        lastName = AddUsersReader["lastName"].ToString(),
                        email = AddUsersReader["email"].ToString(),
                        userName = AddUsersReader["userName"].ToString(),
                    });
                }

                // Close connection in DBClass
                DBClass.MainDBconnection.Close();
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/User/Login");
            }
        }
        //adding user
        public IActionResult OnPostAddUserToCollab(String input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                userID = DBClass.GetUserIDFromUserNameOrEmail(input);
                DBClass.MainDBconnection.Close();
                if (userID > 0)
                {
                    collabID = (int)HttpContext.Session.GetInt32("collaborationID");
                    if (DBClass.UserExistInCollab(userID, collabID))
                    {
                        DBClass.MainDBconnection.Close();

                        if (DBClass.AddUserToCollab(userID, collabID))
                        {
                            DBClass.MainDBconnection.Close();
                            TempData["AddUserToCollabMSG"] = "User added successfully."; // On successful addition
                            TempData["MessageType"] = "Success";
                            //ViewData["AddUserToCollabMSG"] = $"{input} added to collaboration.";
                            return OnGet();
                        }
                        else
                        {
                            DBClass.MainDBconnection.Close();
                            TempData["AddUserToCollabMSG"] = "Could not add the user."; // On failure
                            TempData["MessageType"] = "Error";
                            //ViewData["AddUserToCollabMSG"] = $"Action could not be completed.";
                            return OnGet();
                        }
                    }
                    else
                    {
                        DBClass.MainDBconnection.Close();
                        TempData["AddUserToCollabMSG"] = $"{input} has already joined the collaboration."; //failure
                        TempData["MessageType"] = "Success";
                        return OnGet();
                    }
                }
                else
                {
                    TempData["AddUserToCollabMSG"] = $"{input} does not exist."; //failure
                    TempData["MessageType"] = "Error";

                    return OnGet();
                }
            }
            else
            {
                TempData["AddUserToCollabMSG"] = $"Must enter a username/email."; //null
                TempData["MessageType"] = "Error";
                return OnGet();
            }

        }
    }
}
