using MadisonCountyCollaborationApplication.Pages.DB;
using MadisonCountyCollaborationApplication.Pages.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages.Collaboration
{
    public class ViewCollabDataModel : PageModel
    {
        [BindProperty]
        [Required]
        public int DatasetID { get; set; }
        public List<Datasets> DatasetList { get; set; }
        public ViewCollabDataModel()
        {
            DatasetList = new List<Datasets>();
        }
        //displaying all datasets
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";
                int collabID = (int)HttpContext.Session.GetInt32("collaborationID");
                SqlDataReader DatasetReader = DBClass.CollabDatasetReader(collabID);
                while (DatasetReader.Read())
                {
                    DatasetList.Add(new Datasets
                    {
                        ID = Int32.Parse(DatasetReader["datasetID"].ToString()),
                        Name = DatasetReader["dataSetName"].ToString()

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
        public IActionResult OnPostAnalysis()
        {
            return RedirectToPage("/Datasets");
        }
        //selecting dataset
        public IActionResult OnPostSelect()
        {
            if (DatasetID != null && DatasetID > 0)
            {
                if (DBClass.DatasetExist(DatasetID))
                {
                    HttpContext.Session.SetInt32("datasetID", DatasetID);
                    DBClass.MainDBconnection.Close();
                    return RedirectToPage("../Dataset/ViewData");

                }
                else
                {
                    DBClass.MainDBconnection.Close();
                    ViewData["CollabNotExistMessage"] = "That collaboration does not exist.";
                    return OnGet();
                }

            }
            else
            {
                ViewData["CollabNotExistMessage"] = "That collaboration does not exist.";
                return OnGet();
            }
        }
    }
}
