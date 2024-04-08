using MadisonCountyCollaborationApplication.Pages.DB;
using MathNet.Numerics.Distributions;
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
        public List<double> StandardErrors { get; set; }
        [BindProperty]
        public List<double> PValues { get; set; } = new List<double>();
        [BindProperty]
        public double ConfidenceLevel { get; set; } = .05;
        [BindProperty]
        public double Aplha { get; set; } = .05;
        [BindProperty]
        public double DegreesOfFreedom { get; set; }
        [BindProperty]
        public double CriticalValue { get; set; }
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
                var interceptJson = HttpContext.Session.GetString("Intercept");
                var slopesJson = HttpContext.Session.GetString("Slopes");
                var variablesJson = HttpContext.Session.GetString("Variables");
                var dependentVariableJson = HttpContext.Session.GetString("DependentVariable");
                var pValuesJson = HttpContext.Session.GetString("PValues");
                var confidenceLevelJson = HttpContext.Session.GetString("ConfidenceLevel");
                var criticalValueJson = HttpContext.Session.GetString("CriticalValue");
                CriticalValue = JsonSerializer.Deserialize<double>(criticalValueJson);
                //get dataset name
                datasetID = (int)HttpContext.Session.GetInt32("datasetID");
                DatasetName = DBClass.ExtractDatasetName(datasetID);
                DBClass.MainDBconnection.Close();

                // Deserialize data
                Intercept = JsonSerializer.Deserialize<double>(interceptJson);
                Slopes = JsonSerializer.Deserialize<List<double>>(slopesJson);
                IndependentVariables = JsonSerializer.Deserialize<List<string>>(variablesJson);
                DependentVariable = JsonSerializer.Deserialize<string>(dependentVariableJson);
                var standardErrorsJson = HttpContext.Session.GetString("StandardError");

                if (!string.IsNullOrWhiteSpace(standardErrorsJson))
                {
                    StandardErrors = JsonSerializer.Deserialize<List<double>>(standardErrorsJson);
                }
                else
                {
                    Console.WriteLine("StandardErrors session data is null or empty.");
                }
                if (!string.IsNullOrWhiteSpace(confidenceLevelJson))
                {
                    ConfidenceLevel = JsonSerializer.Deserialize<double>(confidenceLevelJson);
                }

                if (!string.IsNullOrWhiteSpace(pValuesJson))
                {
                    PValues = JsonSerializer.Deserialize<List<double>>(pValuesJson);
                }
                double Alpha = (1.0 - (ConfidenceLevel / 100.0));
                //get process name
                ProcessName = HttpContext.Session.GetString("processName");


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
            string inputs = "";
            foreach (var input in WhatIfInputs)
            {
                inputs += input + ",";
                Console.WriteLine($"Input: {input}");
            }
            inputs = inputs.Remove(inputs.Length - 1);
            HttpContext.Session.SetString("inputs", inputs);
            // Prepare the inputs dictionary from the form submission
            double expectedOutcome = CalculateExpectedOutcome(WhatIfInputs, Slopes, Intercept);
            // Calculate confidence intervals (placeholder for actual calculation)
            var standardErrorsJson = HttpContext.Session.GetString("StandardError");

            if (!string.IsNullOrWhiteSpace(standardErrorsJson))
            {
                StandardErrors = JsonSerializer.Deserialize<List<double>>(standardErrorsJson);
                Console.WriteLine(StandardErrors.Count); // Should now reflect the correct count
            }
            double standardErrorOfPrediction = CalculateStandardErrorOfPrediction(WhatIfInputs, Slopes, StandardErrors);

            var criticalValueJson = HttpContext.Session.GetString("CriticalValue");
            CriticalValue = JsonSerializer.Deserialize<double>(criticalValueJson);

            //double tValue = GetCriticalTValue(DegreesOfFreedom, ConfidenceLevel);
            //Console.WriteLine(tValue.ToString());
            var tValue = CriticalValue;
            double marginOfError = tValue * standardErrorOfPrediction;
            double lowerBound = expectedOutcome - marginOfError;
            double upperBound = expectedOutcome + marginOfError;

            // Calculate expected outcome using the new method

            // Store the expected outcome in the session
            HttpContext.Session.SetString("ExpectedOutcome", expectedOutcome.ToString());
            Console.WriteLine($"Expected Outcome: {expectedOutcome}");
            // Redirect to the WhatIfOutput page with expectedOutcome query parameter
            //return RedirectToPage("WhatIfOutput", new { ExpectedOutcome = expectedOutcome });

            // Store the results in the session
            HttpContext.Session.SetString("ExpectedOutcome", expectedOutcome.ToString());
            HttpContext.Session.SetString("LowerBound", lowerBound.ToString());
            HttpContext.Session.SetString("UpperBound", upperBound.ToString());

            // Serialize and store in session
            var whatIfValuesJson = JsonSerializer.Serialize(WhatIfInputs);
            HttpContext.Session.SetString("WhatIfValues", whatIfValuesJson);


            // Redirect to the WhatIfOutput page
            return RedirectToPage("WhatIfOutput", new { ExpectedOutcome = expectedOutcome, LowerBound = lowerBound, UpperBound = upperBound });

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
        public IActionResult OnPostSimulate()
        {
            return RedirectToPage("MonteCarloRegression");
        }

        private double CalculateStandardErrorOfPrediction(List<double> variableInputs, List<double> slopes, List<double> standardErrors)
        {
            double sumOfSquaredSE = 0;
            // Start loop from 1 to skip the intercept's standard error
            for (int i = 0; i < variableInputs.Count; i++)
            {
                // Adjust the index for standardErrors by adding 1, since the first entry is for the intercept
                sumOfSquaredSE += Math.Pow(standardErrors[i + 1] * variableInputs[i], 2);
            }
            double standardErrorOfPrediction = Math.Sqrt(sumOfSquaredSE);
            return standardErrorOfPrediction;
        }

        private double GetCriticalTValue(double degreesOfFreedom, double confidenceLevel)
        {
            // Convert confidence level to alpha (e.g., 95% confidence -> 0.05 alpha)
            double alpha = 1 - confidenceLevel;
            // Get the critical t-value from the t-distribution
            double tValue = StudentT.InvCDF(0, 1, degreesOfFreedom, 1 - alpha / 2.0);
            return tValue;
        }

    }
}
