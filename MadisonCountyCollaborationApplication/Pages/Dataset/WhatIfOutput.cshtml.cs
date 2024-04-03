using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Serialization;
using System.Text.Json;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class WhatIfOutputModel : PageModel
    {
        [BindProperty]
        public double ExpectedOutcome { get; set; }
        public double Intercept { get; set; }
        public List<double> Slopes { get; set; }
        [BindProperty]
        public List<string> IndependentVariables { get; set; } = new List<string>();
        [BindProperty]
        public string DependentVariable { get; set; }
        [BindProperty]
        public double LowerBound { get; set; }
        [BindProperty]
        public double UpperBound { get; set; }
        [BindProperty]
        public List<double> Values { get; set; }
        public double ConfidenceLevel { get; set; } = .05;
        [BindProperty]
        public string ProcessName { get; set; }
        [BindProperty]
        public string DatasetName { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for " + HttpContext.Session.GetString("username") + " successful!";

                // Retrieve data from session
                var lowerJson = HttpContext.Session.GetString("LowerBound");
                var upperJson = HttpContext.Session.GetString("UpperBound");
                var confidenceLevelJson = HttpContext.Session.GetString("ConfidenceLevel");
                var variablesJson = HttpContext.Session.GetString("Variables");
                var dependentVariableJson = HttpContext.Session.GetString("DependentVariable");


                LowerBound = double.Parse(lowerJson);
                UpperBound = double.Parse(upperJson);
                ExpectedOutcome = double.Parse(HttpContext.Session.GetString("ExpectedOutcome"));
                ConfidenceLevel = JsonSerializer.Deserialize<double>(confidenceLevelJson);
                IndependentVariables = JsonSerializer.Deserialize<List<string>>(variablesJson);
                DependentVariable = JsonSerializer.Deserialize<string>(dependentVariableJson);
                //get process name
                ProcessName = HttpContext.Session.GetString("processName");

                //get dataset name
                DatasetName = HttpContext.Session.GetString("datasetName");


                Values = new List<double> { (LowerBound * .9) , LowerBound, ExpectedOutcome, (UpperBound * 1.1), UpperBound };


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
