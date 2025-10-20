using APP.Database;
using APP.Models.Database;
using APP.Models.Printer;
using APP.Service;
using APP.ViewModels.FormViewModels;
using APP.ViewModels.UserControlViewModels.Home;
using APP.ViewModels.UserControlViewModels.Setting.Sub;
using APP.ViewModels.UserControlViewModels.Tools.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace APP.ViewModels.UserControlViewModels.Tools;

public partial class UCToolsViewModel : ObservableObject
{
    [ObservableProperty]
    private BitmapImage qrCodeImage = null;
    [ObservableProperty]
    List<string> shifts = new List<string>() { "A","B","C"};
    [ObservableProperty]
    string shift;
    [ObservableProperty]
    List<string> machines = new List<string>() { "VF1", "VF2", "VF3", "VF4" };
    [ObservableProperty]
    string machine;
    [ObservableProperty]
    ObservableCollection<string> molds = new ObservableCollection<string>();
    [ObservableProperty]
    string mold;
    [ObservableProperty]
    ObservableCollection<string> models = new ObservableCollection<string>();
    [ObservableProperty]
    string model;
    [ObservableProperty]
    bool isAuto = true;
    [ObservableProperty]
    int quantity = 1;
    [ObservableProperty]
    ObservableCollection<ErrorMaster> nameErrors = new ObservableCollection<ErrorMaster>();
    [ObservableProperty]
    ErrorMaster nameError;
    [ObservableProperty]
    string position;
    [ObservableProperty]
    ObservableCollection<string> persons = new ObservableCollection<string>();
    [ObservableProperty]
    string person;
    [ObservableProperty]
    string positionError;
    [ObservableProperty]
    ObservableCollection<ErrorMaster> reasons = new ObservableCollection<ErrorMaster>();
    [ObservableProperty]
    ErrorMaster reason;
    [ObservableProperty]
    ObservableCollection<ErrorMaster> actions = new ObservableCollection<ErrorMaster>();
    [ObservableProperty]
    ErrorMaster action;
    [ObservableProperty]
    ObservableCollection<Material> codeMaterials = new ObservableCollection<Material>();
    [ObservableProperty]
    Material codeMaterial;
    [ObservableProperty]
    string nameMaterial;
    [ObservableProperty]
    string colorMaterial;
    [ObservableProperty]
    ObservableCollection<HistoryView> historyData = new ObservableCollection<HistoryView>();
    

    private readonly AppDbContext _db;
    private readonly UCMasterSettingViewModel _uCMasterSettingViewModel;
    private readonly UCMaterialSettingViewModel _uCMaterialSettingViewModel;
    private PrinterService _printerService;
    private PLCService _pLCService;

    public UCToolsViewModel(AppDbContext db, UCMasterSettingViewModel uCMasterSettingViewModel, UCMaterialSettingViewModel uCMaterialSettingViewModel, PrinterService printerService, PLCService pLCService )
    {
        _db = db;
        _printerService = printerService;
        _uCMasterSettingViewModel = uCMasterSettingViewModel;
        _uCMasterSettingViewModel.SettingChanged += _uCMasterSettingViewModel_SettingChanged;
        _uCMaterialSettingViewModel = uCMaterialSettingViewModel;
        _pLCService = pLCService;


        _pLCService.ModelChanged += _pLCService_ModelChanged;
        _pLCService.MoldChanged += _pLCService_MoldChanged;

        _pLCService.LastMold = "";
        //_uCMaterialSettingViewModel.SettingChanged += _uCMaterialSettingViewModel_SettingChanged;
        ReloadErrorMaster();
        UpdateHistory();
        //ReloadMaterialMaster();
        LoadMolds();
        LoadPersons();
    }
    public void LoadPersons()
    {
        Persons.Clear();
        var data = _db.Person.Select(p => p.Name).ToList();
        foreach (var item in data)
        {
            Persons.Add(item);
        }
    }

