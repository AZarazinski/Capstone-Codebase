using MadisonCountyCollaborationApplication.Pages.DB;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.DotNet.Interactive.Formatting;
using System.Text.Json.Serialization;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class MultiRegressionModel : PageModel
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
        public List<double> dependentDataList { get; set; } = new List<double>();
        public List<double> independentDataList { get; set; } = new List<double>();
        public double? Intercept { get; set; }
        public List<double> Slopes { get; set; }
        public List<List<double>> independentDataLists { get; set; } = new List<List<double>>();
        public Dictionary<string, double> WhatIfInputs { get; set; } = new Dictionary<string, double>();
        public bool ShowResults { get; set; } = false;
        public double? CalculatedIntercept { get; set; }
        public List<(string Variable, double Slope)> CalculatedSlopes { get; set; } = new List<(string Variable, double Slope)>();
        public double expectedOutcome { get; set; }

        public class VariableSlopePair
        {
            public string Variable { get; set; }
            public double Slope { get; set; }
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

                LoadData();
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login");
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("Starting OnPost");
            // Ensure data is loaded (this might already be handled in OnGet or another place)
            LoadData();

            // Check for data validity or early returns that might be causing issues
            if (Data == null || Data.Rows.Count == 0)
            {
                Console.WriteLine("No data loaded or data is null.");
                return Page(); // Make sure this is the intended behavior
            }

            // Print Independent Variables
            Console.WriteLine("Independent Variables:");
            foreach (var variable in IndependentVariables)
            {
                Console.WriteLine(variable);
            }

            // Print Dependent Variable
            Console.WriteLine($"Dependent Variable: {DependentVariable}");

            // Assuming PopulateDataListsFromDataTable method correctly populates the lists
            PopulateDataListsFromDataTable();

            // Print data lists
            Console.WriteLine("Independent Variables Data Lists:");
            foreach (var list in independentDataLists)
            {
                Console.WriteLine($"[{string.Join(", ", list)}]");
            }

            Console.WriteLine("Dependent Data List:");
            Console.WriteLine($"[{string.Join(", ", dependentDataList)}]");

            // Perform Regression
            CalculateAndSetMultipleRegression();

            // Print Regression Results
            Console.WriteLine($"Intercept: {Intercept}");
            Console.WriteLine("Slopes:");
            foreach (var slope in Slopes)
            {
                Console.WriteLine(slope);
            }

            var result = new
            {
                Intercept = Intercept,
                Slopes = Slopes.Select((slope, index) => new { Variable = IndependentVariables[index], Slope = slope }).ToList()
            };

            // Return the results as JSON
            //return new ContentResult
            //{
            //    Content = JsonSerializer.Serialize(result),
            //    ContentType = "application/json",
            //    StatusCode = 200
            //};

            // Instead of returning JSON, add results to ViewData or a property of the model
            CalculatedIntercept = Intercept;
            CalculatedSlopes = Slopes.Select((slope, index) => (IndependentVariables[index], slope)).ToList();

            // Return the same page to display results
            ViewData["RegressionResults"] = new
            {
                Intercept = Intercept,
                Slopes = Slopes.Select((slope, index) => new { Variable = IndependentVariables[index], Slope = slope }).ToList()
            };

            HttpContext.Session.SetString("Intercept", JsonSerializer.Serialize(CalculatedIntercept));

            // Assuming CalculatedSlopes is a list of (Variable, Slope)
            var slopesDictionary = CalculatedSlopes.ToDictionary(pair => pair.Variable, pair => pair.Slope);
            HttpContext.Session.SetString("Slopes", JsonSerializer.Serialize(slopesDictionary));

            ShowResults = true;

            // Serialize the data to be sent
            var interceptJson = JsonSerializer.Serialize(CalculatedIntercept);
            var slopesJson = JsonSerializer.Serialize(Slopes);
            var variablesJson = JsonSerializer.Serialize(IndependentVariables);
            var dependentVariableJson = JsonSerializer.Serialize(DependentVariable);

            Console.WriteLine("=======================");
            Console.WriteLine(interceptJson);
            Console.WriteLine(slopesJson);
            Console.WriteLine(variablesJson);
            Console.WriteLine(dependentVariableJson);
            Console.WriteLine("=======================");

            // Store data in session
            HttpContext.Session.SetString("Intercept", interceptJson);
            HttpContext.Session.SetString("Slopes", slopesJson);
            HttpContext.Session.SetString("Variables", variablesJson);
            HttpContext.Session.SetString("DependentVariable", dependentVariableJson);

            // Redirect to the receiving page
            return RedirectToPage("RegressionOutput");

        }

        private void LoadData()
        {
            // Initialization logic for Data property
            datasetID = (int)HttpContext.Session.GetInt32("datasetID"); // Provide a default value to avoid null issues
            datasetName = DBClass.ExtractDatasetName(datasetID);
            DBClass.MainDBconnection.Close();
            Data = DBClass.FetchDataForTable(datasetName);
            DBClass.MainDBconnection.Close();
            Console.WriteLine($"Data Loaded for Dataset: {datasetName}{datasetID}");
        }
        public static MultiRegressionModel CalculateMultipleRegression(List<double> yVals, List<List<double>> xVals)
        {
            var interceptColumn = Enumerable.Repeat(1.0, yVals.Count).ToList();
            xVals.Insert(0, interceptColumn);

            Console.WriteLine("Performing Multiple Regression Calculation...");

            var xMatrix = Matrix<double>.Build.DenseOfColumns(xVals);
            var yVector = Vector<double>.Build.DenseOfEnumerable(yVals);
            var coefficients = xMatrix.TransposeThisAndMultiply(xMatrix).Inverse().Multiply(xMatrix.TransposeThisAndMultiply(yVector));

            var intercept = coefficients[0];
            var slopes = new List<double>();
            for (int i = 1; i < coefficients.Count; i++)
            {
                slopes.Add(coefficients[i]);
            }

            Console.WriteLine($"Regression Calculation Complete. Intercept: {intercept}, Slopes: [{string.Join(", ", slopes)}]");

            return new MultiRegressionModel { Intercept = intercept, Slopes = slopes };
        }
        public void CalculateAndSetMultipleRegression()
        {
            Console.WriteLine("Preparing to Calculate Multiple Regression...");
            PopulateDataListsFromDataTable();
            var result = CalculateMultipleRegression(dependentDataList, independentDataLists);
            Intercept = result.Intercept;
            Slopes = result.Slopes;
            Console.WriteLine($"Regression Results: Intercept = {Intercept}, Slopes = [{string.Join(", ", Slopes)}]");
        }
        public void OnPostCreateWhatIf()
        {
            Console.WriteLine("Creating What-If Scenario Result");
            var interceptString = HttpContext.Session.GetString("Intercept");
            var slopesString = HttpContext.Session.GetString("Slopes");

            Console.WriteLine(interceptString.ToString());
            Console.WriteLine(slopesString.ToString());

            double expectedOutcome = 0;

            if (interceptString != null && slopesString != null)
            {
                var intercept = JsonSerializer.Deserialize<double>(interceptString);
                var slopesDictionary = JsonSerializer.Deserialize<Dictionary<string, double>>(slopesString);

                expectedOutcome += intercept; // Start with the intercept

                foreach (var slope in slopesDictionary)
                {
                    // Retrieve the user input value for each variable
                    string inputFieldName = $"WhatIfInputs[{slope.Key}]";
                    if (HttpContext.Request.Form.TryGetValue(inputFieldName, out var inputValue) && double.TryParse(inputValue, out double variableValue))
                    {
                        // Apply the regression equation component for this variable
                        expectedOutcome += slope.Value * variableValue;
                    }
                }
                Console.WriteLine(expectedOutcome.ToString());

                // Store or display the expected outcome as needed
                // For example, adding it to ViewData to display in the Razor view\

                ViewData["ExpectedY"] = expectedOutcome;
                ShowResults = true;
            }
            else
            {
                // Handle the case where the model components are not found in the session
                ViewData["Error"] = "Regression model components not found.";
                ShowResults = false;

            }
        }

        private void PopulateDataListsFromDataTable()
        {
            dependentDataList.Clear();
            independentDataLists.Clear(); // Clear existing data

            // Check if Data is null or empty, or if DependentVariable is null or empty
            if (Data == null || Data.Rows.Count == 0 || string.IsNullOrEmpty(DependentVariable))
            {
                Console.WriteLine("Data is null, empty, or DependentVariable is not set.");
                return; // Early exit if conditions are not met
            }
            string valueToProcess;
            // Populating dependent variable list
            foreach (DataRow row in Data.Rows)
            {
                //valueToProcess = row[DependentVariable]?.ToString();
                //valueToProcess = valueToProcess.Replace("$", "").Replace("(", "-").Replace(")", "").Replace(",","");
                // Here, ensure that the column exists to avoid the ArgumentNullException
                if (Data.Columns.Contains(DependentVariable) && double.TryParse(row[DependentVariable]?.ToString().Replace("$", "").Replace("(", "-").Replace(")", "").Replace(",", ""), out double dependentValue))
                {
                    dependentDataList.Add(dependentValue);
                }
            }

            // Populating independent variables lists
            foreach (var independentVariable in IndependentVariables)
            {
                // Ensure that each independent variable exists in the DataColumn collection
                if (!Data.Columns.Contains(independentVariable))
                {
                    continue; // Skip if the column does not exist
                }

                var tempList = new List<double>();
                foreach (DataRow row in Data.Rows)
                {
                    if (double.TryParse(row[independentVariable]?.ToString().Replace("$", "").Replace("(", "-").Replace(")", "").Replace(",", ""), out double independentValue))
                    {
                        tempList.Add(independentValue);
                    }
                }
                independentDataLists.Add(tempList); // Add the list for the current independent variable
            }
        }

        // Helper class to deserialize JSON from session
        public class RegressionModel
        {
            public double Intercept { get; set; }
            public List<VariableSlopePair> Slopes { get; set; }
        }

    }
}
