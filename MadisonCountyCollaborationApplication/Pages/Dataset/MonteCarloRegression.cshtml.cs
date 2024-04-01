using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.CommandLine.IO;
using System.Data;
using System.Text.Json;


namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class MonteCarloRegressionModel : PageModel
    {

        [BindProperty]
        public string ProcessName { get; set; }
        [BindProperty]
        public string DatasetName { get; set; }

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
        public double ConfidenceLevel { get; set; } = .05;
        [BindProperty]
        public double Aplha { get; set; } = .05;
        [BindProperty]
        public int iterations { get; set; }
        [BindProperty]
        public int years { get; set; }
        [BindProperty]
        public double confidenceInterval { get; set; }
        public string ChartConfigJson { get; private set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for " + HttpContext.Session.GetString("username") + " successful!";

                //get process name
                ProcessName = HttpContext.Session.GetString("processName");

                //get dataset name
                DatasetName = HttpContext.Session.GetString("datasetName");


                // Retrieve data from session
                var interceptJson = HttpContext.Session.GetString("Intercept");
                var slopesJson = HttpContext.Session.GetString("Slopes");
                var variablesJson = HttpContext.Session.GetString("Variables");
                var dependentVariableJson = HttpContext.Session.GetString("DependentVariable");
                var standardErrorsJson = HttpContext.Session.GetString("StandardError");
                var pValuesJson = HttpContext.Session.GetString("PValues");
                var confidenceLevelJson = HttpContext.Session.GetString("ConfidenceLevel");
                Console.WriteLine("variables: " + variablesJson);
                // Deserialize data
                Intercept = JsonSerializer.Deserialize<double>(interceptJson);
                Slopes = JsonSerializer.Deserialize<List<double>>(slopesJson);
                IndependentVariables = JsonSerializer.Deserialize<List<string>>(variablesJson);
                DependentVariable = JsonSerializer.Deserialize<string>(dependentVariableJson);
                Console.WriteLine("----------------------------------------");
                foreach (string variable in IndependentVariables)
                {
                    Console.WriteLine(variable);
                }

                Console.WriteLine("count: "+ IndependentVariables.Count);

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
        public IActionResult OnPostHelp()
        {
            return RedirectToPage("MonteCarloHelp");
        }
    }
}

