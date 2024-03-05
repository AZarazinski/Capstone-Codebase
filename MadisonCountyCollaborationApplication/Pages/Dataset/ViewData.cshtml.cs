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
        public int datasetID { get; set; }
        [BindProperty]
        public string datasetName { get; set; }

        //[BindProperty]
        public String?[,] values { get; set; }
        public DataTable Data { get; private set; }

        public ViewDataModel()
        {
            AttributeList = new List<Attributes>();
        }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                datasetID = (int)HttpContext.Session.GetInt32("datasetID");
                datasetName = DBClass.ExtractDatasetName(datasetID);
                DBClass.MainDBconnection.Close();
                Data = DBClass.FetchDataForTable(datasetName + datasetID.ToString());
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
    

