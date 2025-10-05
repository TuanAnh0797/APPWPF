using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Models.Database;

public class PLCSetting
{
    public string Name { get; set; }
    public string IPAddress { get; set; }
    public int Port { get; set; }
    public int AddressModel { get; set; }
    public int AddressMold { get; set; }
    public int TotalRegisterModel { get; set; }
    public int TotalRegisterMold { get; set; }
    public string TimeUpdate { get; set; }
}
