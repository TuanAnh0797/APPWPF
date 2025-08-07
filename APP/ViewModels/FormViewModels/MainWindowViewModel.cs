using APP.UserControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            CurrentView = new UCHome();
        }
        [RelayCommand]
        private void ShowHome()
        {
            CurrentView = new UCHome();
            Selectedpage = "Home";
        }
        [RelayCommand]
        private void ShowTools()
        {
            CurrentView = new UCTools();
            Selectedpage = "Tools";
        }
        [RelayCommand]
        private void ShowSetting()
        {
            CurrentView = new UCSetting();
            Selectedpage = "Setting";
        }
        [RelayCommand]
        private void ShowHelp()
        {
            CurrentView = new UCHelp();
            Selectedpage = "Help";
        }



    }
}
