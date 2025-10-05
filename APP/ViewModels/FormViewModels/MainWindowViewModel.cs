using APP.Database;
using APP.Interface.language;
using APP.Models.Database;
using APP.Service;
using APP.UserControls;
using APP.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        [ObservableProperty]
        private string pageIcon = "Home";
        [ObservableProperty]
        private string currentUser = "Guest";
        [ObservableProperty]
        private Visibility isLoginVisibility =  Visibility.Visible;
        [ObservableProperty]
        private Visibility isLogoutVisibility = Visibility.Collapsed;
        


        private ILocalizationService _localizationService;
        private UserSession _userSession;
        private AuthorizationService _authorizationService;
        private readonly AppDbContext _db;

        public MainWindowViewModel(ILocalizationService localizationService, UserSession userSession, AuthorizationService authorizationService, AppDbContext db)
        {
            _db = db;
            _localizationService = localizationService;
            CurrentView = App.ServiceProvider.GetRequiredService<UCHome>();
            _authorizationService = authorizationService;
            _userSession = userSession;
            _userSession.PropertyChanged += _userSession_PropertyChanged;
        }

        private void _userSession_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
              ShowToolsCommand.NotifyCanExecuteChanged();
              ShowSettingCommand.NotifyCanExecuteChanged();
        }

        private bool CanTool() => _authorizationService.HasRole( "Admin","Operater");
        private bool CanSetting() => _authorizationService.HasRole("Admin");


        [RelayCommand]
        private void ShowHome()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCHome>();
            Selectedpage = _localizationService.GetString("Home");
            PageIcon = "Home";

        }
        [RelayCommand(CanExecute = nameof(CanTool))]
        private void ShowTools()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCTools>();
            Selectedpage = _localizationService.GetString("Tools");
            PageIcon = "Tools";
        }
        [RelayCommand(CanExecute = nameof(CanSetting))]
        private void ShowSetting()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCSetting>();
            Selectedpage = _localizationService.GetString("Setting");
            PageIcon = "CogOutline";
        }
        [RelayCommand]
        private void ShowHelp()
        {
            CurrentView = App.ServiceProvider.GetRequiredService<UCHelp>();
            Selectedpage = _localizationService.GetString("Help");
            PageIcon = "Help";
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
        [RelayCommand]
        private void Login()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        [RelayCommand]
        private void Logout()
        {
            _authorizationService.Logout();
            CurrentUser = "Guest";
        }

        partial void OnCurrentUserChanged(string value)
        {
           if ( value == "Guest")
           {
                IsLoginVisibility = Visibility.Visible;
                IsLogoutVisibility = Visibility.Collapsed;
            }
           else
           {
                IsLoginVisibility = Visibility.Collapsed;
                IsLogoutVisibility = Visibility.Visible;
            }
        }
    }
}

