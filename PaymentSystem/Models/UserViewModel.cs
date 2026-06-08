namespace PaymentSystem.Models
{
    public class UserDashboardViewModel
    {
        public User User { get; set; }
        public List<Payment> Payments { get; set; }
    }
}