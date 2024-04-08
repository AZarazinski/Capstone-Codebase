using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MadisonCountyCollaborationApplication.Pages.Users
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        [BindProperty]
        public int UserID { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "No Username")]
        public String? Username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "No Password")]
        public String? Password { get; set; }
        public void OnGet()
        {
        }
        public IActionResult OnPostLoginHandler()
        {
            if (Username != null || Password != null)
            {
                if (DBClass.HashedParameterLogin(Username, Password))
                {
                    HttpContext.Session.SetString("username", Username);
                    ViewData["LoginMessage"] = "Login Successful!";

                    string username = HttpContext.Session.GetString("username");
                    var userID = DBClass.UserNameIDConverter(username);

                    var admin =DBClass.CheckAdmin(userID);

                    if (admin)
                    {
                        HttpContext.Session.SetString("userType", "admin" );
                    }

                    DBClass.MainDBconnection.Close();
                    return RedirectToPage("../Home", new { username = Username });
                }
                else
                {
                    ViewData["LoginMessage"] = "Username and Password Incorrect";
                    DBClass.MainDBconnection.Close();
                    OnPostClear();
                    return Page();
                }

            }
            else
            {
                ViewData["LoginMessage"] = "Username and Password Incorrect";
                return OnPostClear();
            }
        }
        public IActionResult OnPostPopulate()
        {
            Username = "demoUser";
            Password = "demoPW";
            return Page();
        }

        public IActionResult OnPostClear()
        {
            Username = string.Empty;
            Password = string.Empty;
            return Page();
        }
       
    }
}
