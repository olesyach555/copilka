using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.DataAccess
{
    /// <summary>
    /// Контекст базы данных для приложения "Копилка" (SQL Server / SSMS).
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
                // СТРОКА ПОДКЛЮЧЕНИЯ ДЛЯ SQL SERVER
                // Измените её на вашу строку подключения к SSMS
                string connectionString = "Server=(localdb)\\mssqllocaldb;Database=KopilkaDB;Trusted_Connection=True;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка точности для decimal (SQL Server требует явного указания или использует 18,2 по умолчанию)
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
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);

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
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserSettings>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId);
        }
    }
}
