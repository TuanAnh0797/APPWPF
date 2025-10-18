using APP.Database;
using APP.Models.Database;
using APP.Service;
using APP.ViewModels.FormViewModels;
using APP.ViewModels.UserControlViewModels.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Media.Media3D;

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

        int index = 1;

        ErrorMasters.Clear();
        var data = _db.ErrorMaster.ToList();
        foreach (var item in data)
        {

            item.STT = index;

            ErrorMasters.Add(item);

            index++;

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
                        int rs = await _db.Database.ExecuteSqlRawAsync("DELETE from ErrorMaster");
                        _db.ChangeTracker.Clear();
                        var Errors = new List<ErrorMaster>();

                        foreach (DataRow row in dt.Rows)
                        {
                            try
                            {
                                ErrorMaster new_material = new ErrorMaster()
                                {
                                    ID = int.Parse(row[0].ToString()),
                                    NameError = row[1].ToString(),
                                    Reason = row[2].ToString(),
                                    Action = row[3].ToString(),
                                    TimeUpdate = DateTime.Now.ToString()
                                };
                                Errors.Add(new_material);


                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        await _db.ErrorMaster.AddRangeAsync(Errors);
                        await _db.SaveChangesAsync();
                        Reload();
                        SettingChanged?.Invoke();

                     

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
                string[] data = File.ReadAllLines(filePath).Skip(1)
                    .ToArray();
                int rs = await _db.Database.ExecuteSqlRawAsync("DELETE from ErrorMaster");
                _db.ChangeTracker.Clear();
                var Errors = new List<ErrorMaster>();
                foreach (var row in data)
                {

                    string[] rowData = row.Split(',');

                    try
                    {
                        ErrorMaster new_material = new ErrorMaster()
                        {
                            ID = int.Parse(rowData[0].ToString()),
                            NameError = rowData[1].ToString(),
                            Reason = rowData[2].ToString(),
                            Action = rowData[3].ToString(),
                            TimeUpdate = DateTime.Now.ToString()
                        };
                        Errors.Add(new_material);


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                await _db.ErrorMaster.AddRangeAsync(Errors);
                await _db.SaveChangesAsync();
                Reload();
                SettingChanged?.Invoke();
               

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

        var filtered = ErrorMasters.Where(p => p.NameError!.Contains(data)
                                   || p.Reason!.Contains(data)
                                   || p.Action!.Contains(data) )
                          .ToList();

        ErrorMasters.Clear();
        foreach (var item in filtered) ErrorMasters.Add(item);
    }



}
public class ShowAddPopUp { }
public class HideAddPopUp { }
