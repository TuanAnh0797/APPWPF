using APP.Database;
using APP.Models.Database;
using APP.ViewModels.UserControlViewModels.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

namespace APP.ViewModels.UserControlViewModels.Setting.Sub;

public partial class UCPersonSettingViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<Person> persons = new ObservableCollection<Person>();
    private readonly AppDbContext _db;
    public UCPersonSettingViewModel(AppDbContext db)
    {
        _db = db;
        Reload();
    }

    private void Reload()
    {


        int index = 1;
        Persons.Clear();
        var data = _db.Person.ToList();
        foreach (var item in data)
        {
            item.STT = index;
            Persons.Add(item);
            index++;
        }
    }


    [RelayCommand]
    private async Task Update(Person person)
    {
        var datachange = await _db.Person.FirstAsync(p => p.ID == person.ID);
        datachange.Name = person.Name;
       
        _db.SaveChanges();
        Reload();
        var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
        tool.LoadPersons();

    }

    [RelayCommand]
    private async Task DeleteAsync(Person person)
    {
        var datachange = await _db.Person.FirstAsync(p => p.ID == person.ID);
        _db.Person.Remove(datachange);
        _db.SaveChanges();
        Reload();
        var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
        tool.LoadPersons();

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
                        int rs = await _db.Database.ExecuteSqlRawAsync("DELETE from Person");
                        _db.ChangeTracker.Clear();
                        var ps = new List<Person>();

                        foreach (DataRow row in dt.Rows)
                        {
                            try
                            {
                                Person new_person = new Person()
                                {
                                    ID = int.Parse(row[0].ToString()),
                                    Name = row[1].ToString(),
                                    TimeUpdate = DateTime.Now.ToString()
                                };
                                ps.Add(new_person);


                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        await _db.Person.AddRangeAsync(ps);
                        await _db.SaveChangesAsync();
                        Reload();
                        var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
                        tool.LoadPersons ();


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
                int rs = await _db.Database.ExecuteSqlRawAsync("DELETE from Person");
                _db.ChangeTracker.Clear();
                var ps = new List<Person>();
                foreach (var row in data)
                {

                    string[] rowData = row.Split(',');

                    try
                    {
                        Person new_person = new Person()
                        {
                            ID = int.Parse(rowData[0].ToString()),
                            Name = rowData[1].ToString(),
                            TimeUpdate = DateTime.Now.ToString()
                        };
                        ps.Add(new_person);


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                await _db.Person.AddRangeAsync(ps);
                await _db.SaveChangesAsync();
                Reload();
                var tool = App.ServiceProvider.GetRequiredService<UCToolsViewModel>();
                tool.LoadPersons();
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

        var filtered = Persons.Where(p => p.Name!.Contains(data)
                                  )
                          .ToList();

        Persons.Clear();
        foreach (var item in filtered) Persons.Add(item);
    }
}
