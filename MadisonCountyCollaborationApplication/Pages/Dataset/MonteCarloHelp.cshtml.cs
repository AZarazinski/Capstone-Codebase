using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadisonCountyCollaborationApplication.Pages
{
    public class MonteCarloHelpModel : PageModel
    {
        [BindProperty]
        public int datasetID { get; set; }
        [BindProperty]
        public int DatasetName { get; set; }
        public void OnGet()
        {
            datasetID = (int)HttpContext.Session.GetInt32("datasetID");
            //DatasetName = DBClass.ExtractDatasetName(datasetID);
            DBClass.MainDBconnection.Close();
        }
    }
}
