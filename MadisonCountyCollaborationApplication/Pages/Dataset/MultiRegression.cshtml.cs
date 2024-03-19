using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class MultiRegressionModel : PageModel
    {
        [BindProperty]
        public string IndependentVariable { get; set; }
        [BindProperty]
        public string DependentVariable { get; set; }
        public DataTable Data { get; private set; }
        [BindProperty]
        public int datasetID { get; set; }
        [BindProperty]
        public string datasetName { get; set; }
        public List<double> dependentDataList { get; set; } = new List<double>();
        public List<double> independentDataList { get; set; } = new List<double>();


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
    }
}
