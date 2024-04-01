using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MadisonCountyCollaborationApplication.Pages.DB;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Plotly.NET.CSharp;
using Plotly.NET.ImageExport;
using Plotly.NET.Interactive;
using System.Text.Json;
using Newtonsoft.Json;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System;
using System.IO;
using System.Threading.Tasks;


namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class SingleRegressionModel : PageModel
    {
        [BindProperty]
        public string ProcessName { get; set; }
        [BindProperty]
        public string DatasetName { get; set; }

        [BindProperty]
        public string IndependentVariable { get; set; }
        [BindProperty]
        public string DependentVariable { get; set; }
        public DataTable Data { get; private set; }
        [BindProperty]
        public int datasetID { get; set; }
        [BindProperty]
        public string datasetName { get; set; }
        public List<double> dependentDataList { get; set; } = new List<double>();
        public List<double> independentDataList { get; set; } = new List<double>();
        public string PlotImageBase64 { get; set; }
        public string ChartConfigJson { get; private set; }

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
        public IActionResult OnPostCreateGraph(string Independent, string Dependent)
        {
            IndependentVariable = Independent;
            DependentVariable = Dependent;
            LoadData();
            string dataSet = datasetName;
            // Assume Datasets_DBClass.AttributeReader<T> is adjusted to return List<string> for validation
            List<string> dependentDataAsString = Datasets_DBClass.AttributeReader<string>(Dependent, dataSet);
            Datasets_DBClass.MainDBconnection.Close();
            List<string> independentDataAsString = Datasets_DBClass.AttributeReader<string>(Independent, dataSet);
            Datasets_DBClass.MainDBconnection.Close();

            for(int i = 0; i<dependentDataAsString.Count(); i++)
            {
                dependentDataAsString[i] = dependentDataAsString[i].Replace("$", "").Replace("(", "-").Replace(")", "").Replace(",", "");
            }
            for (int i = 0; i < dependentDataAsString.Count(); i++)
            {
                independentDataAsString[i] = independentDataAsString[i].Replace("$", "").Replace("(", "-").Replace(")", "").Replace(",", "");
            }
            Console.WriteLine("==============================");
            Console.WriteLine(dependentDataAsString.ToString());
            Console.WriteLine(independentDataAsString.ToString());
            Console.WriteLine("==============================");

            // Validation: Check if all values are numeric
            bool isDependentNumeric = dependentDataAsString.All(str => double.TryParse(str, out _));
            bool isIndependentNumeric = independentDataAsString.All(str => double.TryParse(str, out _));

            if (!isDependentNumeric || !isIndependentNumeric)
            {
                // Not all values are numeric; return an error message
                ViewData["ErrorMessage"] = "One or both of the selected variables contain non-numeric data. Please select different variables.";
                return Page(); // Return to the view with error
            }


            // Convert the string lists to double lists since they are all numeric
            List<double> dependentDataList = dependentDataAsString.Select(double.Parse).ToList();
            List<double> independentDataList = independentDataAsString.Select(double.Parse).ToList();
            var (m, b, rSquared) = CalculateLinearRegression(independentDataList, dependentDataList);

            // Creating Chart Output 
            var chart = Chart.Point<double, double, string>(
                x: independentDataList.ToArray(),
                y: dependentDataList.ToArray()
            )
            .WithTraceInfo("Data Points", ShowLegend: true)
            .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("Independent Variable"))
            .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("Dependent Variable"));

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Ignore circular references
                TypeNameHandling = TypeNameHandling.None, // Additional setting to avoid $type insertion
                Formatting = Formatting.None // Use None for smaller payload; use Indented for readable JSON
            };

            var chartJson = JsonConvert.SerializeObject(chart, settings);
            ChartConfigJson = chartJson;

            var regressionEquation = $"y = {m:F3}x + {b:F3}";
            Console.WriteLine(regressionEquation.ToString());

            // Prepare the response object, now including the regression equation
            var response = new
            {
                Data = new { Fields = new[] { independentDataList, dependentDataList } }, // This is your existing data structure
                RegressionEquation = regressionEquation,
                RegressionLine = new { M = m, B = b } // Include regression line details for plotting
            };

            // Convert to JSON and return
            var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.None
            });
            Console.WriteLine(jsonResponse.ToString());
            return Content(jsonResponse, "application/json");
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

        public (double m, double b, double rSquared) CalculateLinearRegression(List<double> xVals, List<double> yVals)
        {
            if (xVals.Count != yVals.Count)
                throw new InvalidOperationException("The lists must have the same number of elements.");

            int N = xVals.Count;
            double sumX = xVals.Sum();
            double sumY = yVals.Sum();
            double sumXxY = xVals.Zip(yVals, (x, y) => x * y).Sum();
            double sumXsquared = xVals.Select(x => x * x).Sum();
            double sumYsquared = yVals.Select(y => y * y).Sum(); // Optional, for other calculations like R-squared

            double m = (N * sumXxY - sumX * sumY) / (N * sumXsquared - sumX * sumX);
            double b = (sumY - m * sumX) / N;

            double rSquared = Math.Pow(xVals.Zip(yVals, (x, y) => (x - xVals.Average()) * (y - yVals.Average())).Sum() / Math.Sqrt((xVals.Select(x => Math.Pow(x - xVals.Average(), 2)).Sum()) * (yVals.Select(y => Math.Pow(y - yVals.Average(), 2)).Sum())), 2);


            return (m, b, rSquared);
        }    
    }
}
