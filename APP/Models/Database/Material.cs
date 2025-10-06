using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Models.Database;

public partial class Material : ObservableObject
{
    public int ID { get; set; }
    [ObservableProperty]
    public string modelName;
    [ObservableProperty]
    public string materialName;
    public string TimeUpdate { get; set; }
}
