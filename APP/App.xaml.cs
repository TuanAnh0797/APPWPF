using APP.Database;
using APP.Interface.language;
using APP.Service;
using APP.UserControls;
using APP.UserControls.Report;
using APP.UserControls.Setting.Sub;
using APP.ViewModels;
using APP.ViewModels.FormViewModels;
using APP.ViewModels.UserControlViewModels.Home;
using APP.ViewModels.UserControlViewModels.Report;
using APP.ViewModels.UserControlViewModels.Setting;
using APP.ViewModels.UserControlViewModels.Setting.Sub;
using APP.ViewModels.UserControlViewModels.Tools;
using APP.Views;
using LiveChartsCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SQLitePCL;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
        private static Mutex mutex;
        protected override  async void OnStartup(StartupEventArgs e)
        {
            const string appName = "APP"; 
            bool createdNew;
            mutex = new Mutex(true, appName, out createdNew);
            if (!createdNew)
            {
                ActivateExistingWindow();
                Shutdown();
                return;
            }

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
            // service & viewmodel
            services.AddSingleton<UCMasterSettingViewModel>();
            services.AddSingleton<UCControlBarViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<LoginWindowViewModel>();
            services.AddSingleton<UCHomeViewModel>();
            services.AddSingleton<UCSettingViewModel>();
            services.AddSingleton<UCPLCSettingViewModel>();
            services.AddSingleton<UCPrinterSettingViewModel>();
            services.AddSingleton<UCPersonSettingViewModel>();
            services.AddTransient<UCReportViewModel>();
            services.AddSingleton<UCHome>();
            services.AddSingleton<UCSetting>();
            services.AddSingleton<UCMasterSetting>();
            services.AddSingleton<UCMaterialSettingViewModel>();
            services.AddSingleton<UCMaterialSetting>();
            services.AddSingleton<UCPLCSetting>();
            services.AddSingleton<UCPrinterSetting>();
            services.AddSingleton<UCPersonSetting>();
            services.AddSingleton<PLCService>();
            services.AddSingleton<UCToolsViewModel>();
            services.AddSingleton<UCTools>();
            services.AddSingleton<UCHelp>();
            services.AddTransient<UCReport>();
            //Printer
            services.AddSingleton<PrinterService>();
            // Servive language
            services.AddSingleton<ILocalizationService,LocalizationService>();
            //ManagementUser
            services.AddSingleton<UserManagementViewModel>();
            services.AddSingleton<UserManagement>();
            //Init
            ServiceProvider = services.BuildServiceProvider();
            //Remember
            var db = ServiceProvider.GetRequiredService<AppDbContext>();
            var userremember = db.RememberUser.FirstOrDefault();
            if (userremember != null)
            {
                var au = ServiceProvider.GetRequiredService<AuthorizationService>();
                await au.LoginAsync(userremember.UserID, userremember.PassWord, true);
            }
            //Start Monitor
            var PLC = App.ServiceProvider.GetRequiredService<PLCService>();
            PLC.Start();

            base.OnStartup(e);
        }
        private void ActivateExistingWindow()
        {
            try
            {
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        // đưa cửa sổ đang mở ra trước màn hình
                        NativeMethods.ShowWindow(process.MainWindowHandle, NativeMethods.SW_RESTORE);
                        NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }
            }
            catch { }
        }
        internal class NativeMethods
        {
            public const int SW_RESTORE = 9;

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        }

    }
}
