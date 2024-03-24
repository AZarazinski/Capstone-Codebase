using MadisonCountyCollaborationApplication.Pages.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Plotly.NET.CSharp;


namespace MadisonCountyCollaborationApplication.Pages
{
    public class MonteCarloRevCompModel : PageModel
    {
        [BindProperty]
        public int iterations { get; set; }
        [BindProperty]
        public int years { get; set; }

        [BindProperty]
        public Parameters Property { get; set; }
        [BindProperty]
        public Parameters Local {  get; set; }
        [BindProperty]
        public Parameters Permits { get; set; }
        [BindProperty]
        public Parameters Fines { get; set; }
        [BindProperty]
        public Parameters UseOfMoney { get; set; }
        [BindProperty]
        public Parameters Services { get; set; }
        [BindProperty]
        public Parameters Miscellaneous { get; set; }
        [BindProperty]
        public Parameters Commonwealth { get; set; }
        [BindProperty]
        public Parameters Federal { get; set; }
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

            double[] results = RevenueComplex(iterations, years, Services, Permits, Property, Local, Miscellaneous, Fines, 
                UseOfMoney, Commonwealth, Federal);
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
            return Content(jsonResponse, "application/json");
        }
        public IActionResult OnPostRevSimple()
        {
            return RedirectToPage("/MonteCarlo");
        }
        public IActionResult OnPostExpense()
        {
            return RedirectToPage("/MonteCarloExpense");
        }
        public double[] RevenueComplex(int iterations, int years, Parameters serviceParameters, Parameters permitParameters, Parameters propertyParameters,
            Parameters localParameters, Parameters miscellaneousParameters, Parameters fineParameters, Parameters useOfMoneyParameters, Parameters commonwealthParameters, Parameters federalParameters)
        {
            double[] revenues = new double[iterations];
            Simulation sim = new Simulation();
            //assigning distribution
            DataClasses.Distribution dist1 = sim.AssignDistribution(serviceParameters);
            DataClasses.Distribution dist2 = sim.AssignDistribution(permitParameters);
            DataClasses.Distribution dist3 = sim.AssignDistribution(propertyParameters);
            DataClasses.Distribution dist4 = sim.AssignDistribution(localParameters);
            DataClasses.Distribution dist5 = sim.AssignDistribution(miscellaneousParameters);
            DataClasses.Distribution dist6 = sim.AssignDistribution(fineParameters);
            DataClasses.Distribution dist7 = sim.AssignDistribution(useOfMoneyParameters);
            DataClasses.Distribution dist8 = sim.AssignDistribution(commonwealthParameters);
            DataClasses.Distribution dist9 = sim.AssignDistribution(federalParameters);

            //conducting simulation
            double service;
            double permit;
            double property;
            double local;
            double misc;
            double fine;
            double money;
            double commonwealth;
            double federal;

            for (int i = 0; i < iterations; i++)
            {
                service = sim.GenerateResult(dist1, Convert.ToDouble(serviceParameters.initial), serviceParameters.growth, years);
                permit = sim.GenerateResult(dist2, Convert.ToDouble(permitParameters.initial), permitParameters.growth, years);
                property = sim.GenerateResult(dist3, Convert.ToDouble(propertyParameters.initial), propertyParameters.growth, years);
                local = sim.GenerateResult(dist4, Convert.ToDouble(localParameters.initial), localParameters.growth, years);
                misc = sim.GenerateResult(dist5, Convert.ToDouble(miscellaneousParameters.initial), miscellaneousParameters.growth, years);
                fine = sim.GenerateResult(dist6, Convert.ToDouble(fineParameters.initial), fineParameters.growth, years);
                money = sim.GenerateResult(dist7, Convert.ToDouble(useOfMoneyParameters.initial), useOfMoneyParameters.growth, years);
                commonwealth = sim.GenerateResult(dist8, Convert.ToDouble(commonwealthParameters.initial), commonwealthParameters.growth, years);
                federal = sim.GenerateResult(dist9, Convert.ToDouble(federalParameters.initial), federalParameters.growth, years);
                revenues[i] = service + permit + property + local + misc + fine + money + commonwealth + federal;
            }
            double[] CI = sim.ConfidenceInterval(revenues);
            return revenues;

        }
    }
}
