using Microsoft.EntityFrameworkCore;

namespace PaymentSystem.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ================= TABLES =================
        public DbSet<User> Users { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserUpdateRequest> UserUpdateRequests { get; set; }

        // ================= RELATIONSHIP ONLY (NO SCHEMA CONTROL) =================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔥 KEEP ONLY RELATIONSHIP (SAFE FOR EXISTING SQL DB)

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}