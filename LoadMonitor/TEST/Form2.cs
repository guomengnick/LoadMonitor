using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.VisualElements;
using LiveChartsCore.SkiaSharpView.WinForms; // 為 WinForms 提供的控件
using LiveChartsCore.SkiaSharpView.Painting;
using LiveCharts.Wpf;
using SkiaSharp;                             // 提供色彩支持


using System;
using System.Collections.Generic;
using System.Windows.Forms;

using FormsTimer = System.Windows.Forms.Timer;

namespace LoadMonitor
{
  public partial class ViewModel
  {
    private readonly Random _random = new();
    public IEnumerable<ISeries> Series { get; set; }
    public IEnumerable<VisualElement<SkiaSharpDrawingContext>> VisualElements { get; set; }
    public NeedleVisual Needle { get; set; }
    
    public ViewModel()
    {
      var sectionsOuter = 130;
      var sectionsWidth = 20;

      Needle = new NeedleVisual
      {
        Value = 45
      };

      Series = GaugeGenerator.BuildAngularGaugeSections(
          new GaugeItem(60, s => SetStyle(sectionsOuter, sectionsWidth, s)),
          new GaugeItem(30, s => SetStyle(sectionsOuter, sectionsWidth, s)),
          new GaugeItem(10, s => SetStyle(sectionsOuter, sectionsWidth, s)));

      VisualElements =
      [
          new AngularTicksVisual
            {
                Labeler = value => value.ToString("N1"),
                LabelsSize = 16,
                LabelsOuterOffset = 15,
                OuterOffset = 65,
                TicksLength = 20
            },
            Needle
      ];
    }

    [RelayCommand]
    public void DoRandomChange()
    {
      // modifying the Value property B and animates the chart automatically
      Needle.Value = _random.Next(15, 25);
    }

    private static void SetStyle(
        double sectionsOuter, double sectionsWidth, PieSeries<ObservableValue> series)
    {
      series.OuterRadiusOffset = sectionsOuter;
      series.MaxRadialColumnWidth = sectionsWidth;
    }
  }

  public partial class View : UserControl
  {
    
    private readonly LiveChartsCore.SkiaSharpView.WinForms.PieChart pieChart;

    public View()
    {
      Size = new System.Drawing.Size(50, 50);

      var viewModel = new ViewModel();

      pieChart = new LiveChartsCore.SkiaSharpView.WinForms.PieChart
      {
        Series = viewModel.Series,
        VisualElements = viewModel.VisualElements,
        InitialRotation = -225,
        MaxAngle = 270,
        MinValue = 0,
        MaxValue = 100,

        // out of livecharts properties...
        Location = new System.Drawing.Point(0, 0),
        Size = new System.Drawing.Size(10,10),
        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
      };

      Controls.Add(pieChart);

      //var b1 = new Button { Text = "Update", Location = new System.Drawing.Point(0, 0) };
      //b1.Click += (object sender, System.EventArgs e) => viewModel.DoRandomChange();
      //Controls.Add(b1);
      //b1.BringToFront();

      // 初始化定时器
      var updateTimer_ = new FormsTimer
      {
        Interval = 900
      };
      updateTimer_.Tick += (object? sender, EventArgs e) => viewModel.DoRandomChange();
      updateTimer_.Start();

    }
  }





  public partial class Form2 : Form
  {
    public Form2()
    {
      InitializeComponent();
    }
  }
}
