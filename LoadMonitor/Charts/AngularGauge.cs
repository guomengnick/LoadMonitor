using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
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
using System.Drawing;

using FormsTimer = System.Windows.Forms.Timer;
using LiveChartsCore.Drawing;

namespace LoadMonitor
{
  // 定義僅在本文件可見的 ViewModel(只給下方的類用)
  public partial class ViewModel
  {

    private readonly Random _random = new();
    public IEnumerable<ISeries> Series { get; set; }
    public IEnumerable<VisualElement<SkiaSharpDrawingContext>> VisualElements { get; set; }
    public NeedleVisual Needle { get; set; }
    public AngularTicksVisual angularTicksVisual_ { get; set; }

    public ViewModel(string text = "")
    {
      var sectionsOuter = 0;
      var sectionsWidth = 7;

      Needle = new NeedleVisual
      {
        Value = 45
      };

      Series = GaugeGenerator.BuildAngularGaugeSections(
        new GaugeItem(50, s =>
        {
          s.Fill = new SolidColorPaint(0xf012ff75); // 藍色
          SetStyle(sectionsOuter, sectionsWidth, s);
        }),
        new GaugeItem(30, s =>
        {
          s.Fill = new SolidColorPaint(0xf0fafa20); // 紅色
          SetStyle(sectionsOuter, sectionsWidth, s);
        }),
        new GaugeItem(20, s =>
        {
          s.Fill = new SolidColorPaint(0xf0f75273); // 紅色
          SetStyle(sectionsOuter, sectionsWidth, s);
        }));

      angularTicksVisual_ = new AngularTicksVisual
      {
        Labeler = value => value.ToString("N0"), // "N0" 格式為帶千分位的整數
        //Labeler = value =>
        //{
        //  // 顯示整數刻度 0, 1, 2, 3
        //  return value % 1 == 0 && value >= 0 && value <= 3 ? value.ToString() : "";
        //},
        LabelsSize = 12,//儀表上的文字
        LabelsOuterOffset = 30,//儀表上的文字 會往內多少
        OuterOffset = 18,//一整個刻度會往內多少
        TicksLength = 10//刻度的長度就是 那個'''|''''|''''
      };

      VisualElements =[angularTicksVisual_,
        Needle,
        // 添加居中文本
        new LabelVisual
        {
            Text = text, // 要顯示的文本 TODO:FIX_BUG 不能基於Panel 像素, 位置會因為上層部件大小 跑掉
            TextSize = 14, // 字體大小
            Paint = new SolidColorPaint(SkiaSharp.SKColors.Black){
               SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字型
            }, // 文本顏色
            LocationUnit = LiveChartsCore.Measure.MeasureUnit.Pixels, // 使用像素單位
            X = 125, // 水平居中
            Y = 190  // 垂直居中
            //LocationUnit = LiveChartsCore.Measure.MeasureUnit.ChartValues, // 使用相對單位
            //X = 5,  // 水平相對位置 (0.5 表示中心)
            //Y = 9   // 垂直相對位置 (0.9 表示靠近底部)
        }
      ];
    }


    [RelayCommand]
    public void DoRandomChange(double value)
    {
      // modifying the Value property B and animates the chart automatically
      Needle.Value = value;
      // 手動刷新圖表
    }

    private static void SetStyle(
        double sectionsOuter, double sectionsWidth, PieSeries<ObservableValue> series)
    {
      series.OuterRadiusOffset = sectionsOuter;
      series.MaxRadialColumnWidth = sectionsWidth;
    }
  }


  public partial class AngularGauge : UserControl
  {
    public ViewModel viewModel_ = new ViewModel();
    public LiveChartsCore.SkiaSharpView.WinForms.PieChart pie_chart_;

    public ViewModel GetAngularGauge() { return viewModel_; }

    public AngularGauge(string text = "")
    {

      viewModel_ = new ViewModel(text);
      Size = new System.Drawing.Size(50, 50);

      // 需要這樣設定才能顯示中文
      var textPaint = new SolidColorPaint(SKColors.Black)
      {
        SKTypeface = SKFontManager.Default.MatchCharacter('汉')
      };

      pie_chart_ = new LiveChartsCore.SkiaSharpView.WinForms.PieChart
      {
        LegendTextPaint = textPaint,
        TooltipTextPaint = textPaint,
        Series = viewModel_.Series,
        VisualElements = viewModel_.VisualElements,
        InitialRotation = -225,
        MaxAngle = 270,
        MinValue = 0,
        MaxValue = 100,
        //AnimationsSpeed = TimeSpan.Zero,
        // out of livecharts properties...
        Location = new System.Drawing.Point(0, 0),
        Size = new System.Drawing.Size(10, 10),
        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
      };

      Controls.Add(pie_chart_);

      // 初始化定时器
      var updateTimer_ = new FormsTimer
      {
        Interval = 900
      };
      updateTimer_.Tick += (object? sender, EventArgs e) => viewModel_.DoRandomChange(_random.Next(15, 25));
      //updateTimer_.Start();
    }

    private readonly Random _random = new();

    public void UpdateValue(double value)
    {
      if (this.InvokeRequired)
      {
        this.Invoke((Action)(() => viewModel_.DoRandomChange(value))); // 切回主線程執行
      }
      else
      {
        viewModel_.DoRandomChange(value);
      }
    }

  }

}
