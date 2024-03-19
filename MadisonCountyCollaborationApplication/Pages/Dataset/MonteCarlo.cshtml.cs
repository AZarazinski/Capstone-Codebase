//using iTextSharp.text.pdf.parser.clipper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plotly.NET.CSharp;
using Newtonsoft.Json;
//using Org.BouncyCastle.Bcpg.OpenPgp;

namespace _488Labs.Pages
{
    public class MonteCarloModel : PageModel
    {
        [BindProperty]
        public int iterations { get; set; }
        [BindProperty]
        public int years { get; set; }
        [BindProperty]
        public bool constant {  get; set; }

        [BindProperty]
        public List<string> taxParams { get; set; }
        [BindProperty] 
        public List<string> govParams { get; set; }
        [BindProperty]
        public List<string> otherParams { get; set; }

        [BindProperty]
        public string taxInitial { get; set; }
        [BindProperty]
        public string taxDist { get; set; }
        [BindProperty]
        public string taxParam1 { get; set; }
        [BindProperty]
        public string taxParam2 { get; set; }
        [BindProperty]
        public string taxParam3 { get; set; }

        [BindProperty]
        public string govInitial { get; set; }
        [BindProperty]
        public string govDist { get; set; }
        [BindProperty]
        public string govParam1 { get; set; }
        [BindProperty]
        public string govParam2 { get; set; }
        [BindProperty]
        public string govParam3 { get; set; }

        [BindProperty]
        public string otherInitial { get; set; }
        [BindProperty]
        public string otherDist { get; set; }
        [BindProperty]
        public string otherParam1 { get; set; }
        [BindProperty]
        public string otherParam2 { get; set; }
        [BindProperty]
        public string otherParam3 { get; set; }
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
            taxParams.Add(taxInitial);
            taxParams.Add(taxDist);
            taxParams.Add(taxParam1);
            taxParams.Add(taxParam2);
            taxParams.Add(taxParam3);

            govParams.Add(govInitial);
            govParams.Add(govDist);
            govParams.Add(govParam1);
            govParams.Add(govParam2);
            govParams.Add(govParam3);

            otherParams.Add(otherInitial);
            otherParams.Add(otherDist);
            otherParams.Add(otherParam1);
            otherParams.Add(otherParam2);
            otherParams.Add(otherParam3);

