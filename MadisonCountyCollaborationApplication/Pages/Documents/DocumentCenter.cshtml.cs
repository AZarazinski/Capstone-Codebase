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


    }
}


//string connectionString = "your_connection_string_here";
//string filePath = @"path_to_your_file_here";
//string fileName = Path.GetFileName(filePath);

//// Assuming your FileTable has columns: [name], [file_stream], and [path_locator] (automatically managed by SQL Server)
//// Adjust your INSERT statement according to your table's schema.
//string sqlQuery = "INSERT INTO Lab4FileTable (name, file_stream) OUTPUT INSERTED.stream_id VALUES (@FileName, @FileData);";

//// Read the file into a byte array
//byte[] fileData = File.ReadAllBytes(filePath);

//using (SqlConnection connection = new SqlConnection(connectionString))
//{
//    connection.Open();

//    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
//    {
//        // Parameterize to avoid SQL injection
//        command.Parameters.Add("@FileName", SqlDbType.VarChar).Value = fileName;
//        command.Parameters.Add("@FileData", SqlDbType.VarBinary, fileData.Length).Value = fileData;

//        // Execute the command and retrieve the new Stream ID if needed
//        var streamId = command.ExecuteScalar();
//        Console.WriteLine($"File saved successfully. Stream ID: {streamId}");
//    }
//}