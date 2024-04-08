using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;


namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class ViewDataModel : PageModel
    {
        public class CollaborationOption
        {
            public int CollabID { get; set; }
            public string Name { get; set; }
        }


        [BindProperty]
        public string AnalysisType { get; set; }
        [BindProperty]
        public string Independent { get; set; }
        [BindProperty]
        public string Dependent { get; set; }
        public List<Attributes> AttributeList { get; set; }
        
        [BindProperty]
        public int DatasetID { get; set; }
        [BindProperty]
        public string DatasetName { get; set; }

        public DataTable Data { get; private set; }

        [BindProperty]
        public int SelectedCollabID { get; set; } // Binds the selected collaboration ID
        public List<SelectListItem> CollaborationOptions { get; set; } // Holds dropdown options

        [BindProperty]
        public string ProcessName { get; set; }
        [BindProperty]
        public string DatasetNameDisplay { get; set; }
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
                //get process name
                ProcessName = HttpContext.Session.GetString("processName");

                //get dataset name
                DatasetNameDisplay = HttpContext.Session.GetString("datasetName");
                DatasetID = datasetID;
                //DatasetID = (int)HttpContext.Session.GetInt32("datasetID");
                HttpContext.Session.SetInt32("datasetID", DatasetID);
                DatasetName = DBClass.ExtractDatasetName(DatasetID);
                DBClass.MainDBconnection.Close();
                Data = DBClass.FetchDataForTable(DatasetName);
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
            string sqlQuery = @"
                INSERT INTO DataAssists
                (collabID, datasetID)
                VALUES (" + SelectedCollabID + "," + DatasetID + ");";

            DBClass.GeneralInsertQuery(sqlQuery);
            return Page();
        }

        public IActionResult OnPostRedirectToAnalysis()
        {
            string test = "SingleRegression";
            Console.WriteLine(AnalysisType == test);
            switch (AnalysisType)
            {
                case "SingleRegression":
                    // Redirect to the Single Regression analysis page
                    return RedirectToPage("SingleRegression");
                case "MultiRegression":
                    // Redirect to the Multi Regression analysis page
                    return RedirectToPage("MultiRegression");
                case "Simulation":
                    // Redirect to the Multi Regression analysis page
                    return RedirectToPage("MonteCarloSimple");
                case "TylerRegression":
                    return RedirectToPage("TylerRegression");
                default:
                    // Optional: Handle unknown selection or return to the current page with a message
                    return Page();
            }
        }
    }
}
    

