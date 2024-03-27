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
        public double? Intercept { get; set; }
        public List<double> Slopes { get; set; }
        public Dictionary<string, double> WhatIfInputs { get; set; } = new Dictionary<string, double>();
        public bool ShowResults { get; set; } = false;
        public double? CalculatedIntercept { get; set; }
        public double ExpectedOutcome { get; set; } = double.NaN;


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

                // Deserialize data
                Intercept = JsonSerializer.Deserialize<double>(interceptJson);
                Slopes = JsonSerializer.Deserialize<List<double>>(slopesJson);
                IndependentVariables = JsonSerializer.Deserialize<List<string>>(variablesJson);
                DependentVariable = JsonSerializer.Deserialize<string>(dependentVariableJson);

                // Optionally, you can call other methods here to perform additional initialization or processing

                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login");
            }
        }

        public IActionResult OnPostCalculateWhatIfScenarioAsync()
        {
            Console.WriteLine("Creating What-If Scenario Result");

            // Calculate expected outcome
            double expectedOutcome = CalculateExpectedOutcome();

            // Store the expected outcome in the session
            HttpContext.Session.SetString("ExpectedOutcome", expectedOutcome.ToString());

            // Redirect to the WhatIfOutput page with expectedOutcome query parameter
            return RedirectToPage("WhatIfOutput", new { expectedOutcome = expectedOutcome });
        }

        private double CalculateExpectedOutcome()
        {
            // Calculate expected outcome based on the model properties
            double expectedOutcome = Intercept ?? 0;

            if (Slopes != null)
            {
                for (int i = 0; i < IndependentVariables.Count; i++)
                {
                    string variable = IndependentVariables[i];
                    string inputFieldName = $"WhatIfInputs[{variable}]";

                    if (HttpContext.Request.Form.TryGetValue(inputFieldName, out var inputValue) && double.TryParse(inputValue, out double variableValue))
                    {
                        double slope = Slopes[i];
                        expectedOutcome += slope * variableValue;
                    }
                }
            }

            return expectedOutcome;
        }
    }
}
