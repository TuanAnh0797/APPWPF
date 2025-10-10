using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Models.Database;

public partial class ErrorMaster : ObservableObject
{
    public int? ID { get; set; }
    [ObservableProperty]
    public string? nameError;
    [ObservableProperty]
    public string? reason;
    [ObservableProperty]
    public string? action;
    public string? TimeUpdate { get; set; }
    public ErrorMaster()
    {

    }
}
