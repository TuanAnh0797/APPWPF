using APP.Models.Home;
using APP.UserControls;
using APP.UserControls.Home.ChildrenUC;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace APP.ViewModels.UserControlViewModels.Home
{
    public partial class UCHomeViewModel: ObservableObject
    {
        //[ObservableProperty]
        //private UserControl currentView;
        //[ObservableProperty]
        //public List<FuntionHome> funtionHomes = new List<FuntionHome>()
        //{
        //    new FuntionHome() { NameIcon = "DeveloperBoard", NameLabel = "Manage",ColorIcon = Brushes.DarkBlue  },
        //    new FuntionHome() { NameIcon = "BookEdit", NameLabel = "Report", ColorIcon =  Brushes.Red },
        //    new FuntionHome() { NameIcon = "History", NameLabel = "History", ColorIcon =  Brushes.SlateBlue },
        //    new FuntionHome() { NameIcon = "FridgeOutline", NameLabel = "Progress", ColorIcon =  Brushes.Chocolate  }
        //};

        //private FuntionHome _selectedFunction { get; set; }

        //public FuntionHome SelectedFunction
        //{
        //    get
        //    {
        //        return _selectedFunction;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            _selectedFunction = value;
        //            if (_selectedFunction != null)
        //            {
        //                switch (_selectedFunction.NameLabel)
        //                {
        //                    case "Manage":
        //                        ShowManage();
        //                        break;
        //                    case "Report":
        //                        ShowReport();
        //                        break;
        //                    case "History":
        //                        ShowHistory();
        //                        break;
        //                    case "Progress":
        //                        ShowProgess();
        //                        break;
        //                }
        //            }
        //            OnPropertyChanged(nameof(SelectedFunction));
        //        }

               
        //    }
        //}


        public UCHomeViewModel() 
        {
           
        }
        //[RelayCommand]
        //private void ShowManage()
        //{
        //    CurrentView = new UCManage();
            
        //}
        //[RelayCommand]
        //private void ShowReport()
        //{
        //    CurrentView = new UCReport();
           
        //}
        //[RelayCommand]
        //private void ShowHistory()
        //{
        //    CurrentView = new UCHistory();
           
        //}
        //[RelayCommand]
        //private void ShowProgess()
        //{
        //    CurrentView = new UCProgress();
           
        //}
    }
}
