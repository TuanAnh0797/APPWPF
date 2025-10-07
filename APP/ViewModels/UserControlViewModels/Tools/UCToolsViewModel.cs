using APP.Database;
using APP.Models.Database;
using APP.Models.Printer;
using APP.Service;
using APP.ViewModels.UserControlViewModels.Setting.Sub;
using APP.ViewModels.UserControlViewModels.Tools.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    string mold;
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
        //_uCMaterialSettingViewModel.SettingChanged += _uCMaterialSettingViewModel_SettingChanged;
        ReloadErrorMaster();
        UpdateHistory();
        //ReloadMaterialMaster();
    }

    private void _pLCService_MoldChanged(string obj)
    {
        if (IsAuto)
        {
            Mold = obj;
        }
    }
    private void _pLCService_ModelChanged(string obj)
    {
        if (IsAuto)
        {
            Model = obj;
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
        var data = NameErrors.Where(p => p.NameError == value.NameError);
        foreach (var item in data)
        {
            Reasons.Add(item);
        }
    }
    partial void OnReasonChanged(ErrorMaster value)
    {
        Actions.Clear();
        var data = Reasons.Where(p => p.Reason == value.Reason);
        foreach (var item in data)
        {
            Actions.Add(item);
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
                Position = "VF",
                Persion = Person,
                PositionError = "VF",
                Reason = Reason.reason,
                Action = "",
                TimeInsert = DateTime.Now,
            };
            _db.History.Add(history);
            _db.SaveChanges();
            UpdateHistory();

        }
        catch (Exception ex)
        {

            MessageBox.Show(ex.Message);
        }
        
    }
    private void UpdateHistory()
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
    }

  
}
