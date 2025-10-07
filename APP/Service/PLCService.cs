using APP.Database;
using APP.Helper;
using APP.ViewModels.UserControlViewModels.Setting.Sub;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace APP.Service;
public class PLCService
{
    private readonly AppDbContext _db;
    PLC Plc = new PLC();


    private string IpAddress;
    private int PortNumber;
    private int AddressModel;
    private int AddressModelNumber;
    private int AddressMold;
    private int AddressMoldNumber;
    Task _readTask;
    public event Action<string> ModelChanged;
    public event Action<string> MoldChanged;
    string LastModel = "";
    string LastMold = "";

    bool _isRunning = false;
    private CancellationTokenSource _cts;

    private UCPLCSettingViewModel _uCPLCSettingViewModel;
    public PLCService(AppDbContext db, UCPLCSettingViewModel uCPLCSettingViewModel)
    {
        _db = db;
        _uCPLCSettingViewModel = uCPLCSettingViewModel;
        _uCPLCSettingViewModel.PLCConfigChanged += _uCPLCSettingViewModel_PLCConfigChanged;
        loadconfig();
    }

    private void _uCPLCSettingViewModel_PLCConfigChanged()
    {
        loadconfig();
    }

    private void loadconfig()
    {
        var config = _db.PLCSetting.FirstOrDefault();
        if (config == null) return;
        IpAddress = config.IPAddress;
        PortNumber = config.Port;
        AddressModel = config.AddressModel;
        AddressModelNumber = config.TotalRegisterModel;
        AddressMold = config.AddressMold;
        AddressMoldNumber = config.TotalRegisterMold;
    }
    

    public void Start()
    {
        if (_isRunning)
        {
            Console.WriteLine("PLC service is already running.");
            return; // tránh tạo thêm task
        }

        _isRunning = true;
        _cts = new CancellationTokenSource();

        _readTask = Task.Run(() => RunLoop(_cts.Token));
    }

    public void Stop()
    {
        _isRunning = false;
    }


    private async Task RunLoop(CancellationToken token)
    {
        while (_isRunning && !token.IsCancellationRequested)
        {
            try
            {
                using (TcpClient tcpclient = new TcpClient())
                {
                    var connectTask = tcpclient.ConnectAsync(IPAddress.Parse(IpAddress), PortNumber);
                    var timeout = Task.Delay(5000, token);

                    if (await Task.WhenAny(connectTask, timeout) != connectTask)
                        throw new TimeoutException("PLC connection timeout");

                    await connectTask;
                    NetworkStream stream = tcpclient.GetStream();

                    string modelName = (string)await Plc.ReadData(stream, 5000, "D", AddressModel, AddressModelNumber, "String");
                    if (modelName != LastModel)
                    {
                        ModelChanged?.Invoke(modelName);
                        LastModel = modelName;
                    }

                    string moldCode = (string)await Plc.ReadData(stream, 5000, "D", AddressMold, AddressMoldNumber, "String");
                    if (moldCode != LastMold)
                    {
                        MoldChanged?.Invoke(moldCode);
                        LastMold = moldCode;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // bị hủy -> thoát vòng lặp
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PLC error: {ex.Message}");
            }

            await Task.Delay(5000, token);
        }
    }



}
