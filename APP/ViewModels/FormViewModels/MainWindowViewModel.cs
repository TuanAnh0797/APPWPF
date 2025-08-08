using APP.UserControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace APP.ViewModels.FormViewModels
{
    public partial class MainWindowViewModel: ObservableObject
    {
        [ObservableProperty]
        private UserControl currentView;
        [ObservableProperty]
        private string selectedpage = "Home";

        public MainWindowViewModel()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCHome>();
        }
        [RelayCommand]
        private void ShowHome()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCHome>();
            Selectedpage = "Home";
        }
        [RelayCommand]
        private void ShowTools()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCTools>();
            Selectedpage = "Tools";
        }
        [RelayCommand]
        private void ShowSetting()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCSetting>();
            Selectedpage = "Setting";
        }
        [RelayCommand]
        private void ShowHelp()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCHelp>();
            Selectedpage = "Help";
        }



    }
}
