using MadisonCountyCollaborationApplication.Pages.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Plotly.NET.CSharp;

namespace MadisonCountyCollaborationApplication.Pages
{
    public class MonteCarloExpenseModel : PageModel
    {
        [BindProperty]
        public int iterations { get; set; }
        [BindProperty]
        public int years { get; set; }

        [BindProperty]
        public Parameters Admin { get; set; }

        [BindProperty]
        public Parameters Judicial { get; set; }

        [BindProperty]
        public Parameters Safety { get; set; }

        [BindProperty]
        public Parameters Works { get; set; }
        [BindProperty]
        public Parameters Health { get; set; }
        [BindProperty]
        public Parameters Education { get; set; }
        public Parameters Parks { get; set; }
        [BindProperty]
        public Parameters Community { get; set; }

        [BindProperty]
        public Parameters Nondepartmental { get; set; }
        [BindProperty]
        public Parameters Capital { get; set; }
        [BindProperty]
        public Parameters Principal { get; set; }
        public Parameters Interest { get; set; }
        public double confidenceInterval { get; set; }
        public string ChartConfigJson { get; private set; }
        [BindProperty]
        public string ProcessName { get; set; }
        [BindProperty]
        public string DatasetName { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for "
                    + HttpContext.Session.GetString("username")
                    + " successful!";

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

        public IActionResult OnPost()
        {

            double[] results = ExpenseComplex(iterations, years, Admin, Judicial, Safety, Works, Health, Education, Parks,
                Community, Nondepartmental, Capital, Principal, Interest);
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
        public IActionResult OnPostRevComp()
        {
            return RedirectToPage("MonteCarloRevComp");
        }
        public IActionResult OnPostRevSimple()
        {
            return RedirectToPage("MonteCarlo");
        }
        public IActionResult OnPostHelp()
        {
            return RedirectToPage("MonteCarloHelp");
        }
        public double[] ExpenseComplex(int iterations, int years, Parameters administrationParameters,
            Parameters judicialParameters, Parameters safetyParameters, Parameters worksParameters, Parameters healthParameters,
            Parameters educationParameters, Parameters parksParameters, Parameters communityParameters,
            Parameters nondepartmentalParameters, Parameters capitalParameters, Parameters principalParameters,
            Parameters interestParameters)
        {
            double[] revenues = new double[iterations];
            //assigning distribution
            Simulation sim = new Simulation();
            DataClasses.Distribution dist1 = sim.AssignDistribution(administrationParameters);
            DataClasses.Distribution dist2 = sim.AssignDistribution(judicialParameters);
            DataClasses.Distribution dist3 = sim.AssignDistribution(safetyParameters);
            DataClasses.Distribution dist4 = sim.AssignDistribution(worksParameters);
            DataClasses.Distribution dist5 = sim.AssignDistribution(healthParameters);
            DataClasses.Distribution dist6 = sim.AssignDistribution(educationParameters);
            DataClasses.Distribution dist7 = sim.AssignDistribution(parksParameters);
            DataClasses.Distribution dist8 = sim.AssignDistribution(communityParameters);
            DataClasses.Distribution dist9 = sim.AssignDistribution(nondepartmentalParameters);
            DataClasses.Distribution dist10 = sim.AssignDistribution(capitalParameters);
            DataClasses.Distribution dist11 = sim.AssignDistribution(principalParameters);
            DataClasses.Distribution dist12 = sim.AssignDistribution(interestParameters);


            //conducting simulation
            double admin;
            double judicial;
            double safety;
            double work;
            double health;
            double education;
            double parks;
            double community;
            double nondepartmental;
            double capital;
            double principal;
            double interest;
            for (int i = 0; i < iterations; i++)
            {
                admin = sim.GenerateResult(dist1, Convert.ToDouble(administrationParameters.initial), administrationParameters.growth, years);
                judicial = sim.GenerateResult(dist2, Convert.ToDouble(judicialParameters.initial), judicialParameters.growth, years);
                safety = sim.GenerateResult(dist3, Convert.ToDouble(safetyParameters.initial), safetyParameters.growth, years);
                work = sim.GenerateResult(dist4, Convert.ToDouble(worksParameters.initial), worksParameters.growth, years);
                health = sim.GenerateResult(dist5, Convert.ToDouble(healthParameters.initial), healthParameters.growth, years);
                education = sim.GenerateResult(dist6, Convert.ToDouble(educationParameters.initial), educationParameters.growth, years);
                parks = sim.GenerateResult(dist7, Convert.ToDouble(parksParameters.initial), parksParameters.growth, years);
                community = sim.GenerateResult(dist8, Convert.ToDouble(communityParameters.initial), communityParameters.growth, years);
                nondepartmental = sim.GenerateResult(dist9, Convert.ToDouble(nondepartmentalParameters.initial), nondepartmentalParameters.growth, years);
                capital = sim.GenerateResult(dist10, Convert.ToDouble(capitalParameters.initial), capitalParameters.growth, years);
                principal = sim.GenerateResult(dist11, Convert.ToDouble(principalParameters.initial), principalParameters.growth, years);
                interest = sim.GenerateResult(dist12, Convert.ToDouble(interestParameters.initial), interestParameters.growth, years);
                revenues[i] = admin + judicial + safety + work + health + education + parks + community + nondepartmental + capital +
                        principal + interest;
            }
            double[] CI = sim.ConfidenceInterval(revenues, confidenceInterval);
            return revenues;
        }
    }
}

