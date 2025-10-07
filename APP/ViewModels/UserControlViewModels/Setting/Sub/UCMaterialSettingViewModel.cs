using APP.Database;
using APP.Models.Database;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace APP.ViewModels.UserControlViewModels.Setting.Sub;

public partial class UCMaterialSettingViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<Material> materials = new ObservableCollection<Material>();
    private readonly AppDbContext _db;
    public event Action SettingChanged;
    [ObservableProperty]
    string modelName;
    [ObservableProperty]
    string materialName;
    [ObservableProperty]
    string materialCode;
    [ObservableProperty]
    Visibility isShowAdd = Visibility.Collapsed;

    public UCMaterialSettingViewModel(AppDbContext db)
    {
        _db = db;
        Reload();
    }
    private void Reload()
    {
        Materials.Clear();
        var data = _db.Material.ToList();
        foreach (var item in data)
        {
            Materials.Add(item);
        }
    }
    [RelayCommand]
    private async Task Update(Material material)
    {
        var current = await _db.Material.FirstAsync(p => p.ID == material.ID);
        current.ModelName = material.ModelName;
        current.MaterialName = material.MaterialName;
        current.MaterialCode = material.MaterialCode;
        current.TimeUpdate = DateTime.Now.ToString();
        _db.SaveChanges();
        Reload();
        SettingChanged?.Invoke();
    }
    [RelayCommand]
    private async Task DeleteAsync(Material material)
    {
        var current = await _db.Material.FirstAsync(p => p.ID == material.ID);
        _db.Material.Remove(current);
        _db.SaveChanges();
        Reload();
        SettingChanged?.Invoke();
    }
    [RelayCommand]
    private void ShowAdd()
    {
        IsShowAdd = Visibility.Visible;
    }
    [RelayCommand]
    private void CloseAdd()
    {
        IsShowAdd = Visibility.Collapsed;
    }
    [RelayCommand]
    private async Task SaveAdd()
    {
        try
        {
            Material new_material = new Material()
            {
                ModelName = ModelName,
                MaterialName = MaterialName,
                MaterialCode = MaterialCode,
                TimeUpdate = DateTime.Now.ToString()
            };
            await _db.Material.AddAsync(new_material);
            await _db.SaveChangesAsync();
            Reload();
            SettingChanged?.Invoke();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            _db.ChangeTracker.Clear();
        }
      
    }
}
