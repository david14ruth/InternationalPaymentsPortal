using System.Collections.Generic;

namespace PaymentSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalPayments { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<User> Users { get; set; }
        // ✅ ADD THIS
        public UserGrowthViewModel UserGrowth { get; set; }
    }
}