using APP.Interface.language;
using APP.Service;
using APP.UserControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace APP.ViewModels.FormViewModels
{
    public partial class MainWindowViewModel: ObservableObject
    {
        [ObservableProperty]
        private UserControl currentView;
        [ObservableProperty]
        private string selectedpage = "Home";

        private ILocalizationService _localizationService;

        public MainWindowViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            CurrentView = App.ServiceProvider.GetRequiredService<UCHome>();
        }
        [RelayCommand]
        private void ShowHome()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCHome>();
            Selectedpage = _localizationService.GetString("Home");
        }
        [RelayCommand]
        private void ShowTools()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCTools>();
            Selectedpage = _localizationService.GetString("Tools");
        }
        [RelayCommand]
        private void ShowSetting()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCSetting>();
            Selectedpage = _localizationService.GetString("Setting");
        }
        [RelayCommand]
        private void ShowHelp()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCHelp>();
            Selectedpage = _localizationService.GetString("Help");
        }
        [RelayCommand]
        private void ChangeLanguage(string language = "vi")
        {
            language = "vi";
            string dictPath = $"Resources/Language/StringResources.{language}.xaml";
            var dicts = Application.Current.Resources.MergedDictionaries;
            var oldDict = dicts.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("StringResources."));
            if (oldDict != null)
            {
                int index = dicts.IndexOf(oldDict);
                dicts[index] = new ResourceDictionary() { Source = new Uri(dictPath, UriKind.Relative) };
                _localizationService.ChangeLanguage(language);
            }
            else
            {
                dicts.Add(new ResourceDictionary() { Source = new Uri(dictPath, UriKind.Relative) });
                _localizationService.ChangeLanguage(language);
            }
        }



    }
}
