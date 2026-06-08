using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Models;
using PaymentSystem.Services;

namespace PaymentSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AuthController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // =========================================================
        // ================= REGISTER ===============================
        // =========================================================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            model.Email = model.Email?.Trim().ToLower();
            model.FullName = model.FullName?.Trim();

            if (string.IsNullOrWhiteSpace(model.FullName) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.Error = "All fields are required";
                return View(model);
            }

            if (_context.Users.Any(x => x.Email == model.Email))
            {
                ViewBag.Error = "Email already exists";
                return View(model);
            }

            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("RegOtp", otp);
            HttpContext.Session.SetString("RegEmail", model.Email);
            HttpContext.Session.SetString("RegName", model.FullName);
            HttpContext.Session.SetString("RegPassword", model.Password);

            await _emailService.SendOtp(model.Email, otp);

            return RedirectToAction("VerifyRegisterOtp");
        }

        // =========================================================
        // ================= VERIFY REGISTER OTP ====================
        // =========================================================
        [HttpGet]
        public IActionResult VerifyRegisterOtp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyRegisterOtp(string otp)
        {
            var sessionOtp = HttpContext.Session.GetString("RegOtp");
            var email = HttpContext.Session.GetString("RegEmail");
            var name = HttpContext.Session.GetString("RegName");
            var password = HttpContext.Session.GetString("RegPassword");

            if (string.IsNullOrEmpty(sessionOtp) || string.IsNullOrEmpty(email))
                return RedirectToAction("Register");

            if (otp == sessionOtp)
            {
                var user = new User
                {
                    Name = name,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = "User"
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                HttpContext.Session.Clear();

                return RedirectToAction("Login");
            }

            ViewBag.Error = "Invalid OTP";
            return View();
        }

        // =========================================================
        // ================= LOGIN ==================================
        // =========================================================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            username = username?.Trim().ToLower();

            var user = _context.Users.FirstOrDefault(x => x.Email == username);

            if (user == null)
            {
                ViewBag.Error = "Invalid login details";
                return View();
            }

            bool isValidPassword;

            try
            {
                isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            catch
            {
                isValidPassword = (password == user.Password);
            }

            if (!isValidPassword)
            {
                ViewBag.Error = "Invalid login details";
                return View();
            }

            // =====================================================
            // 👑 SUPER ADMIN → NO OTP (DIRECT LOGIN)
            // =====================================================
            if (user.Role == "SuperAdmin")
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("User", user.Name ?? "Admin");
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("Email", user.Email ?? "");

                return RedirectToAction("Index", "Admin");
            }

            // =====================================================
            // 👨‍💼 ADMIN + 👤 USER → OTP LOGIN
            // =====================================================
            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("LoginOtp", otp);
            HttpContext.Session.SetString("LoginEmail", user.Email);
            HttpContext.Session.SetString("LoginRole", user.Role);

            await _emailService.SendOtp(user.Email, otp);

            return RedirectToAction("VerifyOtp");
        }

        // =========================================================
        // ================= VERIFY LOGIN OTP =======================
        // =========================================================
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOtp(string otp)
        {
            var sessionOtp = HttpContext.Session.GetString("LoginOtp");
            var email = HttpContext.Session.GetString("LoginEmail");

            if (string.IsNullOrEmpty(sessionOtp) || string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            if (user == null)
                return RedirectToAction("Login");

            if (otp == sessionOtp)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("User", user.Name ?? "");
                HttpContext.Session.SetString("Role", user.Role ?? "");
                HttpContext.Session.SetString("Email", user.Email ?? "");

                HttpContext.Session.Remove("LoginOtp");
                HttpContext.Session.Remove("LoginEmail");
                HttpContext.Session.Remove("LoginRole");

                // =====================================================
                // ROLE ROUTING FIX
                // =====================================================
                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Index", "UserDashboard");
            }

            ViewBag.Error = "Invalid OTP";
            return View();
        }

        // =========================================================
        // ================= RESEND OTP =============================
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOtp()
        {
            var email = HttpContext.Session.GetString("LoginEmail");

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("LoginOtp", otp);

            await _emailService.SendOtp(email, otp);

            ViewBag.Message = "OTP resent successfully";

            return View("VerifyOtp");
        }

        // =========================================================
        // ================= LOGOUT =================================
        // =========================================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}