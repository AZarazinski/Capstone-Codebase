using MadisonCountyCollaborationApplication.Pages.DB;
using MadisonCountyCollaborationApplication.Pages.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MadisonCountyCollaborationApplication.Pages
{
    public class AdminDashboardModel : PageModel
    {
        [BindProperty(SupportsGet = false)]
        public DataClasses.Users NewUser { get; set; }

        [BindProperty(SupportsGet = false)]
        public DataClasses.UserProcess NewUserProcess { get; set; }

        [BindProperty(SupportsGet = false)]
        public string RemoveUserID { get; set; }

        [BindProperty(SupportsGet = false)]
        public string ProcessName { get; set; }

        [BindProperty(SupportsGet = false)]
        public string RemoveProcessID { get; set; }


        public List<SelectListItem> UserList { get; set; }

        public List<SelectListItem> ProcessList { get; set; }

        public List<SelectListItem> UserTypeList { get; set; }

       

    

        public IActionResult OnGetSessionCheck()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/User/Login");
            }
        }

        public IActionResult OnGet()
        {

            string username = HttpContext.Session.GetString("username");
            var userID = DBClass.UserNameIDConverter(username);

            bool isAdmin = DBClass.CheckAdmin(userID);

            if (isAdmin)
            {

                UserList = new List<SelectListItem>();

                using (var UserReader = DBClass.GeneralReaderQuery("SELECT * FROM Users"))
                {
                    while (UserReader.Read())
                    {
                        UserList.Add(new SelectListItem
                        {
                            Text = UserReader["userName"].ToString(),
                            Value = UserReader["userID"].ToString()
                        });
                    }
                }

                ProcessList = new List<SelectListItem>();

                using (var ProcessReader = DBClass.GeneralReaderQuery("SELECT * FROM Process"))
                {
                    while (ProcessReader.Read())
                    {
                        ProcessList.Add(new SelectListItem
                        {
                            Text = ProcessReader["processName"].ToString(),
                            Value = ProcessReader["processID"].ToString()
                        });
                    }
                }

                UserTypeList = new List<SelectListItem>();

                UserTypeList.Add(new SelectListItem
                {
                    Text = "System Administrator",
                    Value = "admin"
                });

                UserTypeList.Add(new SelectListItem
                {
                    Text = "Normal User",
                    Value = "normal"
                });

                return Page();
            }
            else
            {
                return RedirectToPage("Home");
            }

        }


        

        public IActionResult OnPostAddProcess()
        {

            string addProcess = $"INSERT INTO Process (processName) VALUES (" + "'" + ProcessName + "'" + ");";

            DBClass.GeneralInsertQuery(addProcess);

            TempData["SuccessMessage"] = ProcessName + "was created successfully!";

            return RedirectToPage("AdminDashboard");
 

        }


        public IActionResult OnPostAddUserToProcess()
        {
            string userIDString = NewUserProcess.UserID.ToString();
            string processIDString = NewUserProcess.ProcessID.ToString();

            var userName = DBClass.UserIDtoName(userIDString);
            var processName = DBClass.ProcessIDtoName(processIDString);

            var exist = DBClass.UserProcessExist(NewUserProcess.UserID, NewUserProcess.ProcessID);
            if (!exist)
            {
                string userProcessQuery = $"INSERT INTO UserProcess (userID, processID) VALUES (" + NewUserProcess.UserID + "," + NewUserProcess.ProcessID + ");";
                DBClass.GeneralInsertQuery(userProcessQuery);

                TempData["SuccessMessage"] = userName + " was successfully added to the " + processName + " process!";

                return RedirectToPage("AdminDashboard");
            }
            else
            {
                TempData["ErrorMessage"] = userName + " already has access to the" + processName + " process";
                return RedirectToPage("AdminDashboard");
            }
        }




        public IActionResult OnPostAddUser()
        {

            DBClass.CreateAndHashUser(NewUser);
            TempData["SuccessMessage"] = NewUser.userName + "'s account was created successfully!";
            return RedirectToPage("AdminDashboard");

        }


        public IActionResult OnPostRemoveUser()
        {

            string removeUserQuery = $"DELETE FROM Users WHERE userID =" + RemoveUserID + ";";
            DBClass.GeneralInsertQuery(removeUserQuery);

            
            var userName = DBClass.UserIDtoName(RemoveUserID);

            TempData["SuccessMessage"] = userName + " has been removed from the system successfully!";
            return RedirectToPage("AdminDashboard");

        }


        public IActionResult OnPostRemoveProcess()
        {
            string removeProcessQuery = $"DELETE FROM Process WHERE processID =" + RemoveProcessID + ";";
            DBClass.GeneralInsertQuery(removeProcessQuery);

            var processName = DBClass.ProcessIDtoName(RemoveProcessID);

            TempData["SuccessMessage"] = "The" + processName + " process has been removed from the system!";
            return RedirectToPage("AdminDashboard");
        }



        public IActionResult OnPostClearInputs()
        {
            ModelState.Clear();
            NewUser = new MadisonCountyCollaborationApplication.Pages.DataClasses.Users();
            return Page();
        }

        public IActionResult OnPostPopulate()
        {
            // Set preset values
            NewUser = new DataClasses.Users
            {
                userName = "TestUsername",
                firstName = "Zack",
                lastName = "Benjamin",
                email = "zackbanjamin@gmail.com",
                phone = "1234567890",
                userType = "Admin",
            };


            return Page(); // Return to the same page with preset values populated
        }




    }
}
