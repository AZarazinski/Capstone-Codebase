using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text.Json;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class RegressionOutputModel : PageModel
    {
        [BindProperty]
        public List<string> IndependentVariables { get; set; } = new List<string>();
        [BindProperty]
        public string DependentVariable { get; set; }
        public DataTable Data { get; private set; } = new DataTable();
        [BindProperty]
        public int datasetID { get; set; }
        [BindProperty]
        public string datasetName { get; set; }
        [BindProperty]
        public double? Intercept { get; set; }
        [BindProperty]
        public List<double> Slopes { get; set; }
        [BindProperty]
        public List<double> WhatIfInputs { get; set; } = new List<double>();
        public bool ShowResults { get; set; } = false;
        [BindProperty]
        public double ExpectedOutcome { get; set; } = double.NaN;
        [BindProperty]
        public List<double> StandardErrors { get; set; } = new List<double>();
        [BindProperty]
        public List<double> PValues { get; set; } = new List<double>();
        [BindProperty]
        public double ConfidenceLevel {  get; set; } = .05;
        [BindProperty]
        public double Aplha {  get; set; } = .05;



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
                var standardErrorsJson = HttpContext.Session.GetString("StandardError");
                var pValuesJson = HttpContext.Session.GetString("PValues");
                var confidenceLevelJson = HttpContext.Session.GetString("ConfidenceLevel");

                // Deserialize data
                Intercept = JsonSerializer.Deserialize<double>(interceptJson);
                Slopes = JsonSerializer.Deserialize<List<double>>(slopesJson);
                IndependentVariables = JsonSerializer.Deserialize<List<string>>(variablesJson);
                DependentVariable = JsonSerializer.Deserialize<string>(dependentVariableJson);

                if (!string.IsNullOrWhiteSpace(confidenceLevelJson)) 
                { 
                    ConfidenceLevel = JsonSerializer.Deserialize<double>(confidenceLevelJson);
                }
                if (!string.IsNullOrWhiteSpace(standardErrorsJson))
                {
                    StandardErrors = JsonSerializer.Deserialize<List<double>>(standardErrorsJson);
                }

                if (!string.IsNullOrWhiteSpace(pValuesJson))
                {
                    PValues = JsonSerializer.Deserialize<List<double>>(pValuesJson);
                }
                double Alpha = (1.0 - (ConfidenceLevel / 100.0));
                Console.WriteLine(Alpha.ToString());

                // Optionally, you can call other methods here to perform additional initialization or processing

                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login");
            }
        }

        public IActionResult OnPostCalculateWhatIfScenario()
        {
            Console.WriteLine("Creating What-If Scenario Result");
            Console.WriteLine($"Input count: {WhatIfInputs.Count}");
            foreach (var input in WhatIfInputs)
            {
                Console.WriteLine($"Input: {input}");
            }
            // Prepare the inputs dictionary from the form submission
            double expectedOutcome = CalculateExpectedOutcome(WhatIfInputs, Slopes, Intercept);
            // Calculate expected outcome using the new method

            // Store the expected outcome in the session
            HttpContext.Session.SetString("ExpectedOutcome", expectedOutcome.ToString());
            Console.WriteLine($"Expected Outcome: {expectedOutcome}");
            // Redirect to the WhatIfOutput page with expectedOutcome query parameter
            return RedirectToPage("WhatIfOutput", new { ExpectedOutcome = expectedOutcome });
        }

        private double CalculateExpectedOutcome(List<double> variableInputs, List<double> Slopes, double? Intercept)
        {
            double expectedOutcome = Intercept ?? 0;
            if (Slopes != null && variableInputs != null)
            {
                for (int i = 0; i < variableInputs.Count && i < Slopes.Count; i++)
                {
                    expectedOutcome += Slopes[i] * variableInputs[i];
                    Console.WriteLine($"{Slopes[i]} * {variableInputs[i]}");
                }
            }
            return expectedOutcome;
        }

    }
}
