using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class WhatIfOutputModel : PageModel
    {
        public double ExpectedOutcome { get; set; }
        public double Intercept {  get; set; }
        public List<double> Slopes { get; set; }
        public List<string> IndependentVariables { get; set; } = new List<string>();
        public string DependentVariable { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for " + HttpContext.Session.GetString("username") + " successful!";

                // Retrieve data from session
                var interceptJson = HttpContext.Session.GetString("Intercept");
                var slopesJson = HttpContext.Session.GetString("Slopes");
                var variablesJson = HttpContext.Session.GetString("Variables");
                var dependentVariableJson = HttpContext.Session.GetString("DependentVariable");
                var strExpectedOutcome = HttpContext.Session.GetString("ExpectedOutcome");

                ExpectedOutcome = double.Parse(strExpectedOutcome);
                // Optionally, you can call other methods here to perform additional initialization or processing

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
