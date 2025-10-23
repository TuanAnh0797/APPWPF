using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Models.Database;

public partial class PrinterSetting : ObservableObject
{
    public string ModelName { get; set; }
    public string Path { get; set; }
    public int Ischoose { get; set; }
    [ObservableProperty]
    public bool? isEnable;
    public string TimeUpdate { get; set; }
}