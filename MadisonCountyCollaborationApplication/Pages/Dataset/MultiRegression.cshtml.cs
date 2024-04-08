using MadisonCountyCollaborationApplication.Pages.DB;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
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
        [BindProperty]
        public double ConfidenceLevel { get; set; } = 95; // Default to 95%
        public List<double> StandardError { get; set; }
        public double ConfidenceIntervalLower { get; set; }
        public double ConfidenceIntervalUpper { get; set; }
        public List<double> PValues { get; set; } = new List<double>();
        public int DegreesOfFreedom { get ; set; }
        [BindProperty]
        public string ProcessName { get; set; }
        [BindProperty]
        public string DatasetName { get; set; }


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
                //get process name
                ProcessName = HttpContext.Session.GetString("processName");

                //get dataset name
                DatasetName = HttpContext.Session.GetString("datasetName");
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
            IndependentVariables = Request.Form["IndependentVariables"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();            // Ensure data is loaded (this might already be handled in OnGet or another place)
            LoadData();

            // Check for data validity or early returns that might be causing issues
            if (Data == null || Data.Rows.Count == 0)
            {
                Console.WriteLine("No data loaded or data is null.");
                return Page(); // Make sure this is the intended behavior
            }

            // Assuming PopulateDataListsFromDataTable method correctly populates the lists
            PopulateDataListsFromDataTable();
            // Perform Regression
            CalculateAndSetMultipleRegression();

            var result = new
            {
                Intercept = Intercept,
                Slopes = Slopes.Select((slope, index) => new { Variable = IndependentVariables[index], Slope = slope }).ToList()
            };

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

            // Store data in session
            HttpContext.Session.SetString("Intercept", interceptJson);
            HttpContext.Session.SetString("Slopes", slopesJson);
            HttpContext.Session.SetString("Variables", variablesJson);
            HttpContext.Session.SetString("DependentVariable", dependentVariableJson);
            HttpContext.Session.SetString("DegreesOfFreedom", DegreesOfFreedom.ToString());



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
        }
        public static MultiRegressionModel CalculateMultipleRegression(List<double> yVals, List<List<double>> xVals)
        {
            var interceptColumn = Enumerable.Repeat(1.0, yVals.Count).ToList();
            xVals.Insert(0, interceptColumn);


            var xMatrix = Matrix<double>.Build.DenseOfColumns(xVals);
            var yVector = Vector<double>.Build.DenseOfEnumerable(yVals);
            var coefficients = xMatrix.TransposeThisAndMultiply(xMatrix).Inverse().Multiply(xMatrix.TransposeThisAndMultiply(yVector));

            var intercept = coefficients[0];
            var slopes = new List<double>();
            for (int i = 1; i < coefficients.Count; i++)
            {
                slopes.Add(coefficients[i]);
            }


            return new MultiRegressionModel { Intercept = intercept, Slopes = slopes };
        }
        public void CalculateAndSetMultipleRegression()
        {
            Console.WriteLine("Preparing to Calculate Multiple Regression...");
            PopulateDataListsFromDataTable();
            // Logic for regression calculation follows here
            var xMatrix = BuildXMatrix(independentDataLists);
            var yVector = Vector<double>.Build.DenseOfEnumerable(dependentDataList);
            var coefficients = CalculateMultipleRegression(dependentDataList, xMatrix);

            Intercept = coefficients[0];
            Slopes = coefficients.SubVector(1, coefficients.Count - 1).ToList();

            StandardError = CalculateStandardErrors(dependentDataList, xMatrix, coefficients);
            var pValues = CalculatePValues(coefficients, StandardError, dependentDataList.Count);

            double alpha = (1.0 - (ConfidenceLevel / 100.0));
            int degreesOfFreedom = dependentDataList.Count - coefficients.Count;
            Console.WriteLine(degreesOfFreedom.ToString());
            HttpContext.Session.SetInt32("DegreesOfFreedom", degreesOfFreedom);
            double criticalValue = StudentT.InvCDF(0, 1, degreesOfFreedom, 1 - alpha / 2.0);
            Console.WriteLine(criticalValue.ToString());
            HttpContext.Session.SetString("CriticalValue", criticalValue.ToString());


            ConfidenceIntervalLower = Intercept.Value - criticalValue * StandardError[0];
            ConfidenceIntervalUpper = Intercept.Value + criticalValue * StandardError[0];

            // Store the calculated values in the session for use on subsequent pages
            HttpContext.Session.SetString("StandardError", JsonSerializer.Serialize(StandardError));
            HttpContext.Session.SetString("PValues", JsonSerializer.Serialize(pValues));
            HttpContext.Session.SetString("ConfidenceLevel", ConfidenceLevel.ToString());
            HttpContext.Session.SetString("ConfidenceIntervalLower", ConfidenceIntervalLower.ToString());
            HttpContext.Session.SetString("ConfidenceIntervalUpper", ConfidenceIntervalUpper.ToString());
            var standardErrorJson = JsonSerializer.Serialize(StandardError);
            Console.WriteLine(standardErrorJson); // Ensure this isn't empty or "[]"
            HttpContext.Session.SetString("StandardError", standardErrorJson);
            Console.WriteLine($"Intercept p-value: {pValues[0]:G4}"); // Using general format specifier with precision
            for (int i = 0; i < Slopes.Count; i++)
            {
                Console.WriteLine($"{IndependentVariables[i]} p-value: {pValues[i + 1]:G4}");
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
        public List<double> CalculateStandardErrors(List<double> yVals, Matrix<double> xMatrix, Vector<double> coefficients)
        {
            // Reconstruct yVector from yVals within the scope
            var yVector = Vector<double>.Build.DenseOfEnumerable(yVals);

            var predictions = xMatrix * coefficients;
            var residuals = yVector - predictions;
            var rss = residuals.DotProduct(residuals);
            var mse = rss / (yVals.Count - coefficients.Count);
            var seList = new List<double>();

            var xTxInv = (xMatrix.TransposeThisAndMultiply(xMatrix)).Inverse();
            for (int i = 0; i < coefficients.Count; i++)
            {
                var variance = xTxInv[i, i] * mse;
                seList.Add(Math.Sqrt(variance));
            }

            return seList;
        }

        public List<double> CalculatePValues(Vector<double> coefficients, List<double> standardErrors, int sampleSize)
        {
            var pValues = new List<double>();
            int degreesOfFreedom = sampleSize - coefficients.Count; // Assuming one intercept + multiple slopes

            for (int i = 0; i < coefficients.Count; i++)
            {
                double tStatistic = coefficients[i] / standardErrors[i];
                // Two-tailed p-value
                double pValue = 2.0 * (1.0 - StudentT.CDF(0, 1, degreesOfFreedom, Math.Abs(tStatistic)));
                pValues.Add(pValue);
            }

            return pValues;
        }
        private Matrix<double> BuildXMatrix(List<List<double>> xVals)
        {
            var rowCount = xVals.First().Count;
            var columnCount = xVals.Count + 1; // Adding 1 for the intercept column
            var xMatrix = Matrix<double>.Build.Dense(rowCount, columnCount, 1.0); // Initialize with 1s for intercept

            for (int col = 1; col < columnCount; col++) // Start from 1 to skip intercept column
            {
                for (int row = 0; row < rowCount; row++)
                {
                    xMatrix[row, col] = xVals[col - 1][row]; // Adjusted indexing for xVals
                }
            }

            return xMatrix;
        }
        public Vector<double> CalculateMultipleRegression(List<double> yVals, Matrix<double> xMatrix)
        {
            var yVector = Vector<double>.Build.DenseOfEnumerable(yVals);
            var coefficients = xMatrix.TransposeThisAndMultiply(xMatrix).Inverse().Multiply(xMatrix.TransposeThisAndMultiply(yVector));
            return coefficients;
        }
    }
}
