using APP.Database;
using APP.Models.Database;
using APP.Service;
using APP.ViewModels.FormViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace APP.ViewModels.UserControlViewModels.Setting.Sub;

public partial class UCPLCSettingViewModel : ObservableObject
{
    private readonly AppDbContext _db;
    [ObservableProperty]
    string iPAddress;
    [ObservableProperty]
    string port;
    [ObservableProperty]
    string modelAddress;
    [ObservableProperty]
    string modelAddressNumber;
    [ObservableProperty]
    string currentModel;
    [ObservableProperty]
    string moldAddress;
    [ObservableProperty]
    string moldAddressNumber;
    [ObservableProperty]
    string currentMold;

    public event Action PLCConfigChanged;

    public UCPLCSettingViewModel(AppDbContext db)
    {
        _db = db;
        GetConfigPLC();
    }


    private void GetConfigPLC()
    {
        var config = _db.PLCSetting.FirstOrDefault();
        if (config == null) return;
        IPAddress = config.IPAddress;
        Port = config.Port.ToString();
        ModelAddress = config.AddressModel.ToString();
        ModelAddressNumber = config.TotalRegisterModel.ToString();
        MoldAddress = config.AddressMold.ToString();
        MoldAddressNumber = config.TotalRegisterMold.ToString();
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            var data = _db.PLCSetting.ToList();
            foreach (var item in data)
            {
                _db.PLCSetting.Remove(item);
            }
            PLCSetting pLCSetting = new PLCSetting()
            {
                Name = "VF",
                IPAddress = IPAddress,
                Port = int.Parse(Port),
                AddressModel = int.Parse(ModelAddress),
                TotalRegisterModel = int.Parse(ModelAddressNumber),
                AddressMold = int.Parse(MoldAddress),
                TotalRegisterMold = int.Parse(MoldAddressNumber),
                TimeUpdate = DateTime.Now,

            };
            _db.PLCSetting.Add(pLCSetting);
            _db.SaveChanges();
            GetConfigPLC();
            PLCConfigChanged?.Invoke();
        }
        catch (Exception ex)
        {

            MessageBox.Show(ex.Message);
        }
       
    }
}
