using APP.UserControls;
using APP.ViewModels;
using APP.ViewModels.FormViewModels;
using APP.ViewModels.UserControlViewModels.Home;
using Microsoft.Extensions.DependencyInjection;
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
            var services = new ServiceCollection();
            // Đăng ký service và viewmodel
            services.AddSingleton<UCControlBarViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<UCHomeViewModel>();
            services.AddSingleton<UCHome>();
            services.AddSingleton<UCSetting>();
            services.AddSingleton<UCTools>();
            services.AddSingleton<UCHelp>();

            ServiceProvider = services.BuildServiceProvider();
            base.OnStartup(e);
        }
         
    }
}
