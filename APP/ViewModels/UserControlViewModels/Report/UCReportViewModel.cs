using APP.Database;
using APP.Models.Database;
using APP.Models.Printer;
using APP.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
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
    private readonly AppDbContext _db;
    private PrinterService _printerService;


    [ObservableProperty]
    DateTime? startDate;
    [ObservableProperty]
    DateTime? endDate;



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
        if (StartDate == null || EndDate == null)
        {
            Reload();
        }
        else
        {
            int index = 1;
            Histories.Clear();
            var data = _db.History.Where(p => p.TimeInsert.Date >= StartDate && p.TimeInsert.Date <= EndDate).OrderByDescending(p => p.Id).ToList();
            foreach (var item in data)
            {
                item.STT = index;
                Histories.Add(item);
                index++;
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
