using APP.Database;
using APP.Models.Database;
using APP.ViewModels.UserControlViewModels.Setting.Sub;
using CommunityToolkit.Mvvm.ComponentModel;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace APP.ViewModels.UserControlViewModels.Tools;

public partial class UCToolsViewModel : ObservableObject
{
    [ObservableProperty]
    private BitmapImage qrCodeImage1 = null;
    [ObservableProperty]
    private BitmapImage qrCodeImage2 = null;
    [ObservableProperty]
    private BitmapImage qrCodeImage3 = null;
    [ObservableProperty]
    List<string> shifts = new List<string>() { "1","2","3"};
    [ObservableProperty]
    string shift;
    [ObservableProperty]
    string mold;
    [ObservableProperty]
    string model;
    [ObservableProperty]
    bool isAuto = true;
    [ObservableProperty]
    int quantity;
    [ObservableProperty]
    string codeMaterial;
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
    private readonly AppDbContext _db;
    private readonly UCMasterSettingViewModel _uCMasterSettingViewModel;

    public UCToolsViewModel(AppDbContext db, UCMasterSettingViewModel uCMasterSettingViewModel)
    {
        _db = db;
        _uCMasterSettingViewModel = uCMasterSettingViewModel;
        _uCMasterSettingViewModel.SettingChanged += _uCMasterSettingViewModel_SettingChanged;
        Reload();
    }

    private void _uCMasterSettingViewModel_SettingChanged()
    {
        Reload();
    }

    private void Reload()
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
}
