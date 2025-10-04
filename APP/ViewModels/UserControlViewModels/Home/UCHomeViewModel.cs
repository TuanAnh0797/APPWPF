using APP.Models.Home;
using APP.UserControls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace APP.ViewModels.UserControlViewModels.Home
{
    public partial class UCHomeViewModel : ObservableObject
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

        //
        private readonly Random _rnd = new();

        // --- Cartesian ---
        [ObservableProperty]
        private ISeries[] cartesianSeries;

        [ObservableProperty]
        private Axis[] xAxes;

        [ObservableProperty]
        private Axis[] yAxes;

        // --- Scatter ---
 
        [ObservableProperty]
        private ISeries[] lineSeries;

        [ObservableProperty]
        private Axis[] lineXAxes;

        [ObservableProperty]
        private Axis[] lineYAxes;

        // --- Pie ---
        [ObservableProperty]
        private ISeries[] pieSeries;

        //
        [ObservableProperty]
        private ISeries[] radarSeries;

        [ObservableProperty]
        private PolarAxis[] radarAxes;

        public UCHomeViewModel()
        {
            // Cartesian: Column + Line
            cartesianSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Revenue",
                    Values = new ObservableCollection<double> { 5, 8, 6, 7, 9, 10, 8, 11 }
                },
                new LineSeries<double>
                {
                    Name = "Trend",
                    Values = new ObservableCollection<double> { 4, 7, 5, 6, 8, 9, 7, 10 },

                    Fill = null
                }
            };

            xAxes = new Axis[] { new Axis { Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug" } } };
            yAxes = new Axis[] { new Axis { Name = "Value" } };

            // 1. Line chart (time series)
            var start = DateTime.Today.AddDays(-6);
            var values = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(start, 5),
                new DateTimePoint(start.AddDays(1), 8),
                new DateTimePoint(start.AddDays(2), 6),
                new DateTimePoint(start.AddDays(3), 7),
                new DateTimePoint(start.AddDays(4), 9),
                new DateTimePoint(start.AddDays(5), 4),
                new DateTimePoint(start.AddDays(6), 10)
            };

            lineSeries = new ISeries[]
            {
                new LineSeries<DateTimePoint>
                {
                    Name = "Sales",
                    Values = values,
                    Fill = null,
                }
            };

            lineXAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => new DateTime((long)value).ToString("MM-dd"),
                    LabelsRotation = 45
                }
            };

            lineYAxes = new Axis[]
            {
                new Axis { Name = "Units" }
            };
            // Pie
            pieSeries = new ISeries[]
            {
                new PieSeries<double> { Name="Product A", Values = new ObservableCollection<double>{35} },
                new PieSeries<double> { Name="Product B", Values = new ObservableCollection<double>{25} },
                new PieSeries<double> { Name="Product C", Values = new ObservableCollection<double>{20} },
                new PieSeries<double> { Name="Product D", Values = new ObservableCollection<double>{20} },
            };
            // Radar chart (PolarChart)
            radarSeries = new ISeries[]
            {
        new PolarLineSeries<double>
        {
            Name = "Quality",
            Values = new double[] { 8, 6, 7, 9, 5 },
            GeometrySize = 10
        },
        new PolarLineSeries<double>
        {
            Name = "Target",
            Values = new double[] { 7, 7, 7, 7, 7 },
            GeometrySize = 5
        }
            };

            radarAxes = new PolarAxis[]
            {
        new PolarAxis
        {
            Labels = new[] { "Speed", "Power", "Accuracy", "Range", "Durability" }
        }
            };

        }

    }
}
