using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Serialization;
//using System.Text.Json;
using Newtonsoft.Json;
using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MadisonCountyCollaborationApplication.Pages.DB;
using Plotly.NET.CSharp;



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
        public List<Parameters> parameters { get; set; } = new List<Parameters>();
        [BindProperty]
        public Parameters param0 { get; set; } = new Parameters();
        [BindProperty]
        public Parameters param1 { get; set; } = new Parameters();
        [BindProperty]
        public Parameters param2 { get; set; } = new Parameters();
        [BindProperty]
        public Parameters param3 { get; set; } = new Parameters();
        [BindProperty]
        public Parameters param4 { get; set; } = new Parameters();
        [BindProperty]
        public Parameters param5 { get; set; } = new Parameters();
        [BindProperty]
        public Parameters param6 { get; set; } = new Parameters();
        [BindProperty]
        public Parameters param7 { get; set; } = new Parameters();
        [BindProperty]
        public Parameters param8 { get; set; } = new Parameters();
        [BindProperty]
        public Parameters param9 { get; set; } = new Parameters();
        [BindProperty]
        public double[] results { get; set; }
        [BindProperty]
        public string confidence { get; set; }
        public string ChartConfigJson { get; private set; }
        public int dataID { get; set; }
        public List<double> WhatIfValues { get; set; } = new List<double>();


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

                //var whatIfValuesJson = HttpContext.Session.GetString("WhatIfValues");
                //if (!string.IsNullOrEmpty(whatIfValuesJson))
                //{
                //    WhatIfValues = JsonSerializer.Deserialize<List<double>>(whatIfValuesJson);
                //}

                LowerBound = double.Parse(lowerJson);
                UpperBound = double.Parse(upperJson);
                ExpectedOutcome = double.Parse(HttpContext.Session.GetString("ExpectedOutcome"));
                ConfidenceLevel = System.Text.Json.JsonSerializer.Deserialize<double>(confidenceLevelJson);
                IndependentVariables = System.Text.Json.JsonSerializer.Deserialize<List<string>>(variablesJson);
                DependentVariable = System.Text.Json.JsonSerializer.Deserialize<string>(dependentVariableJson);
                //get process name
                ProcessName = HttpContext.Session.GetString("processName");

                //get dataset name
                DatasetName = HttpContext.Session.GetString("datasetName");


                Values = new List<double> { (LowerBound * .9), LowerBound, ExpectedOutcome, (UpperBound * 1.1), UpperBound };

                var interceptJson = HttpContext.Session.GetString("Intercept");
                var slopesJson = HttpContext.Session.GetString("Slopes");
                Intercept = System.Text.Json.JsonSerializer.Deserialize<double>(interceptJson);
                Slopes = System.Text.Json.JsonSerializer.Deserialize<List<double>>(slopesJson);
                IndependentVariables = System.Text.Json.JsonSerializer.Deserialize<List<string>>(variablesJson);
                DependentVariable = System.Text.Json.JsonSerializer.Deserialize<string>(dependentVariableJson).Replace("_", " ");

                dataID = (int)HttpContext.Session.GetInt32("datasetID");
                DatasetName = DBClass.ExtractDatasetName(dataID);
                DBClass.MainDBconnection.Close();
                string[] inputs = HttpContext.Session.GetString("inputs").ToString().Split(",");
                for (int i = 0; i < IndependentVariables.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            param0.initial = inputs[i];
                            param0.beta = Slopes[i];
                            param0.param1 = inputs[i];
                            param0.param2 = Datasets_DBClass.AccountDeviation(DatasetName, IndependentVariables[i]);
                            Datasets_DBClass.MainDBconnection.Close();
                            param0.dist = "Normal";
                            param0.growth = "None";
                            parameters.Add(param0);
                            break;
                        case 1:
                            param1.initial = inputs[i];
                            param1.beta = Slopes[i];
                            param1.param1 = inputs[i];
                            param1.param2 = Datasets_DBClass.AccountDeviation(DatasetName, IndependentVariables[i]);
                            Datasets_DBClass.MainDBconnection.Close();
                            param1.dist = "Normal";
                            param1.growth = "None";
                            parameters.Add(param1);
                            break;
                        case 2:
                            param2.initial = inputs[i];
                            param2.beta = Slopes[i];
                            param2.param1 = inputs[i];
                            param2.param2 = Datasets_DBClass.AccountDeviation(DatasetName, IndependentVariables[i]);
                            Datasets_DBClass.MainDBconnection.Close();
                            param2.dist = "Normal";
                            param2.growth = "None";
                            parameters.Add(param2);
                            break;
                        case 3:
                            param3.initial = inputs[i];
                            param3.beta = Slopes[i];
                            param3.param1 = inputs[i];
                            param3.param2 = Datasets_DBClass.AccountDeviation(DatasetName, IndependentVariables[i]);
                            Datasets_DBClass.MainDBconnection.Close();
                            param3.dist = "Normal";
                            param3.growth = "None";
                            parameters.Add(param3);
                            break;
                        case 4:
                            param4.initial = inputs[i];
                            param4.beta = Slopes[i];
                            param4.param1 = inputs[i];
                            param4.param2 = Datasets_DBClass.AccountDeviation(DatasetName, IndependentVariables[i]);
                            Datasets_DBClass.MainDBconnection.Close();
                            param4.dist = "Normal";
                            param4.growth = "None";
                            parameters.Add(param4);
                            break;
                        case 5:
                            param5.initial = inputs[i];
                            param5.beta = Slopes[i];
                            param5.param1 = inputs[i];
                            param5.param2 = Datasets_DBClass.AccountDeviation(DatasetName, IndependentVariables[i]);
                            Datasets_DBClass.MainDBconnection.Close();
                            param5.dist = "Normal";
                            param5.growth = "None";
                            parameters.Add(param5);
                            break;
                        case 6:
                            param6.initial = inputs[i];
                            param6.beta = Slopes[i];
                            param6.param1 = inputs[i];
                            param6.param2 = Datasets_DBClass.AccountDeviation(DatasetName, IndependentVariables[i]);
                            Datasets_DBClass.MainDBconnection.Close();
                            param6.dist = "Normal";
                            param6.growth = "None";
                            parameters.Add(param6);
                            break;
                        case 7:
                            param7.initial = inputs[i];
                            param7.beta = Slopes[i];
                            param7.param1 = inputs[i];
                            param7.param2 = Datasets_DBClass.AccountDeviation(DatasetName, IndependentVariables[i]);
                            Datasets_DBClass.MainDBconnection.Close();
                            param7.dist = "Normal";
                            param7.growth = "None";
                            parameters.Add(param7);
                            break;
                        case 8:
                            param8.initial = inputs[i];
                            param8.beta = Slopes[i];
                            param8.param1 = inputs[i];
                            param8.param2 = Datasets_DBClass.AccountDeviation(DatasetName, IndependentVariables[i]);
                            Datasets_DBClass.MainDBconnection.Close();
                            param8.dist = "Normal";
                            param8.growth = "None";
                            parameters.Add(param8);
                            break;
                        case 9:
                            param9.initial = inputs[i];
                            param9.beta = Slopes[i];
                            param9.param1 = inputs[i];
                            param9.param2 = Datasets_DBClass.AccountDeviation(DatasetName, IndependentVariables[i]);
                            Datasets_DBClass.MainDBconnection.Close();
                            param9.dist = "Normal";
                            param9.growth = "None";
                            parameters.Add(param9);
                            break;
                    }
                }

                results = MCRegression(10000, 1, parameters, (double)Intercept);
                double[] CI = new Simulation().ConfidenceInterval(results, 0.95);

                var chart = Chart.Histogram<double, double, string>(X: results)
                    .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("Revenue"))
                    .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("Frequency"));

                string confidence = "With a 95% certainty we can say that your " + DependentVariable +
                " 1 year out from today will be between " + String.Format("{0:0}", CI[0]) + " and " + String.Format("{0:0}", CI[0]);
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
                    Data = new { Fields = new[] { results } },
                    Confidence = confidence,
                    Dependent = DependentVariable
                    // This is your existing data structure
                };
                var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.None,
                    Formatting = Formatting.None
                });

                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login");
            }
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
        public IActionResult OnPostSimulate()
        {
            return RedirectToPage("MonteCarloRegression");
        }
    }
}
