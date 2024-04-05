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


        public List<SelectListItem> UserList { get; set; }

        public List<SelectListItem> ProcessList { get; set; }

 

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

                return Page();
            }
            else
            {
                return RedirectToPage("Home");
            }

        }


        

        public IActionResult OnPostAddProcess()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
            //else
            //{

                string addProcess = $"INSERT INTO Process (processName) VALUES (" + "'" + ProcessName + "'" + ");";

                DBClass.GeneralInsertQuery(addProcess);



            return RedirectToPage("AdminDashboard");
            //}

        }


        public IActionResult OnPostAddUserToProcess()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
            //else
            //{
                string userProcessQuery = $"INSERT INTO UserProcess (userID, processID) VALUES (" + NewUserProcess.UserID + "," + NewUserProcess.ProcessID + ");";

                DBClass.GeneralInsertQuery(userProcessQuery);

                return RedirectToPage("AdminDashboard");
            //}
        }




        public IActionResult OnPostAddUser()
        {
         

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
                    // Assuming DBClass.CreateAndHashUser properly implements user creation
                    DBClass.CreateAndHashUser(NewUser);
                    TempData["SuccessMessage"] = "User created successfully.";
                    return RedirectToPage("AdminDashboard"); // Or wherever you want to redirect
            //    }
            //    catch (Exception ex)
            //    {
            //        ModelState.AddModelError("", "An error occurred while creating the user.");
            //    }
            //}

            // This is for re-displaying the page with validation errors, if any.
            return Page();
        }


        public IActionResult OnPostRemoveUser()
        {

            string removeUserQuery = $"DELETE FROM Users WHERE userID =" + RemoveUserID + ";";

            DBClass.GeneralInsertQuery(removeUserQuery);

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
