using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.CommandLine.IO;
using System.Data;
using MadisonCountyCollaborationApplication.Pages.DataClasses;
using Newtonsoft.Json;
using Plotly.NET.CSharp;



namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class MonteCarloRegressionModel : PageModel
    {
        [BindProperty]
        public List<string> IndependentVariables { get; set; }
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
        public bool ShowResults { get; set; } = false;
        [BindProperty]
        public double ConfidenceLevel { get; set; } = .05;
        [BindProperty]
        public double Aplha { get; set; } = .05;
        [BindProperty]
        public int iterations { get; set; }
        [BindProperty]
        public int years { get; set; }
        [BindProperty]
        public double confidenceInterval { get; set; }
        [BindProperty]
        public List<Parameters> parameters { get; set; }
        [BindProperty]
        public Parameters param0 { get; set; }
        [BindProperty]
        public Parameters param1 { get; set; }
        [BindProperty]
        public Parameters param2 { get; set; }
        [BindProperty]
        public Parameters param3 { get; set; }
        [BindProperty]
        public Parameters param4 { get; set; }
        [BindProperty]
        public Parameters param5 { get; set; }
        [BindProperty]
        public Parameters param6 { get; set; }
        [BindProperty]
        public Parameters param7 { get; set; }
        [BindProperty]
        public Parameters param8 { get; set; }
        [BindProperty]
        public Parameters param9 { get; set; }

        public string ChartConfigJson { get; private set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for " + HttpContext.Session.GetString("username") + " successful!";

                // Retrieve data from session
                var variablesJson = HttpContext.Session.GetString("Variables");
                var dependentVariableJson = HttpContext.Session.GetString("DependentVariable");
                // Deserialize data
                IndependentVariables = System.Text.Json.JsonSerializer.Deserialize<List<string>>(variablesJson);
                DependentVariable = System.Text.Json.JsonSerializer.Deserialize<string>(dependentVariableJson);

                // Optionally, you can call other methods here to perform additional initialization or processing

                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login");
            }
        }
        public IActionResult OnPostHelp()
        {
            return RedirectToPage("MonteCarloHelp");
        }
        public IActionResult OnPost()
        {
            var interceptJson = HttpContext.Session.GetString("Intercept");
            var slopesJson = HttpContext.Session.GetString("Slopes");
            var variablesJson = HttpContext.Session.GetString("Variables");
            var dependentVariableJson = HttpContext.Session.GetString("DependentVariable");

            Intercept = System.Text.Json.JsonSerializer.Deserialize<double>(interceptJson);
            Slopes = System.Text.Json.JsonSerializer.Deserialize<List<double>>(slopesJson);
            IndependentVariables = System.Text.Json.JsonSerializer.Deserialize<List<string>>(variablesJson);
            DependentVariable = System.Text.Json.JsonSerializer.Deserialize<string>(dependentVariableJson);

            for (int i = 0; i < IndependentVariables.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        param0.beta = Slopes[i];
                        parameters.Add(param0);
                        break;
                    case 1:
                        param1.beta = Slopes[i];
                        parameters.Add(param1);
                        break;
                    case 2:
                        param2.beta = Slopes[i];
                        parameters.Add(param2);
                        break;
                    case 3:
                        param3.beta = Slopes[i];
                        parameters.Add(param3);
                        break;
                    case 4:
                        param4.beta = Slopes[i];
                        parameters.Add(param4);
                        break;
                    case 5:
                        param5.beta = Slopes[i];
                        parameters.Add(param5);
                        break;
                    case 6:
                        param6.beta = Slopes[i];
                        parameters.Add(param6);
                        break;
                    case 7:
                        param7.beta = Slopes[i];
                        parameters.Add(param7);
                        break;
                    case 8:
                        param8.beta = Slopes[i];
                        parameters.Add(param8);
                        break;
                    case 9:
                        param9.beta = Slopes[i];
                        parameters.Add(param9);
                        break;
                }
            }
            
            double[] results = MCRegression(iterations, years, parameters, (double)Intercept);
            double[] CI = new Simulation().ConfidenceInterval(results, confidenceInterval);
            
            var chart = Chart.Histogram<double, double, string>(X: results)
                .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("Revenue"))
                .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("Frequency"));

            //var chart = Chart.Histogram(x:results.ToList());

            //.WithTraceInfo("Data Points", ShowLegend: true)
            //.WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("Independent Variable"))
            //.WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("Dependent Variable"));
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Ignore circular references
                TypeNameHandling = TypeNameHandling.None, // Additional setting to avoid $type insertion
                Formatting = Formatting.None // Use None for smaller payload; use Indented for readable JSON
            };

            var chartJson = JsonConvert.SerializeObject(chart, settings);
            ChartConfigJson = chartJson;
            var response = new
            {
                Data = new { Fields = new[] { results } } // This is your existing data structure
            };
            var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.None
            });
            Console.WriteLine(jsonResponse);
            return Content(jsonResponse, "application/json");
        }

        public double[] MCRegression(int iterations, int years, List<Parameters> param, double intercept)
        {
            double[] revenues = new double[iterations];
            Simulation sim = new Simulation();
            //assigning distribution
            List<Distribution> dist = new List<Distribution>();

            foreach (Parameters p in param)
            {
                dist.Add(sim.AssignDistribution(p));
            }

            //conducting simulation
            double[] revs = new double[param.Count];

            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < param.Count; j++)
                {
                    revs[j] = sim.GenerateResult(dist[j], Convert.ToDouble(param[j].initial), param[j].growth, years) * param[j].beta;
                }
                revenues[i] = intercept + revs.Sum();
            }
            
            return revenues;
        }
    }
}

