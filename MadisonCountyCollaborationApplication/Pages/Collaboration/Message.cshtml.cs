using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MadisonCountyCollaborationApplication.Pages.DB;
using MadisonCountyCollaborationApplication.Pages.DataClasses;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class MessageModel : PageModel
    {
        [BindProperty]
        public MadisonCountyCollaborationApplication.Pages.DataClasses.CollabChat NewM { get; set; }
        public List<CollabChat> MessageList { get; set; }
        public MessageModel()
        {
            MessageList = new List<CollabChat>();
        }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";


                int CollabID = (int)HttpContext.Session.GetInt32("collaborationID");

                SqlDataReader MessageReader = DBClass.MessageReader(CollabID);
                while (MessageReader.Read())
                {
                    MessageList.Add(new CollabChat
                    {
                        userID = Int32.Parse(MessageReader["userID"].ToString()),
                        messageInfo = MessageReader["messageInfo"].ToString()
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

        public IActionResult OnPostSend()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            NewM.userID = 1;
            NewM.collabID = (int)HttpContext.Session.GetInt32("collaborationID");
            NewM.messageTime = DateTime.Now;
            DBClass.CreateMessage(NewM);
            DBClass.MainDBconnection.Close();
            return RedirectToPage("/Collaboration");
        }
        public IActionResult OnPostClear()
        {
            ModelState.Clear();
            NewM = new MadisonCountyCollaborationApplication.Pages.DataClasses.CollabChat();
            OnGet();
            return Page();
        }
        public IActionResult OnPostPopulate()
        {
            NewM = new DataClasses.CollabChat
            {
                messageInfo = "Testing for message info populate",
                messageTime = DateTime.Now
            };
            OnGet();
            return Page();
        }
    }
}
