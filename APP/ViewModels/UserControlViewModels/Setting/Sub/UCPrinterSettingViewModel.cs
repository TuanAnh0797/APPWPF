using APP.Models.Database;
using APP.Service;
using APP.UserControls.Setting.Sub;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.ViewModels.UserControlViewModels.Setting.Sub;

public partial class UCPrinterSettingViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<PrinterSetting> modelPrinters = new ObservableCollection<PrinterSetting>();
    [ObservableProperty]
    PrinterSetting modelPrinter;
    [ObservableProperty]
    string pathTemp;
    private PrinterService _printerService;

    public UCPrinterSettingViewModel(PrinterService printerService)
    {
        _printerService = printerService;
        init();
    }
    private void init()
    {
       var data =  _printerService.GetAllPrinter();
        foreach (var printer in data)
        {
            ModelPrinters.Add(printer);
        }

        ModelPrinter = ModelPrinters.Where(p => p.Ischoose == 1).FirstOrDefault();
       

    }
    partial void OnModelPrinterChanged(PrinterSetting value)
    {
        PathTemp = value.Path;
    }

    [RelayCommand]
    private void PrintTest()
    {
        _printerService.Print(new Models.Printer.ModelPrint(), true);
    }

}
