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



    public UCHomeViewModel(AppDbContext db)
    {

        _db = db;
        UpdateHistory();



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
                DataLabelsFormatter = point => point.Model.ToString("0") ,
                Padding = 2,


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
            LabelsRotation = 90,
            TextSize= 12

        } };
        DataErrorByMaterial.Clear();
        foreach (var item in result)
        {
            DataErrorByMaterial.Add(item.TotalQuantity);
        }

        if (DataErrorByMaterial.Count == 0)
        {
            YAxes[0].MaxLimit = 10;
        }
        else
        {
            YAxes[0].MaxLimit = DataErrorByMaterial.Max() + 10;
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
           LabelsRotation = 45, // xoay 45 độ
        TextSize = 12,
        } };

        int index = 1;


        DataErrorByDate.Clear();
        foreach (var item in Enumerable.Range(1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)))
        {
            var rs = resultByDate.Where(p => p.Date.Day == item).FirstOrDefault();
            if (rs != null)
            {
                DataErrorByDate.Add(rs.TotalQuantity);
            }
            else
            {
                DataErrorByDate.Add(0);
            }
        }

        if (DataErrorByDate.Count == 0)
        {
            YAxesByDate[0].MaxLimit = 10;
        }
        else
        {
            YAxesByDate[0].MaxLimit = DataErrorByDate.Max() + 1;
        }








    }

}
