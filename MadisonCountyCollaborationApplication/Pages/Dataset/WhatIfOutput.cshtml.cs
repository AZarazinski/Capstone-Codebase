using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Plotly.NET;
using Plotly.NET.CSharp;
using Plotly.NET.LayoutObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.FSharp.Core.ByRefKinds;

namespace MadisonCountyCollaborationApplication.Pages.Dataset
{
    public class WhatIfOutputModel : PageModel
    {
        [BindProperty]
        public double ExpectedOutcome { get; set; }
        public double Intercept { get; set; }
        public List<double> Slopes { get; set; }
        public List<string> IndependentVariables { get; set; } = new List<string>();
        public string DependentVariable { get; set; }
        [BindProperty]
        public double LowerBound { get; set; }
        [BindProperty]
        public double UpperBound { get; set; }
        [BindProperty]
        public List<double> Values { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewData["LoginMessage"] = "Login for " + HttpContext.Session.GetString("username") + " successful!";

                // Retrieve data from session
                var lowerJson = HttpContext.Session.GetString("LowerBound");
                var upperJson = HttpContext.Session.GetString("UpperBound");

                LowerBound = double.Parse(lowerJson);
                UpperBound = double.Parse(upperJson);
                ExpectedOutcome = double.Parse(HttpContext.Session.GetString("ExpectedOutcome"));

                Values = new List<double> { LowerBound, ExpectedOutcome, UpperBound };


                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("/Login");
            }

        }
    }
}
