using Kopilka.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.DataAccess
{
    /// <summary>
    /// Контекст базы данных для приложения "Копилка".
    /// </summary>
    public class KopilkaDbContext : DbContext
    {
        /// <summary>
        /// Набор данных для таблицы "Пользователи".
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Набор данных для таблицы "Счета".
        /// </summary>
        public DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// Набор данных для таблицы "Категории".
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Набор данных для таблицы "Транзакции".
        /// </summary>
        public DbSet<Transaction> Transactions { get; set; }

        public KopilkaDbContext()
        {
        }

        public KopilkaDbContext(DbContextOptions<KopilkaDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=Kopilka.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
