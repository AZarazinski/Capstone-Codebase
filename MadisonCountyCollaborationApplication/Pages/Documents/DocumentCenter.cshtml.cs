using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MadisonCountyCollaborationApplication.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace MadisonCountyCollaborationApplication.Pages.Documents
{
    public class DocumentCenterModel : PageModel
    {
        [BindProperty]
        [Required]
        public int DocumentID { get; set; }
        public List<DataClasses.Documents> DocumentList { get; set; }
        [BindProperty]
        public List<IFormFile> FileList { get; set; }




        public DocumentCenterModel()
        {
            DocumentList = new List<DataClasses.Documents>();
        }

        public IActionResult OnGet()
        {
            //displaying Documents
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                //SqlDataReader DocumentReader = DBClass.DocReader();
                //while (DocumentReader.Read())
                //{
                //    DocumentList.Add(new DataClasses.Documents
                //    {
                //        DocumentID = Int32.Parse(DocumentReader["DocumentID"].ToString()),
                //        DocumentName = DocumentReader["DocumentName"].ToString()

                //    });
                //}
                //DBClass.MainDBconnection.Close();
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
                    if (!formFile.FileName.EndsWith(".pdf"))
                    {
                        ModelState.AddModelError("FileList", "Only PDF files are allowed.");
                        return RedirectToPage("/DocumentCenter");
                    }
                    // full path to file in temp location
                    var filePath = Directory.GetCurrentDirectory() + @"\wwwroot\Documents\" + formFile.FileName;



                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    return Page();


                }
            }
            return RedirectToPage("/DocumentCenter");
        }

        ////selecting dataset
        //public IActionResult OnPostSelect()
        //{
        //    if (DocumentID != null && DocumentID > 0)
        //    {
        //        if (DBClass.DatasetExist(DocumentID))
        //        {
        //            HttpContext.Session.SetInt32("DocumentID", DocumentID);
        //            DBClass.MainDBconnection.Close();
        //            return RedirectToPage("/ViewDocuments");

        //        }
        //        else
        //        {
        //            DBClass.MainDBconnection.Close();
        //            ViewData["CollabNotExistMessage"] = "That collaboration does not exist.";
        //            return OnGet();
        //        }

        //    }
        //    else
        //    {
        //        ViewData["CollabNotExistMessage"] = "That collaboration does not exist.";
        //        return OnGet();
        //    }
        //}

    }
}
