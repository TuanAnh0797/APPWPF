using APP.Database;
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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D.Converters;

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
    private readonly AppDbContext _db;


    public bool IsEnable
    {
        get { return ModelPrinter.IsEnable ?? false; }
    }


    public UCPrinterSettingViewModel(PrinterService printerService, AppDbContext db)
    {
        _db = db;
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
    [RelayCommand]
    private void Save()
    {
        try
        {
            var printer = ModelPrinters.Where(p => p.Ischoose == 1).FirstOrDefault();
            printer.IsEnable = ModelPrinter.IsEnable;

            _db.PrinterSetting.Update(printer);
            _db.SaveChanges();
            init();

        }
        catch (Exception ex)
        {

            MessageBox.Show(ex.Message);
        }

    }

}
