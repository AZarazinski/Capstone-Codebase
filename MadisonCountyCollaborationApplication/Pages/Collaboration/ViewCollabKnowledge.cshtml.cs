using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class ViewCollabKnowledgeModel : PageModel
    {
        [BindProperty]
        public int KnowledgeItemID { get; set; }
        [BindProperty]
        public int swotID { get; set; }
        public List<KnowledgeItems> KnowledgeList { get; set; }
        public List<SWOT> swotItemsList { get; set; }

        public ViewCollabKnowledgeModel()
        {
            KnowledgeList = new List<KnowledgeItems>();
            swotItemsList = new List<SWOT>();


        }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";


                int CollabID = (int)HttpContext.Session.GetInt32("collaborationID");
                //reading knowledge
                SqlDataReader KnowledgeReader = DBClass.CollabKnowledgeReader(CollabID);
                while (KnowledgeReader.Read())
                {
                    KnowledgeList.Add(new KnowledgeItems
                    {
                        knowledgeItemID = Int32.Parse(KnowledgeReader["knowledgeItemID"].ToString()),
                        title = KnowledgeReader["title"].ToString(),
                        KISubject = KnowledgeReader["KISubject"].ToString()
                    });
                }

                // Close your connection in DBClass
                DBClass.MainDBconnection.Close();
                //reading SWOT
                SqlDataReader SWOTReader = DBClass.CollabSWOTReader(CollabID);
                while (SWOTReader.Read())
                {
                    swotItemsList.Add(new SWOT
                    {
                        swotID = int.Parse(SWOTReader["swotID"].ToString()),
                        title = SWOTReader["title"].ToString(),
                        category = SWOTReader["category"].ToString(),
                        strengths = SWOTReader["strengths"].ToString(),
                        weaknesses = SWOTReader["weaknesses"].ToString(),
                        opportunities = SWOTReader["opportunities"].ToString(),
                        threats = SWOTReader["threats"].ToString(),
                        //,KMDate = ((DateTime)KnowledgeItemsReader["KMDate"]).ToString("yyyy-MM-dd HH:mm:ss")

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
        //selecting knowledge
        public IActionResult OnPostSelectKnowledge()
        {
            if (KnowledgeItemID != null && KnowledgeItemID > 0)
            {
                if (DBClass.KnowledgeExist(KnowledgeItemID))
                {
                    DBClass.MainDBconnection.Close();
                    HttpContext.Session.SetInt32("knowledgeItemID", KnowledgeItemID);
                    string[] fields = DBClass.KnowledgeItemView(KnowledgeItemID);
                    DBClass.MainDBconnection.Close();
                    HttpContext.Session.SetString("KnowledgeTitle", fields[0]);
                    DBClass.MainDBconnection.Close();
                    HttpContext.Session.SetString("KnowledgeSubject", fields[1]);
                    DBClass.MainDBconnection.Close();
                    HttpContext.Session.SetString("KnowledgeCategory", fields[2]);
                    DBClass.MainDBconnection.Close();
                    HttpContext.Session.SetString("KnowledgeInfo", fields[3]);
                    DBClass.MainDBconnection.Close();
                    return RedirectToPage("/Reply");

                }
                else
                {
                    DBClass.MainDBconnection.Close();
                    ViewData["KnowledgeNotExistMessage"] = "That KnowledgeItem does not exist.";
                    return OnGet();
                }

            }
            else
            {
                ViewData["KnowledgeNotExistMessage"] = "That KnowledgeItem does not exist.";
                return OnGet();
            }
        }
        //select SWOT
        public IActionResult OnPostViewSWOT()
        {
            HttpContext.Session.SetInt32("swotID", swotID);
            return RedirectToPage("/ViewSWOTs");
        }
        public IActionResult OnPostKnowledge()
        {
            return RedirectToPage("/KnowledgeItems");
        }
    }
}
