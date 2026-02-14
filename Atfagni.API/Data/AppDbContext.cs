using Microsoft.EntityFrameworkCore;
using Atfagni.API.Entities;

namespace Atfagni.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<Booking> Bookings { get; set; } // Ajouté
        public DbSet<CommissionRule> CommissionRules { get; set; } // Ajouté
        public DbSet<WalletTransaction> WalletTransactions { get; set; } // Ajouté

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Exemple de règle : Un utilisateur a plusieurs transactions
            modelBuilder.Entity<User>()
                .HasMany<WalletTransaction>()
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}