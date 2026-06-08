using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PaymentSystem.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly AppDbContext _context;

        public UserDashboardController(AppDbContext context)
        {
            _context = context;
        }

        // ================= DASHBOARD =================
        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Login", "Auth");

            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Auth");
            }

            // ================= SAFE PAYMENT QUERY =================
            List<Payment> payments = new List<Payment>();

            try
            {
                payments = _context.Payments
                    .AsNoTracking()
                    .Where(p => p.UserId == user.Id)
                    .OrderByDescending(p => p.DateCreated)
                    .ToList();
            }
            catch (Exception ex)
            {
                // LOG ERROR (optional)
                Console.WriteLine("Payment load error: " + ex.Message);

                payments = new List<Payment>(); // prevent crash
            }

            var model = new UserDashboardViewModel
            {
                User = user,
                Payments = payments
            };

            return View(model);
        }

        // ================= DOWNLOAD STATEMENT =================
        public IActionResult DownloadStatement()
        {
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Login", "Auth");

            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Auth");
            }

            List<Payment> payments = new List<Payment>();

            try
            {
                payments = _context.Payments
                    .AsNoTracking()
                    .Where(p => p.UserId == user.Id)
                    .OrderByDescending(p => p.DateCreated)
                    .ToList();
            }
            catch
            {
                payments = new List<Payment>();
            }

            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(doc, ms);

                doc.Open();

                // ================= TITLE =================
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);

                var title = new Paragraph(
                    "GLOBALTRUST BANK\nTRANSACTION STATEMENT\n\n",
                    titleFont
                )
                {
                    Alignment = Element.ALIGN_CENTER
                };

                doc.Add(title);

                // ================= USER INFO =================
                doc.Add(new Paragraph(
                    $"Customer: {user.Name ?? ""}\n" +
                    $"Email: {user.Email ?? ""}\n" +
                    $"Generated: {DateTime.Now:dd MMM yyyy HH:mm}\n\n"
                ));

                // ================= TABLE =================
                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;

                string[] headers = { "Recipient", "Bank", "Amount", "Date" };

                foreach (var h in headers)
                    table.AddCell(new Phrase(h));

                foreach (var p in payments)
                {
                    table.AddCell(p.RecipientName ?? "");
                    table.AddCell(p.BankName ?? "");
                    table.AddCell("R " + p.Amount.ToString("0.00"));
                    table.AddCell(p.DateCreated.ToString("dd MMM yyyy"));
                }

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(),
                    "application/pdf",
                    "Transaction_Statement.pdf");
            }
        }
    }
}