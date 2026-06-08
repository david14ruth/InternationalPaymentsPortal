namespace PaymentSystem.Models
{
    public class UserUpdateRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string NewName { get; set; }

        public string NewEmail { get; set; }

        public bool IsApproved { get; set; } = false;
    }

}