﻿using CommunityToolkit.Mvvm.Input;
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
using Log = Serilog.Log;

namespace LoadMonitor
{
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
        Value = 0,
        Fill = new SolidColorPaint(0xf0302f29)
      };

      Series = GaugeGenerator.BuildAngularGaugeSections(
        new GaugeItem(50, s =>
        {
          s.Fill = new SolidColorPaint(0xff2bf211); // 藍色
          SetStyle(sectionsOuter, sectionsWidth, s);
        }),
        new GaugeItem(30, s =>
        {
          s.Fill = new SolidColorPaint(0xffffff05); // 黃色
          SetStyle(sectionsOuter, sectionsWidth, s);
        }),
        new GaugeItem(20, s =>
        {
          s.Fill = new SolidColorPaint(0xfff75273); // 紅色
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

      VisualElements = [angularTicksVisual_,
        Needle,
        // 添加居中文本
        new LabelVisual
        {
            //Text = text, // 要顯示的文本 TODO:FIX_BUG 不能基於Panel 像素, 位置會因為上層部件大小 跑掉
            //TextSize = 14, // 字體大小
            //Paint = new SolidColorPaint(SkiaSharp.SKColors.Black){
            //   SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字型
            //}, // 文本顏色
            //LocationUnit = LiveChartsCore.Measure.MeasureUnit.Pixels, // 使用像素單位
            
            //X = 100, // 水平居中
            //Y = 100  // 垂直居中
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

    public static void SetStyle(
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
        Title = new LabelVisual
        {
          Text = text,
          TextSize = 15,
          Paint = new SolidColorPaint(SkiaSharp.SKColors.Black)
          {
            SKTypeface = SKFontManager.Default.MatchCharacter('汉'), // 設定中文字型
            SKFontStyle = SKFontStyle.Bold,
          }, // 文本顏色
        },
        LegendTextPaint = textPaint,
        TooltipTextPaint = textPaint,
        Series = viewModel_.Series,
        VisualElements = viewModel_.VisualElements,
        InitialRotation = -225,
        MaxAngle = 270,
        MinValue = 0,
        MaxValue = 100,
        AnimationsSpeed = TimeSpan.Zero, // 设置动画持续时间为1秒
        // out of livecharts properties...
        //Location = new System.Drawing.Point(0, 0),
        Size = new System.Drawing.Size(10, 10),
        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
      };
      //pie_chart_.VisualElements.
      Controls.Add(pie_chart_);

      var panelWidth = pie_chart_.Width; // 獲取 Panel 的寬度
      var panelHeight = pie_chart_.Height; // 獲取 Panel 的高度
      // 初始化定时器
      //var updateTimer_ = new FormsTimer
      //{
      //  Interval = 900
      //};
      //updateTimer_.Tick += (object? sender, EventArgs e) => viewModel_.DoRandomChange(_random.Next(15, 25));
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

    public void SetGaugeMaxValue(int max_index)
    {

      pie_chart_.MaxValue = max_index;
      viewModel_.angularTicksVisual_.Labeler = value =>
      {

        const double epsilon = 1e-9; // 定義容差
        if (max_index <= 1)
        {
          // 使用 Math.Abs 判斷是否接近 0.2 的倍數
          bool isCloseToStep = Math.Abs(value % 0.2) < epsilon || Math.Abs(value % 0.2 - 0.2) < epsilon;

          Log.Information($"value {value}, isCloseToStep: {isCloseToStep}, return: {(isCloseToStep ? value.ToString("0.0") : "*")}");

          return isCloseToStep ? value.ToString("0.0") : "";
        }

        // 如果是整數刻度，則顯示值
        //return value % 1 == 0 && value >= 0 && value <= max_index
        //    ? (value * 10000).ToString("0")
        //    : ""; // 將值顯示為 10000 的倍數

        return value % 1 == 0 && value >= 0 && value <= max_index ? value.ToString() : "";
      };


      var sectionsOuter = -5;//顏色固定的寬度
      var sectionsWidth = 11;

      // 計算每個區域的比例值
      viewModel_.Series = GaugeGenerator.BuildAngularGaugeSections(
          new GaugeItem(max_index * 0.6, s => // 60% 的範圍 (3/5 * max_index)
          {
            s.Fill = new SolidColorPaint(0xff22ff05); // 綠色
            ViewModel.SetStyle(sectionsOuter, sectionsWidth, s);
          }),
          new GaugeItem(max_index * 0.2, s => // 20% 的範圍 (1/5 * max_index)
          {
            s.Fill = new SolidColorPaint(0xfffbff05); // 黃色
            ViewModel.SetStyle(sectionsOuter, sectionsWidth, s);
          }),
          new GaugeItem(max_index * 0.2, s => // 20% 的範圍 (1/5 * max)
          {
            s.Fill = new SolidColorPaint(0xffff0505); // 紅色
            ViewModel.SetStyle(sectionsOuter, sectionsWidth, s);
          })
      );

      pie_chart_.Series = viewModel_.Series;

    }

  }

}
