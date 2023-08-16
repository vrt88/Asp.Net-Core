using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using SitoVetrina.Areas.Identity.Pages.Prodotto;
using SitoVetrina.Models;
using SitoVetrina.Repository;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Cryptography.X509Certificates;
using static SitoVetrina.Areas.Identity.Pages.Prodotto.CreaProdottoModel;

namespace SitoVetrina.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment,IConfiguration configuration)
        {
            _logger = logger;
            hostingEnvironment = environment;
            _configuration=configuration;
        }
        public IActionResult CreaProdotto()
        {
            return View(new CreaProdottoModel(hostingEnvironment));
        }
        public IActionResult DettagliProdotto()
        {
            return View(new DettagliProdottoModel(hostingEnvironment));
        }

        public IActionResult Index()
        {
            Context.DapperContext context = new Context.DapperContext(_configuration);
            string url = HttpContext.Request.GetDisplayUrl();
            return View(new IndexRepository(context, url));
        }
        //public IActionResult Index()
        //{
        //    string url = HttpContext.Request.GetDisplayUrl();
        //    return View(new IndexModel(url));
        //}
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}