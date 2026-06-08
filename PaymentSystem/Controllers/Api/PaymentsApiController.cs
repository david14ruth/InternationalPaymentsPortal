using Microsoft.AspNetCore.Mvc;
using PaymentSystem.Models;

namespace PaymentSystem.Controllers.Api
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentsApiController(AppDbContext context)
        {
            _context = context;
        }

        // ================= CREATE PAYMENT =================
        [HttpPost("create")]
        public IActionResult Create([FromBody] Payment model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.Status = PaymentStatus.Pending;
            model.DateCreated = DateTime.Now;

            _context.Payments.Add(model);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Payment created successfully",
                paymentId = model.Id,
                status = model.Status.ToString()
            });
        }

        // ================= GET ALL USER PAYMENTS =================
        [HttpGet("user/{userId}")]
        public IActionResult GetUserPayments(int userId)
        {
            var payments = _context.Payments
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.DateCreated)
                .ToList();

            return Ok(payments);
        }

        // ================= GET SINGLE PAYMENT =================
        [HttpGet("{id}")]
        public IActionResult GetPayment(int id)
        {
            var payment = _context.Payments.FirstOrDefault(p => p.Id == id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }
    }
}