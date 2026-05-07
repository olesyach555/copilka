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

            // Глобальное отключение каскадного удаления для предотвращения циклов в SQL Server
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Исключения, где каскад оправдан и не создает циклов
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
        }
    }
}