            double[] results = RevenueSimple(iterations, years, constant, taxParams, govParams, otherParams);
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
        //parameters follow: (current value, distribution name, statistical parameters)
        public double[] ExpenseComplex(int iterations, int years, bool constant, List<String> administrationParameters,
            List<String> judicialParameters, List<String> safetyParameters, List<String> worksParameters, List<String> healthParameters,
            List<String> educationParameters, List<String> parksParameters, List<String> communityParameters,
            List<String> nondepartmentalParameters, List<String> capitalParameters, List<String> principalParameters,
            List<String> interestParameters)
        {
            double[] revenues = new double[iterations];
            //assigning distribution
            DataClasses.Distribution dist1 = AssignDistribution(administrationParameters);
            DataClasses.Distribution dist2 = AssignDistribution(judicialParameters);
            DataClasses.Distribution dist3 = AssignDistribution(safetyParameters);
            DataClasses.Distribution dist4 = AssignDistribution(worksParameters);
            DataClasses.Distribution dist5 = AssignDistribution(healthParameters);
            DataClasses.Distribution dist6 = AssignDistribution(educationParameters);
            DataClasses.Distribution dist7 = AssignDistribution(parksParameters);
            DataClasses.Distribution dist8 = AssignDistribution(communityParameters);
            DataClasses.Distribution dist9 = AssignDistribution(nondepartmentalParameters);
            DataClasses.Distribution dist10 = AssignDistribution(capitalParameters);
            DataClasses.Distribution dist11 = AssignDistribution(principalParameters);
            DataClasses.Distribution dist12 = AssignDistribution(interestParameters);


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

            if (constant == true)
            {
                for (int i = 0; i < iterations; i++)
                {
                    admin = Convert.ToDouble(administrationParameters[0]) * Math.Pow(1 + dist1.GenerateRandom(), years);
                    judicial = Convert.ToDouble(judicialParameters[0]) * Math.Pow(1 + dist2.GenerateRandom(), years);
                    safety = Convert.ToDouble(safetyParameters[0]) * Math.Pow(1 + dist3.GenerateRandom(), years);
                    work = Convert.ToDouble(worksParameters[0]) * Math.Pow(1 + dist4.GenerateRandom(), years);
                    health = Convert.ToDouble(healthParameters[0]) * Math.Pow(1 + dist5.GenerateRandom(), years);
                    education = Convert.ToDouble(educationParameters[0]) * Math.Pow(1 + dist6.GenerateRandom(), years);
                    parks = Convert.ToDouble(parksParameters[0]) * Math.Pow(1 + dist7.GenerateRandom(), years);
                    community = Convert.ToDouble(communityParameters[0]) * Math.Pow(1 + dist8.GenerateRandom(), years);
                    nondepartmental = Convert.ToDouble(nondepartmentalParameters[0]) * Math.Pow(1 + dist9.GenerateRandom(), years);
                    capital = Convert.ToDouble(capitalParameters[0]) * Math.Pow(1 + dist10.GenerateRandom(), years);
                    principal = Convert.ToDouble(principalParameters[0]) * Math.Pow(1 + dist11.GenerateRandom(), years);
                    interest = Convert.ToDouble(interestParameters[0]) * Math.Pow(1 + dist12.GenerateRandom(), years);

                    revenues[i] = admin + judicial + safety + work + health + education + parks + community + nondepartmental + capital +
                        principal + interest;
                }
            }
            else
            {
                for (int i = 0; i < iterations; i++)
                {
                    admin = Convert.ToDouble(administrationParameters[0]);
                    judicial = Convert.ToDouble(judicialParameters[0]);
                    safety = Convert.ToDouble(safetyParameters[0]);
                    work = Convert.ToDouble(worksParameters[0]);
                    health = Convert.ToDouble(healthParameters[0]);
                    education = Convert.ToDouble(educationParameters[0]);
                    parks = Convert.ToDouble(parksParameters[0]);
                    community = Convert.ToDouble(communityParameters[0]);
                    nondepartmental = Convert.ToDouble(nondepartmentalParameters[0]);
                    capital = Convert.ToDouble(capitalParameters[0]);
                    principal = Convert.ToDouble(principalParameters[0]);
                    interest = Convert.ToDouble(interestParameters[0]);


                    for (int j = 0; j < years; j++)
                    {
                        admin *= 1 + dist1.GenerateRandom();
                        judicial *= 1 + dist2.GenerateRandom();
                        safety *= 1 + dist3.GenerateRandom();
                        work *= 1 + dist4.GenerateRandom();
                        health *= 1 + dist5.GenerateRandom();
                        education *= 1 + dist6.GenerateRandom();
                        parks *= 1 + dist7.GenerateRandom();
                        community *= 1 + dist8.GenerateRandom();
                        nondepartmental *= 1 + dist9.GenerateRandom();
                        capital *= 1 + dist10.GenerateRandom();
                        principal *= 1 + dist11.GenerateRandom();
                        interest *= 1 + dist12.GenerateRandom();
                    }
                    revenues[i] = admin + judicial + safety + work + health + education + parks + community + nondepartmental + capital +
                                            principal + interest;
                }
            }
            return revenues;
        }
        public double[] RevenueComplex(int iterations, int years, bool constant, List<String> serviceParameters, List<String> permitParameters, List<String> propertyParameters,
            List<String> localParameters, List<String> miscellaneousParameters, List<String> fineParameters, List<String> useOfMoneyParameters, List<String> commonwealthParameters, List<String> federalParameters)
        {
            double[] revenues = new double[iterations];
            //assigning distribution
            DataClasses.Distribution dist1 = AssignDistribution(serviceParameters);
            DataClasses.Distribution dist2 = AssignDistribution(permitParameters);
            DataClasses.Distribution dist3 = AssignDistribution(propertyParameters);
            DataClasses.Distribution dist4 = AssignDistribution(localParameters);
            DataClasses.Distribution dist5 = AssignDistribution(miscellaneousParameters);
            DataClasses.Distribution dist6 = AssignDistribution(fineParameters);
            DataClasses.Distribution dist7 = AssignDistribution(useOfMoneyParameters);
            DataClasses.Distribution dist8 = AssignDistribution(commonwealthParameters);
            DataClasses.Distribution dist9 = AssignDistribution(federalParameters);

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

            if (constant == true)
            {
                for (int i = 0; i < iterations; i++)
                {
                    service = Convert.ToDouble(serviceParameters[0]) * Math.Pow(1 + dist1.GenerateRandom(), years);
                    permit = Convert.ToDouble(permitParameters[0]) * Math.Pow(1 + dist2.GenerateRandom(), years);
                    property = Convert.ToDouble(propertyParameters[0]) * Math.Pow(1 + dist3.GenerateRandom(), years);
                    local = Convert.ToDouble(localParameters[0]) * Math.Pow(1 + dist4.GenerateRandom(), years);
                    misc = Convert.ToDouble(miscellaneousParameters[0]) * Math.Pow(1 + dist5.GenerateRandom(), years);
                    fine = Convert.ToDouble(fineParameters[0]) * Math.Pow(1 + dist6.GenerateRandom(), years);
                    money = Convert.ToDouble(useOfMoneyParameters[0]) * Math.Pow(1 + dist7.GenerateRandom(), years);
                    commonwealth = Convert.ToDouble(commonwealthParameters[0]) * Math.Pow(1 + dist8.GenerateRandom(), years);
                    federal = Convert.ToDouble(federalParameters[0]) * Math.Pow(1 + dist9.GenerateRandom(), years);

                    revenues[i] = service + permit + local + misc + fine + money + commonwealth + federal;
                }
            }
            else
            {
                for (int i = 0; i < iterations; i++)
                {
                    service = Convert.ToDouble(serviceParameters[0]);
                    permit = Convert.ToDouble(permitParameters[0]);
                    property = Convert.ToDouble(propertyParameters[0]);
                    local = Convert.ToDouble(localParameters[0]);
                    misc = Convert.ToDouble(miscellaneousParameters[0]);
                    fine = Convert.ToDouble(fineParameters[0]);
                    money = Convert.ToDouble(useOfMoneyParameters[0]);
                    commonwealth = Convert.ToDouble(commonwealthParameters[0]);
                    federal = Convert.ToDouble(federalParameters[0]);

                    for (int j = 0; j < years; j++)
                    {
                        service *= 1 + dist1.GenerateRandom();
                        permit *= 1 + dist2.GenerateRandom();
                        property *= 1 + dist3.GenerateRandom();
                        local *= 1 + dist4.GenerateRandom();
                        misc *= 1 + dist5.GenerateRandom();
                        fine *= 1 + dist6.GenerateRandom();
                        money *= 1 + dist7.GenerateRandom();
                        commonwealth *= 1 + dist8.GenerateRandom();
                        federal *= 1 + dist9.GenerateRandom();

                    }
                    revenues[i] = service + permit + local + misc + fine + money + commonwealth + federal;
                }
            }
            return revenues;
        }
        public double[] RevenueSimple(int iterations, int years, bool constant, List<String> taxParameters, 
            List<String> intergovernmentalParameters, List<String> otherParameters)
        {
            double[] revenues = new double[iterations];

            //assigning distribution
            DataClasses.Distribution dist1 = AssignDistribution(taxParameters);

            DataClasses.Distribution dist2 = AssignDistribution(intergovernmentalParameters);

            DataClasses.Distribution dist3 = AssignDistribution(otherParameters);

            //conducting simulation
            double tax;
            double gov;
            double other;
            if (constant == true)
            {
                for (int i = 0; i < iterations; i++)
                {
                    tax = Convert.ToDouble(taxParameters[0]) * Math.Pow(1 + dist1.GenerateRandom(), years);
                    gov = Convert.ToDouble(intergovernmentalParameters[0]) * Math.Pow(1 + dist1.GenerateRandom(), years);
                    other = Convert.ToDouble(otherParameters[0]) * Math.Pow(1 + dist1.GenerateRandom(), years);
                    revenues[i] = tax + gov + other;
                }
            }
            else
            {
                for (int i = 0; i < iterations; i++)
                {
                    tax = Convert.ToDouble(taxParameters[0]);
                    gov = Convert.ToDouble(intergovernmentalParameters[0]);
                    other = Convert.ToDouble(otherParameters[0]);
                    for (int j = 0; j < years; j++)
                    {
                        tax *= 1 + dist1.GenerateRandom();
                        gov *= 1 + dist2.GenerateRandom();
                        other *= 1 + dist3.GenerateRandom();
                    }
                    revenues[i] = tax + gov + other;
                }
            }
            return revenues;
        }
        public DataClasses.Distribution AssignDistribution(List<String> distribution)
        {
            if (distribution[0].Equals("Uniform"))
            {
                if (Convert.ToDouble(distribution[2]) < Convert.ToDouble(distribution[3]))
                {
                    return new DataClasses.Uniform(new Random(), Convert.ToDouble(distribution[2]), Convert.ToDouble(distribution[3]));
                }
                else
                {
                    throw new InvalidOperationException("Min must be below Max");
                }
            }
            else if (distribution[1].Equals("Triangular"))
            {
                if (Convert.ToDouble(distribution[2]) <= Convert.ToDouble(distribution[3]) &
                    Convert.ToDouble(distribution[2]) < Convert.ToDouble(distribution[4]))
                {
                    if (Convert.ToDouble(distribution[4]) <= Convert.ToDouble(distribution[3]))
                    {
                        return new DataClasses.Triangular(new Random(), Convert.ToDouble(distribution[2]), Convert.ToDouble(distribution[4]),
                            Convert.ToDouble(distribution[3]));
                    }
                    else
                    {
                        throw new InvalidOperationException("Likely must be below the max");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Min must be below max and Likely");
                }

            }
            else if (distribution[1].Equals("Normal"))
            {
                return new DataClasses.Normal(new Random(), Convert.ToDouble(distribution[2]), Convert.ToDouble(distribution[3]));
            }
            else if (distribution[1].Equals("Lognormal"))
            {
                return new DataClasses.Normal(new Random(), Convert.ToDouble(distribution[2]), Convert.ToDouble(distribution[3]));
            }
            else
            {
                return new DataClasses.Constant(new Random(), Convert.ToDouble(distribution[2]));
            }
        }
    }
}
