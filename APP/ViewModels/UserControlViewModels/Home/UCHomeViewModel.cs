using APP.Database;
using APP.Models.Home;
using APP.UserControls;
using APP.ViewModels.UserControlViewModels.Tools.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace APP.ViewModels.UserControlViewModels.Home;

public partial class UCHomeViewModel : ObservableObject
{
    [ObservableProperty]
    ObservableCollection<HistoryView> historyData = new ObservableCollection<HistoryView>();
    ObservableCollection<int> DataErrorByMaterial = new ObservableCollection<int>();
    ObservableCollection<int> DataErrorByDate = new ObservableCollection<int>();




    private readonly AppDbContext _db;

    //
    private readonly Random _rnd = new();

    // --- Cartesian ---
    [ObservableProperty]
    private ISeries[] errorbyMaterialSeries;
    [ObservableProperty]
    private Axis[] xAxes;
    [ObservableProperty]
    private Axis[] yAxes;

    [ObservableProperty]
    private ISeries[] errorbyDateSeries;
    [ObservableProperty]
    private Axis[] xAxesByDate;
    [ObservableProperty]
    private Axis[] yAxesByDate;





    //// --- Scatter ---

    //[ObservableProperty]
    //private ISeries[] lineSeries;

    //[ObservableProperty]
    //private Axis[] lineXAxes;

    //[ObservableProperty]
    //private Axis[] lineYAxes;

    //// --- Pie ---
    //[ObservableProperty]
    //private ISeries[] pieSeries;

    ////
    //[ObservableProperty]
    //private ISeries[] radarSeries;

    //[ObservableProperty]
    //private PolarAxis[] radarAxes;

    public UCHomeViewModel(AppDbContext db)
    {

        _db = db;
        UpdateHistory();

        // Cartesian: Column + Line
       

    //    // 1. Line chart (time series)
    //    var start = DateTime.Today.AddDays(-6);
    //    var values = new ObservableCollection<DateTimePoint>
    //    {
    //        new DateTimePoint(start, 5),
    //        new DateTimePoint(start.AddDays(1), 8),
    //        new DateTimePoint(start.AddDays(2), 6),
    //        new DateTimePoint(start.AddDays(3), 7),
    //        new DateTimePoint(start.AddDays(4), 9),
    //        new DateTimePoint(start.AddDays(5), 4),
    //        new DateTimePoint(start.AddDays(6), 10)
    //    };

    //    lineSeries = new ISeries[]
    //    {
    //        new LineSeries<DateTimePoint>
    //        {
    //            Name = "Sales",
    //            Values = values,
    //            Fill = null,
    //        }
    //    };

    //    lineXAxes = new Axis[]
    //    {
    //        new Axis
    //        {
    //            Labeler = value => new DateTime((long)value).ToString("MM-dd"),
    //            LabelsRotation = 45
    //        }
    //    };

    //    lineYAxes = new Axis[]
    //    {
    //        new Axis { Name = "Units" }
    //    };
    //    // Pie
    //    pieSeries = new ISeries[]
    //    {
    //        new PieSeries<double> { Name="Product A", Values = new ObservableCollection<double>{35} },
    //        new PieSeries<double> { Name="Product B", Values = new ObservableCollection<double>{25} },
    //        new PieSeries<double> { Name="Product C", Values = new ObservableCollection<double>{20} },
    //        new PieSeries<double> { Name="Product D", Values = new ObservableCollection<double>{20} },
    //    };
    //    // Radar chart (PolarChart)
    //    radarSeries = new ISeries[]
    //    {
    //new PolarLineSeries<double>
    //{
    //    Name = "Quality",
    //    Values = new double[] { 8, 6, 7, 9, 5 },
    //    GeometrySize = 10
    //},
    //new PolarLineSeries<double>
    //{
    //    Name = "Target",
    //    Values = new double[] { 7, 7, 7, 7, 7 },
    //    GeometrySize = 5
    //}
    //    };

    //    radarAxes = new PolarAxis[]
    //    {
    //new PolarAxis
    //{
    //    Labels = new[] { "Speed", "Power", "Accuracy", "Range", "Durability" }
    //}
    //    };

    }
    public void UpdateHistory()
    {


        var result = _db.History
       .Where(p => p.TimeInsert.Date == DateTime.Now.Date)
       .GroupBy(p => new { p.ModelName, p.MaterialName })
       .Select(g => new
       {
           g.Key.MaterialName,
           g.Key.ModelName,
           TotalQuantity = g.Sum(x => x.Quantity),
       })
       .ToList();

        HistoryData.Clear();
        int index = 1;
        foreach (var item in result)
        {
            HistoryData.Add(new HistoryView()
            {
                STT = index.ToString(),
                Model = item.ModelName,
                Material = item.MaterialName,
                Quantity = item.TotalQuantity.ToString(),

            });
            index++;
        }
        ErrorbyMaterialSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Name = "Material",
                Values = DataErrorByMaterial,
                Fill = new SolidColorPaint(SKColors.OrangeRed), 
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                DataLabelsSize = 14,
                DataLabelsFormatter = point => point.Model.ToString("0") 
            },
        };
        YAxes = new Axis[] { new Axis { Name = "Quantity",
            MinStep = 1,
            MinLimit = 0,
             ShowSeparatorLines = true, 
        } };



        ErrorbyDateSeries = new ISeries[]
        {
            new LineSeries<int>
            {
                Name = "Quantity by Date",
                Values = DataErrorByDate,
                Fill = null,
                GeometrySize = 5,
                 Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 },

            }
        };
        YAxesByDate = new Axis[] { new Axis { Name = "Quantity",
            MinStep = 1,
            MinLimit = 0,
            ShowSeparatorLines = true,
        } };


        UpdateChart();

    }

    public void UpdateChart()
    {
      var result = _db.History
      .Where(p => p.TimeInsert.Month == DateTime.Now.Month)
      .GroupBy(p => new { p.MaterialName })
      .Select(g => new
      {
          g.Key.MaterialName,
          TotalQuantity = g.Sum(x => x.Quantity),
      })
      .ToList();

        XAxes = new Axis[] {new Axis
        {
            Labels = result.Select(p=> p.MaterialName).ToArray(),
            ShowSeparatorLines = true,
        } };
        DataErrorByMaterial.Clear();
        foreach (var item in result)
        {
            DataErrorByMaterial.Add(item.TotalQuantity);
        }


        var resultByDate = _db.History
       .Where(p => p.TimeInsert.Month == DateTime.Now.Month)
       .GroupBy(p => new { p.TimeInsert.Date })
       .Select(g => new
       {
         g.Key.Date,
         TotalQuantity = g.Sum(x => x.Quantity),
       }).ToList();

        XAxesByDate = new Axis[] {new Axis
        {
             Labels = Enumerable
            .Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
            .Select(day => day.ToString())  // 👈 chuyển int sang string
            .ToArray(),
            ShowSeparatorLines = true,
        } };

        int index = 1;


        DataErrorByDate.Clear();
        foreach (var item in Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)))
        {
           var rs =  resultByDate.Where(p => p.Date.Day == item).FirstOrDefault();
            if (rs != null)
            {
                DataErrorByDate.Add(rs.TotalQuantity);
            }
            else
            {
                DataErrorByDate.Add(0);
            }
        }




       


    }

}
