using APP.Database;
using APP.Models.Database;
using APP.Models.Printer;
using APP.Service;
using APP.ViewModels.UserControlViewModels.Home;
using APP.ViewModels.UserControlViewModels.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace APP.ViewModels.UserControlViewModels.Report;

public partial class UCReportViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<History> histories = new ObservableCollection<History>();

    [ObservableProperty]
    ObservableCollection<string> shifts = new ObservableCollection<string>() {"All", "A","B","C"};
    [ObservableProperty]
    string shift;
    private readonly AppDbContext _db;
    private PrinterService _printerService;


    [ObservableProperty]
    DateTime? startDate;
    [ObservableProperty]
    DateTime? endDate;

    [ObservableProperty]
    DateTime? startTime ;
    [ObservableProperty]
    DateTime? endTime;




    public UCReportViewModel(AppDbContext db, PrinterService printerService)
    {
        _db = db;
        _printerService = printerService;
        Reload();
    }
    private void Reload()
    {

        int index = 1;

        Histories.Clear();
        var data = _db.History.OrderByDescending(p=>p.Id).Take(100).ToList();
        foreach (var item in data)
        {

            item.STT = index;
            Histories.Add(item);
            index++;

        }
    }

    [RelayCommand]
    private async Task Search()
    {

        StartDate.Value.AddHours( StartTime?.Hour ?? 0);
       
        DateTime StartDateSearch = new DateTime(StartDate?.Year ?? 1, StartDate?.Month ?? 1, StartDate?.Day ?? 1, StartTime?.Hour ?? 0, StartTime?.Minute ?? 0, StartTime?.Second ?? 0);
        DateTime EndDateSearch = new DateTime(EndDate?.Year ?? 1, EndDate?.Month ?? 1, EndDate?.Day ?? 1, EndTime?.Hour ?? 0, EndTime?.Minute ?? 0, EndTime?.Second ?? 0);



        if (StartDate == null || EndDate == null)
        {
            Reload();
        }
        else
        {

            if (Shift == "All" || string.IsNullOrEmpty(Shift))
            {
                int index = 1;
                Histories.Clear();
                var data = _db.History.Where(p => p.TimeInsert.Date >= StartDateSearch && p.TimeInsert.Date <= EndDateSearch).OrderByDescending(p => p.Id).ToList();
                foreach (var item in data)
                {
                    item.STT = index;
                    Histories.Add(item);
                    index++;
                }
            }
            else
            {
                int index = 1;
                Histories.Clear();
                var data = _db.History.Where(p => p.Shift == Shift && p.TimeInsert.Date >= StartDateSearch && p.TimeInsert.Date <= EndDateSearch).OrderByDescending(p => p.Id).ToList();
                foreach (var item in data)
                {
                    item.STT = index;
                    Histories.Add(item);
                    index++;
                }
            }

            
        }
       
    }


    [RelayCommand]
    private async Task Update(History history)
    {
        var rs = MessageBox.Show("Bạn chắc chắn muốn cập nhật dữ liệu này không?", "Xác nhận", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        if (rs != MessageBoxResult.Yes) return;
        var datachange = await _db.History.FirstAsync(p => p.Id == history.Id);
        datachange = history;
        _db.SaveChanges();
        Reload();
        var home = App.ServiceProvider.GetRequiredService<UCHomeViewModel>();
        home.UpdateChart();
        home.UpdateHistory();

        var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
        tool.UpdateHistory();

    }

    [RelayCommand]
    private async Task Delete(History history)
    {

        var rs =  MessageBox.Show("Bạn chắc chắn muốn xoá dữ liệu này không?","Xác nhận", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        if (rs != MessageBoxResult.Yes) return;
        var datachange = await _db.History.FirstAsync(p => p.Id == history.Id);
        _db.History.Remove(datachange);
        _db.SaveChanges();

        File.Delete($"C:\\Logger\\{datachange.TimeInsert:dd_MM_yyyy}\\{history.ModelName}_{datachange.TimeInsert:HH_mm_ss_dd_MM_yyyy}.csv");

        Reload();


        var home = App.ServiceProvider.GetRequiredService<UCHomeViewModel>();
        home.UpdateChart();
        home.UpdateHistory();
        var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
        tool.UpdateHistory();


    }

    [RelayCommand]
    private async Task ExportCsvFile()
    {

        try
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.FileName = $"{StartDate:HH_mm_dd_MM_yyyy}({Shift}).csv";

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                StreamWriter writer = null;
                try
                {
                    writer = new StreamWriter(filePath, false, Encoding.UTF8);
                    writer.WriteLine("Shift,Mold,ModelName,Quantity,MaterialName,MaterialCode,MaterialColor,NameError,Position,Persion,PositionError,Reason,Action,TimeInsert");
                    // Ghi dữ liệu


                    foreach (var history in Histories)
                    {
                        writer.WriteLine($"{history.Shift},{history.Mold},{history.ModelName},{history.Quantity},{history.MaterialName},{history.MaterialCode},{history.MaterialColor},{history.NameError},{history.Position},{history.Persion},{history.PositionError},{history.Reason},{history.Action},{history.TimeInsert:yyyy-MM-dd HH:mm:ss}");

                    }

                    MessageBox.Show($"Lưu file CSV thành công tại:\n{filePath}");
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }
        }
        catch (Exception ex)
        {

           MessageBox.Show(ex.ToString());
        }




       


       

    }

    [RelayCommand]
    private async Task Print(History history)
    {

        var rs = MessageBox.Show("Bạn chắc chắn muốn in lại dữ liệu này không?", "Xác nhận", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        if (rs != MessageBoxResult.Yes) return;

        try
        {
            ModelPrint dataprint = new ModelPrint()
            {
                Day = DateTime.Now.Day.ToString(),
                Month = DateTime.Now.Month.ToString(),
                Year = DateTime.Now.Year.ToString(),
                Shift = history.Shift,
                Mold = history.Mold,
                Hour = DateTime.Now.ToString("HH:mm:ss"),
                Model = history.ModelName,
                Quantity = history.Quantity.ToString(),
                MaterialCode = history.MaterialCode,
                MaterialName = history.MaterialName,
                Person = history.Persion,
                NameError = history.NameError,
                Reason = history.Reason,
                MaterialColor = history.MaterialColor
            };
            _printerService.Print(dataprint);
        }
        catch (Exception ex)
        {

           MessageBox.Show(ex.ToString());
        }
       
        

        
    }
}
