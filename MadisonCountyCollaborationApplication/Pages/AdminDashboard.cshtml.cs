using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadisonCountyCollaborationApplication.Pages
{
    public class AdminDashboardModel : PageModel
    {
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

        //public IActionResult OnGet() 
        //{

        //    string username = HttpContext.Session.GetString("username");
        //    var userID = DBClass.UserNameIDConverter(username);

        //    bool isAdmin = DBClass.CheckAdmin(userID);

        //    if (isAdmin)
        //    {
        //        return Page();
        //    }
        //    else
        //    {
        //        return RedirectToPage("Home");
        //    }

        //}

        [BindProperty]
        public MadisonCountyCollaborationApplication.Pages.DataClasses.Users NewUser { get; set; }

      
        public IActionResult OnPostCreate()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Add hashed PW to AUTH and create user and add to Users table
            DBClass.CreateAndHashUser(NewUser);
            DBClass.MainDBconnection.Close();

            return RedirectToPage("Login");
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
                userPassword = "12345",
                street = "123 Street",
                city = "Harrisonburg",
                userState = "VA",
                zip = "22801"
            };


            return Page(); // Return to the same page with preset values populated
        }

    }
}
