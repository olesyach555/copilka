using Kopilka.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace Kopilka.FinanceManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await using var context = new KopilkaDbContext();
            await context.Database.MigrateAsync();
        }
    }
}
