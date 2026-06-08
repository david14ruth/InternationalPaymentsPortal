using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Models;

namespace PaymentSystem.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        // ================= CREATE PAYMENT =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Payment model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            model.UserId = userId.Value;

            // ✅ ENUM FIX
            model.Status = PaymentStatus.Pending;

            model.DateCreated = DateTime.Now;

            _context.Payments.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Payment submitted successfully";

            return RedirectToAction("Success", new { id = model.Id });
        }

        // ================= SUCCESS =================
        public IActionResult Success(int id)
        {
            var payment = _context.Payments
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);

            if (payment == null)
                return NotFound();

            return View(payment);
        }
    }
}