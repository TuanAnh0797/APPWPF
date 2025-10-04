using CommunityToolkit.Mvvm.ComponentModel;
using QRCoder;
using System;
using System.Collections.Generic;
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
    private BitmapImage qrCodeImage;
    public UCToolsViewModel()
    {
        GenerateQrCode("https://example.com");
    }
    private void GenerateQrCode(string text)
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
        QrCodeImage = image;
    }
}
