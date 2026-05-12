using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.DataAccess
{
    /// <summary>
    /// Контекст базы данных для приложения "Копилка" (SQLite).
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Family> Families { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<DebtContract> DebtContracts { get; set; } = null!;
        public DbSet<PaymentSchedule> PaymentSchedules { get; set; } = null!;
        public DbSet<PaymentHistory> PaymentHistories { get; set; } = null!;
        public DbSet<FinancialGoal> FinancialGoals { get; set; } = null!;
        public DbSet<Reminder> Reminders { get; set; } = null!;
        public DbSet<UserSettings> UserSettings { get; set; } = null!;

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=kopilka.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка точности для decimal (в SQLite они мапятся в TEXT или REAL)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));
                foreach (var property in properties)
                {
                    property.SetPrecision(18);
                    property.SetScale(2);
                }
            }

            // Глобальное отключение каскадного удаления (для SQLite это менее критично, чем для SQL Server, но полезно для консистентности)
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Настройка связей
            modelBuilder.Entity<User>()
                .HasOne(u => u.Family)
                .WithMany(f => f.Users)
                .HasForeignKey(u => u.FamilyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PaymentSchedule>()
                .HasOne(p => p.DebtContract)
                .WithMany()
                .HasForeignKey(p => p.DebtContractId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
