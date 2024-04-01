using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadisonCountyCollaborationApplication.Pages.Process
{
    public class DocumentHandlingModel : PageModel
    {

        // Method to check session before accessing the page
        //public IActionResult OnGetSessionCheck()
        //{
        //    if (HttpContext.Session.GetString("username") == null)
        //    { // If user is not logged in, set an error message and redirect to login page
        //        HttpContext.Session.SetString("LoginError", "You must login to access that page!");
        //        return RedirectToPage("/Login/HashedLogin");
        //    }
        //    else
        //    {
        //        return Page(); // If logged in, allow access to the page
        //    }
        //}

        //// Method to handle GET requests for processing a Document
        //public async Task<IActionResult> OnGetAsync(string filePath)
        //{
        //    int ProcessID = (int)HttpContext.Session.GetInt32("processID");

        //    if (string.IsNullOrWhiteSpace(filePath))
        //    {
        //        ModelState.AddModelError("", "File path is not provided.");
        //        return Page();
        //    }

        //    if (!System.IO.File.Exists(filePath)) // error message if the file does not exist 
        //    {
        //        ModelState.AddModelError("", "File does not exist.");
        //        return Page();
        //    }

        //    try
        //    {
        //        await ValidateFile(filePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", $"Error processing file: {ex.Message}"); // error message if there is an error processing file
        //        return Page();
        //    }

        //    // Redirect to a success page and optionally pass the table name for confirmation.
        //    TempData["TableName"] = Path.GetFileNameWithoutExtension(filePath);
        //    return RedirectToPage("./Index", new { ProcessID = ProcessID });
        //}


        //public async Task ValidateFile(filePath)
        //{

        //}
    }
}
