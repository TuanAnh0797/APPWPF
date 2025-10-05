using APP.ViewModels.FormViewModels;
using APP.ViewModels.UserControlViewModels.Setting;
using APP.ViewModels.UserControlViewModels.Setting.Sub;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APP.UserControls.Setting.Sub
{
    /// <summary>
    /// Interaction logic for UCMasterSetting.xaml
    /// </summary>
    public partial class UCMasterSetting : UserControl
    {
        private UCMasterSettingViewModel _viewModel;
        public UCMasterSetting()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<UCMasterSettingViewModel>();
            DataContext = _viewModel;

            WeakReferenceMessenger.Default.Register<ShowAddPopUp>(this, (r, m) =>
            {
                AddPopup.Visibility = Visibility.Visible;
            });

            WeakReferenceMessenger.Default.Register<HideAddPopUp>(this, (r, m) =>
            {
                AddPopup.Visibility = Visibility.Collapsed;
            });
        }
    }
}
