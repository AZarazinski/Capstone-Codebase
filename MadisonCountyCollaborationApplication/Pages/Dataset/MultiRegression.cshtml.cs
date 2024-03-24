using MadisonCountyCollaborationApplication.Pages.DB;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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

        [BindProperty]
        public string IndependentVariablesInput { get; set; }


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
        public async Task<IActionResult> OnPostAsync(string DependentVariable, string IndependentVariables)
        {
            // Your existing logic to set up and perform the regression analysis...

            // Assuming CalculateAndSetMultipleRegression populates Intercept and Slopes correctly
            CalculateAndSetMultipleRegression();
            Console.WriteLine("HI");

            // Log to the console
            Console.WriteLine($"Intercept: {Intercept}");
            if (Slopes != null)
            {
                foreach (var slope in Slopes)
                {
                    Console.WriteLine($"Slope: {slope}");
                }
            }
            var result = new
            {
                Intercept = Intercept,
                Slopes = Slopes.Select((slope, index) => new { Variable = IndependentVariables[index], Slope = slope }).ToList()
            };

            // Return the results as JSON
            return new ContentResult
            {
                Content = JsonSerializer.Serialize(result),
                ContentType = "application/json",
                StatusCode = 200
            };
        }


        private void LoadData()
        {
            // Initialization logic for Data property
            datasetID = (int)HttpContext.Session.GetInt32("datasetID"); // Provide a default value to avoid null issues
            datasetName = DBClass.ExtractDatasetName(datasetID);
            DBClass.MainDBconnection.Close();
            Data = DBClass.FetchDataForTable(datasetName + datasetID.ToString());
            DBClass.MainDBconnection.Close();
        }

        public static MultiRegressionModel CalculateMultipleRegression(List<double> yVals, List<List<double>> xVals)
        {
            // Adding a column of 1s for the intercept term
            var interceptColumn = Enumerable.Repeat(1.0, yVals.Count).ToList();
            xVals.Insert(0, interceptColumn);

            var xMatrix = Matrix<double>.Build.DenseOfColumns(xVals);
            var yVector = Vector<double>.Build.DenseOfEnumerable(yVals);

            // Solving the linear system for regression coefficients
            var coefficients = xMatrix.TransposeThisAndMultiply(xMatrix).Inverse().Multiply(xMatrix.TransposeThisAndMultiply(yVector));

            // Extracting the intercept and slopes
            var intercept = coefficients[0];
            var slopes = new List<double>();
            for (int i = 1; i < coefficients.Count; i++)
            {
                slopes.Add(coefficients[i]);
            }

            // Placeholder for the rest of the calculation
            return new MultiRegressionModel { Intercept = intercept, Slopes = slopes };
        }

        public void CalculateAndSetMultipleRegression()
        {
            PopulateDataListsFromDataTable();

            // Now passing the list of lists for independent variables and the single list for the dependent variable
            var result = CalculateMultipleRegression(dependentDataList, independentDataLists); // Assuming dependentDataList is correctly populated

            Intercept = result.Intercept;
            Slopes = result.Slopes;
        }

        private void PopulateDataListsFromDataTable()
        {
            dependentDataList.Clear();
            independentDataLists.Clear(); // Use the corrected variable name

            if (Data != null && Data.Rows.Count > 0)
            {
                // Populating dependent variable list
                foreach (DataRow row in Data.Rows)
                {
                    if (double.TryParse(row[DependentVariable].ToString(), out double dependentValue))
                    {
                        dependentDataList.Add(dependentValue);
                    }
                }

                // Populating independent variables lists
                foreach (var independentVariable in IndependentVariables)
                {
                    var tempList = new List<double>();
                    foreach (DataRow row in Data.Rows)
                    {
                        if (double.TryParse(row[independentVariable].ToString(), out double independentValue))
                        {
                            tempList.Add(independentValue);
                        }
                    }
                    independentDataLists.Add(tempList); // Now correctly using the list of lists
                }
                // Assuming you adjust your regression method to take List<List<double>> for independent variables
            }
        }
    }
}
