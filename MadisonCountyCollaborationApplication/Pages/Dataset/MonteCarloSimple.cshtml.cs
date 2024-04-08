using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plotly.NET.CSharp;
using Newtonsoft.Json;
using MadisonCountyCollaborationApplication.Pages.DataClasses;
using MathNet.Numerics.Random;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class MonteCarloSimpleModel : PageModel
    {
        [BindProperty]
        public int iterations { get; set; }
        [BindProperty]
        public int years { get; set; }
        [BindProperty]
        public Parameters Taxes { get; set; }
        [BindProperty]
        public Parameters Gov { get; set; }
        [BindProperty]
        public Parameters Other { get; set; }
        [BindProperty]
        public double confidenceInterval { get; set; }
        public string ChartConfigJson { get; private set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login");
            }
        }
        public IActionResult OnPost()
        {
            double[] results = RevenueSimple(iterations, years, Taxes, Gov, Other);
            var chart = Chart.Histogram<double, double, string>(X: results)
                .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("Revenue"))
                .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("Frequency"));

            double[] CI = new Simulation().ConfidenceInterval(results, confidenceInterval);

            string confidence = confidenceInterval + "% Confidence Interval[" + String.Format("{0:0}", CI[0])  + "," + String.Format("{0:0}", CI[1])  + "]";


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
                Confidence = confidence
                // This is your existing data structure
            };
            var jsonResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.None
            });
            return Content(jsonResponse, "application/json");
        }
        public IActionResult OnPostRevComp()
        {
            return RedirectToPage("MonteCarloRevComp");
        }
        public IActionResult OnPostExpense()
        {
            return RedirectToPage("MonteCarloExpense");
        }
        public IActionResult OnPostHelp()
        {
            return RedirectToPage("MonteCarloHelp");
        }
        //parameters follow: (current value, distribution name, statistical parameters)

        public double[] RevenueSimple(int iterations, int years, Parameters taxParameters,
            Parameters intergovernmentalParameters, Parameters otherParameters)
        {
            double[] revenues = new double[iterations];
            Simulation sim = new Simulation();
            //assigning distribution
            DataClasses.Distribution dist1 = sim.AssignDistribution(taxParameters);
            Console.WriteLine("Taxes");
            DataClasses.Distribution dist2 = sim.AssignDistribution(intergovernmentalParameters);
            Console.WriteLine("Gov");
            DataClasses.Distribution dist3 = sim.AssignDistribution(otherParameters);
            Console.WriteLine("Other");

            //conducting simulation
            double tax;
            double gov;
            double other;
            for (int i = 0; i < iterations; i++)
            {
                tax = sim.GenerateResult(dist1, Convert.ToDouble(taxParameters.initial), taxParameters.growth, years);
                gov = sim.GenerateResult(dist2, Convert.ToDouble(intergovernmentalParameters.initial), intergovernmentalParameters.growth, years);
                other = sim.GenerateResult(dist3, Convert.ToDouble(otherParameters.initial), otherParameters.growth, years);
                revenues[i] = tax + gov + other;
            }



            return revenues;
        }

    }
}
