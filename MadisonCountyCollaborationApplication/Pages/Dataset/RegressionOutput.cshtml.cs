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


        public IActionResult OnGet(double intercept, List<string> independentVariables, List<double> slopes, string dependentVariable, double expectedOutcome, bool showResults)
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for " + HttpContext.Session.GetString("username") + " successful!";

                // You may need to assign the parameters to the corresponding properties of the model
                Intercept = intercept;
                IndependentVariables = independentVariables;
                Slopes = slopes;
                DependentVariable = dependentVariable;
                ExpectedOutcome = expectedOutcome;
                ShowResults = showResults;

                // Optionally, you can call other methods here to perform additional initialization or processing

                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login");
            }
        }
        //public void LoadVariables()
        //{
        //    // Retrieve data from session
        //    var interceptJson = HttpContext.Session.GetString("Intercept");
        //    var slopesJson = HttpContext.Session.GetString("Slopes");
        //    var variablesJson = HttpContext.Session.GetString("Variables");
        //    var dependentVariableJson = HttpContext.Session.GetString("DependentVariable");
        //    Console.WriteLine(interceptJson.ToString());
        //    Console.WriteLine(slopesJson.ToString());
        //    Console.WriteLine(variablesJson.ToString());
        //    Console.WriteLine(dependentVariableJson.ToString());

        //    // Deserialize data
        //    Intercept = JsonSerializer.Deserialize<double>(interceptJson);
        //    Slopes = JsonSerializer.Deserialize<List<double>>(slopesJson);
        //    IndependentVariables = JsonSerializer.Deserialize<List<string>>(variablesJson);
        //    DependentVariable = JsonSerializer.Deserialize<string>(dependentVariableJson);
        //}
        public async Task<IActionResult> OnPostCalculateWhatIfScenarioAsync()
        {
            Console.WriteLine("Creating What-If Scenario Result");

            double expectedOutcome = 0;

            if (Intercept.HasValue && Slopes != null)
            {
                expectedOutcome += Intercept.Value; // Start with the intercept

                foreach (var variable in IndependentVariables)
                {
                    string inputFieldName = $"WhatIfInputs[{variable}]";
                    if (HttpContext.Request.Form.TryGetValue(inputFieldName, out var inputValue) && double.TryParse(inputValue, out double variableValue))
                    {
                        // Find the corresponding slope for the variable
                        int slopeIndex = IndependentVariables.IndexOf(variable);
                        if (slopeIndex >= 0 && slopeIndex < Slopes.Count)
                        {
                            double slope = Slopes[slopeIndex];
                            // Apply the regression equation component for this variable
                            expectedOutcome += slope * variableValue;
                        }
                    }
                }

                Console.WriteLine(expectedOutcome.ToString());

                // Store or display the expected outcome as needed
                // For example, adding it to ViewData to display in the Razor view\

                ViewData["ExpectedY"] = expectedOutcome;
                ShowResults = true;

                // Update the model properties
                ExpectedOutcome = expectedOutcome;

                // Redirect back to the page with updated model properties
                return RedirectToPage("RegressionOutput", new { Intercept = Intercept.Value, IndependentVariables = IndependentVariables, Slopes = Slopes, DependentVariable = DependentVariable, ExpectedOutcome = expectedOutcome, ShowResults = true });
            }
            else
            {
                // Handle the case where the model components are not found in the session
                ViewData["Error"] = "Regression model components not found.";
                ShowResults = false;
                return RedirectToPage();
            }
        }

    }
}
