using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddDbContext<KopilkaDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IConfiguration>(configuration);
            services.AddTransient<AuthService>();
            services.AddTransient<LoginWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<KopilkaDbContext>();
                // Применяем все ожидающие миграции при запуске
                dbContext.Database.Migrate();
            }

            var loginWindow = _serviceProvider.GetService<LoginWindow>();
            loginWindow.Show();
        }
    }
}
