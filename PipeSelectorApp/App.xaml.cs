using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace PipeSelectorApp
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            services.AddDbContext<PipesDbContext>(options =>
                options.UseSqlServer("Data Source=192.168.11.222,1433;Initial Catalog=PipeNomenclature;User ID=PipeDataUser;Password=itpz2025zhargal;TrustServerCertificate=True;"));

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = new MainWindow();
            mainWindow.InitializeDataContext(ServiceProvider.GetService<PipesDbContext>()!);
            mainWindow.Show();
        }
    }
}