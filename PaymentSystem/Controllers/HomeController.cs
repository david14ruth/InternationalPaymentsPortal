using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Models;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaymentSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // ================= HOME =================
        public IActionResult Index()
        {
            return View();
        }

        // ================= SERVICES =================
        public IActionResult Services()
        {
            return View();
        }

        // ================= PRIVACY =================
        public IActionResult Privacy()
        {
            return View();
        }

        // ================= CONTACT =================
        public IActionResult Contact()
        {
            return View();
        }

        // ================= REDIRECT TO LOGIN (IMPORTANT ADDITION) =================
        public IActionResult Login()
        {
            return RedirectToAction("Login", "Auth");
        }

        // ================= REDIRECT TO DASHBOARD (OPTIONAL BUT USEFUL) =================
        public IActionResult Dashboard()
        {
            return RedirectToAction("Index", "UserDashboard");
        }

        // ================= ERROR =================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}