    public void LoadMolds()
    {
        Molds.Clear();
        var data = _db.Material.GroupBy(p => p.Mold).Select(g => g.Key).ToList();
        foreach (var item in data)
        {
            Molds.Add(item);
        }
    }
    partial void OnMoldChanged(string value)
    {
        Models.Clear();
        var data = _db.Material.Where(w=>w.Mold == value).GroupBy(p => p.ModelName).Select(g => g.Key).ToList();
        foreach (var item in data)
        {
            Models.Add(item);
        }
        if (Models.Count == 1)
        {
            Model = Models[0];
        }
        else
        {
            Model = "";
        }
    }

    private void _pLCService_MoldChanged(string obj)
    {
        if (IsAuto)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Mold = obj;
            });
           
        }
    }
    private void _pLCService_ModelChanged(string obj)
    {
        if (IsAuto)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Model = obj;
            });
        }
    }
    private void _uCMaterialSettingViewModel_SettingChanged()
    {
        ReloadMaterialMaster();
    }
    private void _uCMasterSettingViewModel_SettingChanged()
    {
        ReloadErrorMaster();
    }
    private void ReloadMaterialMaster()
    {
        CodeMaterials.Clear();
        var data = _db.Material.ToList();
        foreach (var item in data)
        {
            CodeMaterials.Add(item);
        }
    }

    private void ReloadErrorMaster()
    {
        NameErrors.Clear();
        var data = _db.ErrorMaster.ToList();
        foreach (var item in data)
        {
            NameErrors.Add(item);
        }
    }
    #region On
    partial void OnNameErrorChanged(ErrorMaster value)
    {
        Reasons.Clear();
        if (value != null)
        {
            var data = NameErrors.Where(p => p.NameError == value.NameError);
            foreach (var item in data)
            {
                Reasons.Add(item);
            }
        }

       
    }
    partial void OnReasonChanged(ErrorMaster value)
    {

        if (value != null)
        {
            Actions.Clear();
            var data = Reasons.Where(p => p.Reason == value.Reason);
            foreach (var item in data)
            {
                Actions.Add(item);
            }
        }

       
    }

    partial void OnModelChanged(string value)
    {
        CodeMaterials.Clear();
        var data = _db.Material.Where(p=> p.ModelName == value);
        foreach (var item in data)
        {
            CodeMaterials.Add(item);
        }
    }

    partial void OnCodeMaterialChanged(Material value)
    {
        if (value != null)
        {
            QrCodeImage = GenerateQrCode(value.MaterialName);
            NameMaterial = value.MaterialName;
        }
        else
        {
            QrCodeImage = null;
            NameMaterial = "";
        }
        
    }

    #endregion
    private BitmapImage GenerateQrCode(string text)
    {
        using var generator = new QRCodeGenerator();
        var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

        using var qrCode = new QRCode(data);
        using Bitmap qrBitmap = qrCode.GetGraphic(20);

        using var ms = new MemoryStream();
        qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        ms.Position = 0;

        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = ms;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        return image;
    }
    //[RelayCommand]
    //private void MaterialKeyUp(object parameter)
    //{

    //}
    [RelayCommand]
    private void Print()
    {
        try
        {

            if (string.IsNullOrEmpty(Shift))
            {
                MessageBox.Show("Vui lòng chọn ca làm việc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(Machine))
            {
                MessageBox.Show("Vui lòng chọn máy!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var Checkmold = _db.Material.Where(p => p.Mold == Mold).FirstOrDefault();
            if (Checkmold == null)
            {
                MessageBox.Show("Mã khuôn không tồn tại trong cài đặt. Hãy kiểm liên hệ với Leader để kiểm tra cài đặt!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var Checkmodel= _db.Material.Where(p => p.ModelName == Model).FirstOrDefault();
            if (Checkmodel == null)
            {
                MessageBox.Show("Model không tồn tại trong cài đặt. Hãy kiểm liên hệ với Leader để kiểm tra cài đặt!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

           
            if (NameError == null)
            {
                MessageBox.Show("Vui lòng chọn tên lỗi!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                var checkerror = _db.ErrorMaster.Where(p => p.NameError == NameError.NameError).FirstOrDefault();
                if (checkerror == null)
                {
                    MessageBox.Show("Tên lỗi không tồn tại trong cài đặt. Hãy kiểm liên hệ với Leader để kiểm tra cài đặt!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
               
            }
            if (string.IsNullOrEmpty(Person))
            {
                MessageBox.Show("Vui lòng chọn người thao tác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Reason == null)
            {
                MessageBox.Show("Vui lòng chọn nguyên nhân!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CodeMaterial == null)
            {
                MessageBox.Show("Vui lòng chọn mã linh kiện!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            ModelPrint dataprint = new ModelPrint()
            {
                Day = DateTime.Now.Day.ToString(),
                Month = DateTime.Now.Month.ToString(),
                Year = DateTime.Now.Year.ToString(),
                Shift = Shift,
                Mold = Mold,
                Hour = DateTime.Now.ToString("HH:mm:ss"),
                Model = Model,
                Quantity = Quantity.ToString(),
                MaterialCode = CodeMaterial.MaterialCode,
                MaterialName = NameMaterial,
                Person = Person,
                NameError = NameError.NameError,
                Reason = Reason.Reason,
                MaterialColor = ColorMaterial
            };
            _printerService.Print(dataprint);

            MessageBoxResult rs = MessageBox.Show("Đã in phiếu thành công chưa?", "Xác nhận in phiếu", MessageBoxButton.YesNo,MessageBoxImage.Question);

            DateTime now = DateTime.Now;

            History history = new History()
            {
                Shift = Shift,
                Mold = Mold,
                ModelName = Model,
                Quantity = Quantity,
                MaterialName = NameMaterial,
                MaterialCode = CodeMaterial.MaterialCode,
                MaterialColor = ColorMaterial,
                NameError = NameError.NameError,
                Position = Machine,
                Persion = Person,
                PositionError = Machine,
                Reason = Reason.reason,
                Action = "",
                TimeInsert = now,
            };
            if (rs == MessageBoxResult.Yes)
            {
                SaveHistoryToCsv(   history,  $"C:\\Logger\\{now:dd_MM_yyyy}\\{history.ModelName}_{now:HH_mm_ss_dd_MM_yyyy}.csv" );
                _db.History.Add(history);
                _db.SaveChanges();
                Person = "";
                ColorMaterial = "";
                Reason = null;
                NameError = null;
                CodeMaterial = null;
                UpdateHistory();
            }
            else
            {
                SaveHistoryToCsv(history,  $"C:\\Logger\\Fail\\{now:dd_MM_yyyy}\\{history.ModelName}_{now:HH_mm_ss_dd_MM_yyyy}.csv");
            }


           

        }
        catch (Exception ex)
        {

            MessageBox.Show(ex.Message);
        }
        
    }
    public void UpdateHistory()
    {


      var result = _db.History
     .Where(p => p.TimeInsert.Date == DateTime.Now.Date)
     .GroupBy(p => new { p.ModelName,p.MaterialName })
     .Select(g => new
     {
         g.Key.MaterialName,
         g.Key.ModelName,
         TotalQuantity = g.Sum(x => x.Quantity),
     })
     .ToList();

        HistoryData.Clear();
        int index = 1;
        foreach (var item in result)
        {
            HistoryData.Add(new HistoryView()
            {
                STT = index.ToString(),
                Model = item.ModelName,
                Material = item.MaterialName,
                Quantity = item.TotalQuantity.ToString(),

            });
            index ++;
        }

        var home = App.ServiceProvider.GetRequiredService<UCHomeViewModel>();
        home.UpdateHistory();
        home.UpdateChart();
    }

    private  static void SaveHistoryToCsv(History history, string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        bool fileExists = File.Exists(filePath);
        using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            // Nếu file mới tạo, thêm header
            if (!fileExists)
            {
                writer.WriteLine("Shift,Mold,ModelName,Quantity,MaterialName,MaterialCode,MaterialColor,NameError,Position,Persion,PositionError,Reason,Action,TimeInsert");
            }
            // Ghi dữ liệu một dòng
            writer.WriteLine($"{history.Shift},{history.Mold},{history.ModelName},{history.Quantity},{history.MaterialName},{history.MaterialCode},{history.MaterialColor},{history.NameError},{history.Position},{history.Persion},{history.PositionError},{history.Reason},{history.Action},{history.TimeInsert:yyyy-MM-dd HH:mm:ss}");
        }
    }


}
