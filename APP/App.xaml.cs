using APP.Database;
using APP.Interface.language;
using APP.Service;
using APP.UserControls;
using APP.UserControls.Setting.Sub;
using APP.ViewModels;
using APP.ViewModels.FormViewModels;
using APP.ViewModels.UserControlViewModels.Home;
using APP.ViewModels.UserControlViewModels.Setting;
using APP.ViewModels.UserControlViewModels.Setting.Sub;
using APP.ViewModels.UserControlViewModels.Tools;
using LiveChartsCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SQLitePCL;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace APP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        protected override  async void OnStartup(StartupEventArgs e)
        {

            Batteries.Init();

            var services = new ServiceCollection();

            //Database
            string pathdb = Directory.GetCurrentDirectory() + "\\Resources\\Database\\VFDB.db";
            services.AddDbContext<AppDbContext>(option => {
                option.UseSqlite($"Data Source={pathdb}");
            });

            //Authorization

            services.AddSingleton<UserSession>();
            services.AddSingleton<AuthorizationService>();

            // Đăng ký service và viewmodel
            services.AddSingleton<UCMasterSettingViewModel>();
            services.AddSingleton<UCControlBarViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<LoginWindowViewModel>();
            services.AddSingleton<UCHomeViewModel>();
            services.AddSingleton<UCToolsViewModel>();
            services.AddSingleton<UCSettingViewModel>();
           
            services.AddSingleton<UCPLCSettingViewModel>();
            services.AddSingleton<UCPrinterSettingViewModel>();


            services.AddSingleton<UCHome>();
            services.AddSingleton<UCSetting>();
            services.AddSingleton<UCMasterSetting>();
            services.AddSingleton<UCPLCSetting>();
            services.AddSingleton<UCPrinterSetting>();
            services.AddSingleton<UCTools>();
            services.AddSingleton<UCHelp>();

            //Printer
            services.AddSingleton<PrinterService>();
            

            // Servive language
            services.AddSingleton<ILocalizationService,LocalizationService>();
            ServiceProvider = services.BuildServiceProvider();






            //Remember
            var db = ServiceProvider.GetRequiredService<AppDbContext>();
            var userremember = db.RememberUser.FirstOrDefault();
            if (userremember != null)
            {
                var au = ServiceProvider.GetRequiredService<AuthorizationService>();
                await au.LoginAsync(userremember.UserID, userremember.PassWord, true);
            }


            base.OnStartup(e);
        }
         
    }
}
