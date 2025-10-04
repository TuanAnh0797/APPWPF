using APP.Database;
using APP.Interface.language;
using APP.Service;
using APP.UserControls;
using APP.ViewModels;
using APP.ViewModels.FormViewModels;
using APP.ViewModels.UserControlViewModels.Home;
using APP.ViewModels.UserControlViewModels.Tools;
using LiveChartsCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SQLitePCL;
using System.Configuration;
using System.Data;
using System.Windows;

namespace APP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {

            Batteries.Init();

            var services = new ServiceCollection();
            //Authorization
            services.AddDbContext<AppDbContext>(option => {
                option.UseSqlite("Data Source=D:\\Project\\Database\\VFDB.db");
            });
            services.AddSingleton<UserSession>();
            services.AddSingleton<AuthorizationService>();

            // Đăng ký service và viewmodel
            services.AddSingleton<UCControlBarViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<LoginWindowViewModel>();
            services.AddSingleton<UCHomeViewModel>();
            services.AddSingleton<UCToolsViewModel>();
            services.AddSingleton<UCHome>();
            services.AddSingleton<UCSetting>();
            services.AddSingleton<UCTools>();
            services.AddSingleton<UCHelp>();
            // Servive language
            services.AddSingleton<ILocalizationService,LocalizationService>();
           



            ServiceProvider = services.BuildServiceProvider();
            base.OnStartup(e);
        }
         
    }
}
