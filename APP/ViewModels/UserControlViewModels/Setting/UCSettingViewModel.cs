﻿using APP.UserControls;
using APP.UserControls.Setting.Sub;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace APP.ViewModels.UserControlViewModels.Setting;

public partial class UCSettingViewModel : ObservableObject
{
    [ObservableProperty]
    private UserControl currentView;
    public UCSettingViewModel()
    {
        CurrentView = App.ServiceProvider.GetRequiredService<UCPLCSetting>();
    }
    [RelayCommand]
    private void ShowPLCSetting()
    {
        CurrentView = App.ServiceProvider.GetRequiredService<UCPLCSetting>();
    }
    [RelayCommand]
    private void ShowPrinterSetting()
    {
        CurrentView = App.ServiceProvider.GetRequiredService<UCPrinterSetting>();
    }
    [RelayCommand]
    private void ShowMasterSetting()
    {
        CurrentView = App.ServiceProvider.GetRequiredService<UCMasterSetting>();
    }
}
