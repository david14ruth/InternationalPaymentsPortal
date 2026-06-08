using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentSystem.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string RecipientName { get; set; }

        [Required]
        [RegularExpression(@"^\d{10,16}$")]
        public string AccountNumber { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{6}[A-Z0-9]{2,5}$")]
        public string SwiftCode { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; }

        // ✅ ENUM STATUS (IMPORTANT)
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public User? User { get; set; }
    }

    // ================= ENUM =================
    public enum PaymentStatus
    {
        Pending = 0,
        Verified = 1,
        SentToSWIFT = 2,
        Completed = 3,
        Rejected = 4
    }
}