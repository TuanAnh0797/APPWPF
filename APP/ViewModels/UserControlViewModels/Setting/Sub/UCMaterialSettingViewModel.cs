using APP.Database;
using APP.Models.Database;
using APP.ViewModels.UserControlViewModels.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
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
    string mold;
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
        int index = 1;
        Materials.Clear();
        var data = _db.Material.ToList().OrderBy(p=> p.ID);
        foreach (var item in data)
        {
            item.STT = index;
            Materials.Add(item);
            index++;
        }
    }
    


    [RelayCommand]
    private async Task Update(Material material)
    {
        var current = await _db.Material.FirstAsync(p => p.ID == material.ID);
        current.ModelName = material.ModelName;
        current.Mold = material.Mold;
        current.MaterialName = material.MaterialName;
        current.MaterialCode = material.MaterialCode;
        current.TimeUpdate = DateTime.Now.ToString();
        _db.SaveChanges();
        Reload();
        SettingChanged?.Invoke();
        var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
        tool.LoadMolds();
    }
    [RelayCommand]
    private async Task DeleteAsync(Material material)
    {
        var current = await _db.Material.FirstAsync(p => p.ID == material.ID);
        _db.Material.Remove(current);
        _db.SaveChanges();
        Reload();
        SettingChanged?.Invoke();
        var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
        tool.LoadMolds();
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
                Mold = Mold,
                MaterialName = MaterialName,
                MaterialCode = MaterialCode,
                TimeUpdate = DateTime.Now.ToString()
            };
            await _db.Material.AddAsync(new_material);
            await _db.SaveChangesAsync();
            Reload();
            SettingChanged?.Invoke();
            var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
            tool.LoadMolds();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            _db.ChangeTracker.Clear();
        }
      
    }
    [RelayCommand]
    private async Task ImportExcelFile()
    {
        try
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1;\"";
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    DataTable? dtSheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dtSheet != null)
                    {
                        string query = $"SELECT  * FROM [Master$]";
                        DataTable dt = new DataTable();
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection))
                        {
                            adapter.Fill(dt);
                        }
                         int rs = await _db.Database.ExecuteSqlRawAsync("DELETE from Material");
                        _db.ChangeTracker.Clear();
                        var materials = new List<Material>();

                        foreach (DataRow row in dt.Rows)
                        {
                            try
                            {
                                Material new_material = new Material()
                                {
                                    ID = int.Parse(row[0].ToString()),
                                    ModelName = row[1].ToString(),
                                    Mold = row[2].ToString(),
                                    MaterialName = row[3].ToString(),
                                    MaterialCode = row[4].ToString(),
                                    TimeUpdate = DateTime.Now.ToString()
                                };
                                materials.Add(new_material);


                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        await _db.Material.AddRangeAsync(materials);
                        await _db.SaveChangesAsync();
                        Reload();
                        SettingChanged?.Invoke();
                        var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
                        tool.LoadMolds();

                    }
                }

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }
    [RelayCommand]
    private async Task ImportCsvFile()
    {
        try
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Csv Files|*.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string[] data = File.ReadAllLines(filePath, Encoding.UTF8).Skip(1)
                    .ToArray();
                int rs = await _db.Database.ExecuteSqlRawAsync("DELETE from Material");
                _db.ChangeTracker.Clear();
                var materials = new List<Material>();
                foreach (var row in data)
                {

                    string[] rowData = row.Split(',');

                    try
                    {
                        Material new_material = new Material()
                        {
                            ID = int.Parse(rowData[0].ToString()),
                            ModelName = rowData[1].ToString(),
                            Mold = rowData[2].ToString(),
                            MaterialName = rowData[3].ToString(),
                            MaterialCode = rowData[4].ToString(),
                            TimeUpdate = DateTime.Now.ToString()
                        };
                        materials.Add(new_material);


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                await _db.Material.AddRangeAsync(materials);
                await _db.SaveChangesAsync();
                Reload();
                SettingChanged?.Invoke();
                var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
                tool.LoadMolds();

            }
        }

        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());

        }

    }
    [RelayCommand]
    private void Search(string data)
    {
        Reload();
        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        var filtered = Materials.Where(p => p.ModelName!.Contains(data)
                                   || p.Mold!.Contains(data)
                                   || p.MaterialName!.Contains(data)
                                   || p.MaterialCode!.Contains(data))
                          .ToList();

        Materials.Clear();
        foreach (var item in filtered) Materials.Add(item);
    }
}
