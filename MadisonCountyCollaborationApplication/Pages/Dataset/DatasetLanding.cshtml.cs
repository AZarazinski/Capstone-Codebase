using MadisonCountyCollaborationApplication.Pages.DB;
using MadisonCountyCollaborationApplication.Pages.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class DatasetLandingModel : PageModel
    {
        [BindProperty]
        [Required]
        public int DatasetID { get; set; }
        public List<Datasets> DatasetList { get; set; }
        [BindProperty]
        public List<IFormFile> FileList { get; set; }
        public MadisonCountyCollaborationApplication.Pages.DataClasses.Attributes NewA { get; set; }
        public MadisonCountyCollaborationApplication.Pages.DataClasses.AttributeValue NewV { get; set; }



        public DatasetLandingModel()
        {
            DatasetList = new List<Datasets>();
        }

        public IActionResult OnGet()
        {
            //displaying datasets
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                SqlDataReader DatasetReader = DBClass.DatasetReader();
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
                return RedirectToPage("../User/Login");
            }
        }
        //adding datataset
        public IActionResult OnPostAsync()
        {
            String attributeName;
            int attributeID;
            String dataValue;
            //looping through each file
            foreach (var formFile in FileList)
            {
                if (formFile.Length > 0)
                {
                    // Ensure only CSV files are uploaded
                    if (!formFile.FileName.EndsWith(".csv"))
                    {
                        ModelState.AddModelError("FileList", "Only CSV files are allowed.");
                        return RedirectToPage("Datasets");
                    }
                    // full path to file in temp location
                    var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\fileupload\" + formFile.FileName;



                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    return RedirectToPage("FileHandling", new { filePath = filePath });
                   

                }
            }
            return RedirectToPage("DatasetLanding");
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
                    return RedirectToPage("ViewData");

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
