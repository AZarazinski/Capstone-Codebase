using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class ViewDataModel : PageModel
    {
        public List<Attributes> AttributeList { get; set; }
        [BindProperty]
        public int DatasetID { get; set; }
        [BindProperty]
        public string DatasetName { get; set; }

        public DataTable Data { get; private set; }

        public ViewDataModel()
        {
            AttributeList = new List<Attributes>();
        }
        public IActionResult OnGet(int datasetID)
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                DatasetID = datasetID;
                DatasetName = DBClass.ExtractDatasetName(DatasetID);
                DBClass.MainDBconnection.Close();
                Data = DBClass.FetchDataForTable(DatasetName + DatasetID.ToString());
                DBClass.MainDBconnection.Close();
            

                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login");
            }
        }
        public IActionResult OnPostAddCollabData()
        {
            return RedirectToPage("AddCollabData");
        }
    }
}
    

