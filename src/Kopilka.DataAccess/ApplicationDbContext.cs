using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.DataAccess
{
    /// <summary>
    /// Контекст базы данных для приложения "Копилка".
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Family> Families { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<DebtContract> DebtContracts { get; set; }
        public DbSet<PaymentSchedule> PaymentSchedules { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        public DbSet<FinancialGoal> FinancialGoals { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Date> Dates { get; set; }

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
            // Настройка decimal для SQLite
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));
                foreach (var property in properties)
                {
                    property.SetColumnType("TEXT");
                }
            }

            // Связи и каскадное удаление
            modelBuilder.Entity<User>()
                .HasOne(u => u.Family)
                .WithMany(f => f.Users)
                .HasForeignKey(u => u.FamilyId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<DebtContract>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<PaymentSchedule>()
                .HasOne(p => p.DebtContract)
                .WithMany()
                .HasForeignKey(p => p.DebtContractId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentHistory>()
                .HasOne(ph => ph.DebtContract)
                .WithMany()
                .HasForeignKey(ph => ph.DebtContractId);

            modelBuilder.Entity<FinancialGoal>()
                .HasOne(g => g.OwnerUser)
                .WithMany()
                .HasForeignKey(g => g.OwnerUserId);

            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<UserSettings>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId);
        }
    }
}
