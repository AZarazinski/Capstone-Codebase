using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class GenerateReportModel : PageModel
    {
        public int collabID { get; set; }
        public string collabName { get; set; }
        public string collabdateCreated { get; set; }
        public List<Plans> PlansList { get; set; } = new List<Plans>();
        public int planID { get; set; }
        public String? planName { get; set; }
        public String? planDesc { get; set; }
        public DateTime? dateCreated { get; set; }
        public List<DataClasses.Users> UsersList { get; set; } = new List<DataClasses.Users>();
        public int UserID { get; set; }
        public String? userName { get; set; }
        public String? firstName { get; set; }
        public String? lastName { get; set; }
        public String? email { get; set; }
        public List<KnowledgeItems> KnowledgeItemsList { get; set; } = new List<KnowledgeItems>();
        public int KnowledgeItemsID { get; set; }
        public String? title { get; set; }
        public String? KISubject { get; set; }
        public String? category { get; set; }
        public String? information { get; set; }
        public DateTime? KMDate { get; set; }
        public string currentUserFirstName { get; set; }
        public string currentUserLastName { get; set; }
        public List<SWOT> SwotList { get; set; } = new List<SWOT>();

        public DateTime currentsession { get; set; } = DateTime.Now;
        public IActionResult OnGet()
        {
            string currentUser = HttpContext.Session.GetString("username");
            if (currentUser != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + currentUser
                    + " successful!";
                string[] userFields = DBClass.GetUserForReportGenerator(currentUser);
                DBClass.MainDBconnection.Close();
                currentUserFirstName = userFields[0];
                currentUserLastName = userFields[1];

                int collabID = (int)HttpContext.Session.GetInt32("collaborationID");
                string[] fields = DBClass.CollabReportView(collabID);
                DBClass.MainDBconnection.Close();
                collabName = fields[0];
                collabdateCreated = fields[2];

                // All Collab Elements
                SqlDataReader GetPlansFromCollabReader = DBClass.GetPlansFromCollabReader(collabID);
                while (GetPlansFromCollabReader.Read())
                {
                    PlansList.Add(new Plans
                    {
                        planID = Int32.Parse(GetPlansFromCollabReader["planID"].ToString()),
                        planName = GetPlansFromCollabReader["planName"].ToString(),
                        planDesc = GetPlansFromCollabReader["planDesc"].ToString(),
                        dateCreated = GetPlansFromCollabReader.GetDateTime(GetPlansFromCollabReader.GetOrdinal("dateCreated"))
                    });
                }
                DBClass.MainDBconnection.Close();

                // All Users for the Collab
                SqlDataReader GetUsersFromCollabReader = DBClass.GetUsersFromCollabReader(collabID);
                while (GetUsersFromCollabReader.Read())
                {
                    UsersList.Add(new DataClasses.Users
                    {
                        userID = Int32.Parse(GetUsersFromCollabReader["userID"].ToString()),
                        firstName = GetUsersFromCollabReader["firstName"].ToString(),
                        lastName = GetUsersFromCollabReader["lastName"].ToString(),
                        email = GetUsersFromCollabReader["email"].ToString(),
                        userName = GetUsersFromCollabReader["userName"].ToString(),
                    });
                }
                DBClass.MainDBconnection.Close();

                // KnowledgeItems for the Collab
                SqlDataReader GetKnowledgeItemsFromCollabReader = DBClass.GetKnowledgeItemsFromCollabReader(collabID);
                while (GetKnowledgeItemsFromCollabReader.Read())
                {
                    KnowledgeItemsList.Add(new KnowledgeItems
                    {
                        knowledgeItemID = Int32.Parse(GetKnowledgeItemsFromCollabReader["knowledgeItemID"].ToString()),
                        title = GetKnowledgeItemsFromCollabReader["title"].ToString(),
                        KISubject = GetKnowledgeItemsFromCollabReader["KISubject"].ToString(),
                        category = GetKnowledgeItemsFromCollabReader["category"].ToString(),
                        information = GetKnowledgeItemsFromCollabReader["information"].ToString(),
                        KMDate = GetKnowledgeItemsFromCollabReader.GetDateTime(GetKnowledgeItemsFromCollabReader.GetOrdinal("KMDate"))
                    });
                }
                DBClass.MainDBconnection.Close();

                SqlDataReader GeSWOTFromCollabReader = DBClass.CollabSWOTReader(collabID);
                while (GeSWOTFromCollabReader.Read())
                {
                    SwotList.Add(new SWOT
                    {
                        swotID = Int32.Parse(GeSWOTFromCollabReader["swotID"].ToString()),
                        title = GeSWOTFromCollabReader["title"].ToString(),
                        strengths = GeSWOTFromCollabReader["strengths"].ToString(),
                        category = GeSWOTFromCollabReader["category"].ToString(),
                        weaknesses = GeSWOTFromCollabReader["weaknesses"].ToString(),
                        opportunities = GeSWOTFromCollabReader["opportunities"].ToString(),
                        threats = GeSWOTFromCollabReader["threats"].ToString(),
                        swotDate = GeSWOTFromCollabReader.GetDateTime(GeSWOTFromCollabReader.GetOrdinal("swotDate"))
                    });
                }
                DBClass.MainDBconnection.Close();
                // Close your connection in DBClass
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
