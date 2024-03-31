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
        [Required]
        public int ProcessID { get; set; }

        public IActionResult OnPostHome()
        {
            return RedirectToPage("/HomePage");
        }

        public IActionResult OnPostCreate()
        {
            return RedirectToPage("/Process/CreateProcess");
        }

        public IActionResult OnPostSelect()
        {
            if (ProcessID > 0)
            {
                if (DBClass.ProcessExist(ProcessID))
                {
                    HttpContext.Session.SetInt32("processID", ProcessID);
                    DBClass.MainDBconnection.Close();
                    return RedirectToPage("/Process/Index");
                }
                else
                {
                    DBClass.MainDBconnection.Close();
                    ViewData["ProcessNotExistMessage"] = "That process does not exist.";
                    return OnGet();
                }
            }
            else
            {
                ViewData["ProcessNotExistMessage"] = "That process does not exist.";
                return OnGet();
            }
        }

        public List<DataClasses.Process> ProcessList { get; set; }

        public HomeModel()
        {
            ProcessList = new List<DataClasses.Process>();
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for " + HttpContext.Session.GetString("username") + " successful!";
                string username = HttpContext.Session.GetString("username");
                var userID = DBClass.UserNameIDConverter(username);

                string sqlQuery = $@"
                    SELECT Process.ProcessID, processName, notesAndInfo
                    FROM Process
                    INNER JOIN UserProcess ON UserProcess.ProcessID = Process.ProcessID
                    WHERE UserProcess.userID = {userID};";

                SqlDataReader processReader = DBClass.GeneralReaderQuery(sqlQuery);
                while (processReader.Read())
                {
                    ProcessList.Add(new DataClasses.Process
                    {
                        ProcessID = processReader.GetInt32(processReader.GetOrdinal("processID")),
                        ProcessName = processReader.GetString(processReader.GetOrdinal("processName")),
                        
                    });
                }

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
