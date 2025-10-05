using APP.Database;
using APP.Models.Database;
using APP.Service;
using APP.ViewModels.FormViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace APP.ViewModels.UserControlViewModels.Setting.Sub;

public partial class UCMasterSettingViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<ErrorMaster> errorMasters = new ObservableCollection<ErrorMaster>();
    [ObservableProperty]
    string nameError;
    [ObservableProperty]
    string reason;
    [ObservableProperty]
    string action;
    private readonly AppDbContext _db;


    public event Action SettingChanged;

    public UCMasterSettingViewModel(AppDbContext db)
    {
        _db = db;
        Reload();
    }

    private void Reload()
    {
        ErrorMasters.Clear();
        var data = _db.ErrorMaster.ToList();
        foreach (var item in data)
        {
            ErrorMasters.Add(item);
        }
    }

    [RelayCommand]
    private async Task Update(ErrorMaster errorMaster)
    {
       var datachange = await  _db.ErrorMaster.FirstAsync(p=> p.ID == errorMaster.ID);
       datachange.NameError = errorMaster.NameError;
       datachange.Action = errorMaster.Action;
       datachange.Reason = errorMaster.Reason;
       _db.SaveChanges();
       Reload();
        SettingChanged?.Invoke();
   }

    [RelayCommand]
    private async Task DeleteAsync(ErrorMaster errorMaster)
    {
        var datachange = await _db.ErrorMaster.FirstAsync(p => p.ID == errorMaster.ID);
        _db.ErrorMaster.Remove(datachange);
        _db.SaveChanges();
        Reload();
        SettingChanged?.Invoke();
    }
    [RelayCommand]
    private void ShowAdd()
    {
        WeakReferenceMessenger.Default.Send(new ShowAddPopUp());
    }

    [RelayCommand]
    private void CloseAdd()
    {
        WeakReferenceMessenger.Default.Send(new HideAddPopUp());
    }
    [RelayCommand]
    private async Task SaveAdd()
    {
        ErrorMaster errorMaster = new ErrorMaster()
        {
            NameError = NameError,
            Reason = Reason,
            Action = Action,
            TimeUpdate = DateTime.Now.ToString()
        };
        await _db.ErrorMaster.AddAsync(errorMaster);
        await _db.SaveChangesAsync();
        Reload();
        SettingChanged?.Invoke();
    }
    

}
public class ShowAddPopUp { }
public class HideAddPopUp { }
