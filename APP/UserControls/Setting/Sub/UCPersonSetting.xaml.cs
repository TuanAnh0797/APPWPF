using APP.ViewModels.UserControlViewModels.Setting.Sub;
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
    /// Interaction logic for UCPersonSetting.xaml
    /// </summary>
    public partial class UCPersonSetting : UserControl
    {
        private UCPersonSettingViewModel _viewModel;
        public UCPersonSetting()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<UCPersonSettingViewModel>();
            DataContext = _viewModel;
        }
    }
}
