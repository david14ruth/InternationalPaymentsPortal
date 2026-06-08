using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Models;

namespace PaymentSystem.Controllers
{
    public class AdminPaymentsController : Controller
    {
        private readonly AppDbContext _context;

        public AdminPaymentsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            var user = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(user))
                return RedirectToAction("Login", "Auth");

            if (role != "Admin" && role != "SuperAdmin")
                return Forbid();

            var payments = _context.Payments
                .Include(p => p.User)
                .OrderByDescending(p => p.DateCreated)
                .ToList();

            return View(payments);
        }

        [HttpPost]
        public IActionResult Verify(int id)
        {
            var p = _context.Payments.Find(id);
            if (p == null) return NotFound();

            if (p.Status != PaymentStatus.Pending)
                return RedirectToAction("Index");

            p.Status = PaymentStatus.Verified;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SendToSwift(int id)
        {
            var p = _context.Payments.Find(id);
            if (p == null) return NotFound();

            if (p.Status != PaymentStatus.Verified)
                return RedirectToAction("Index");

            p.Status = PaymentStatus.SentToSWIFT;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Complete(int id)
        {
            var p = _context.Payments.Find(id);
            if (p == null) return NotFound();

            if (p.Status != PaymentStatus.SentToSWIFT)
                return RedirectToAction("Index");

            p.Status = PaymentStatus.Completed;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var p = _context.Payments.Find(id);
            if (p == null) return NotFound();

            p.Status = PaymentStatus.Rejected;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}