using System;

namespace PaymentSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public string? Role { get; set; }
        public string? Status { get; set; }

        public bool IsApproved { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? PendingName { get; set; }
        public string? PendingEmail { get; set; }
    }
}