﻿using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.VisualElements;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.VisualElements;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LoadMonitor.Components
{
  // 切割軸XYZ 馬達
  internal class CutMotor : PartBase
  {
    private Single single_form_ = new Single();
    public CutMotor(string name,
      Panel DetailChartPanel, double max_current, SKColor chart_color, MainForm owner, string image_path, string settingName) :
      base(name, max_current, DetailChartPanel, chart_color, owner, image_path, settingName) // 主轴最大负载值为 10A
    {
    }

    protected override Action<string, string> DetailFormUpdater => (leftText, rightText) =>
    {
      single_form_.Invoke(new Action(() => single_form_.UpdateText(leftText, rightText)));
    };

    //public abstract void ReinitializeChart()
    //{

    //}

    public override Form GetDetailForm()
    {
      // 创建并配置要添加的 AngularGauge 控件
      CartesianChart cartesianChart_ = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]
          {
            new LineSeries<ObservableValue>
            {
              Values = data_,
              Fill = new SolidColorPaint(base.fill_color_), // 填充颜色
              GeometrySize = 0, // 无点标记
              Stroke = new SolidColorPaint(base.line_color_, 1), // 线条颜色和粗细
              LineSmoothness = 0, // 无弧度     
            },
          },
        AnimationsSpeed = TimeSpan.Zero, // 设置动画持续时间为1秒
        EasingFunction = LiveChartsCore.EasingFunctions.ExponentialOut, // 设置缓动效果
        // 設置 X 軸
        XAxes = new Axis[]
        {
          new Axis
          {
            Labels = null, // 自動生成標籤
            MinLimit = 0, // 最小值（秒）
            MaxLimit = 60, // 最大值（秒）
            UnitWidth = 30,
            Labeler = value =>
            {
              if (value == 0)return $"60{Language.GetString("單位.秒")}";
              else if (value == 60)return "0";
              return ""; // 其他值不顯示
            },
            LabelsPaint = new SolidColorPaint(SKColors.Black)
            {
              SKTypeface = SKFontManager.Default.MatchCharacter('汉'), // 設定中文字體
                  
            },
            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
            {
              StrokeThickness = 1 // 分隔線的寬度
            },
            TextSize = 10,
            Padding = new LiveChartsCore.Drawing.Padding(50, 20, 50, 0) // 調整標籤與邊界的距離
          }
        },
        // 設置 Y 軸
        YAxes = new Axis[]
        {
          new Axis
          {
            MinLimit = 0, // 最小值（安培）
            MaxLimit = Math.Ceiling(base.MaxLoadingValue), // 最大值（安培）
            Labeler = value =>
            {
              if (value == Math.Ceiling(base.MaxLoadingValue))
              {
                return $"% {Language.GetString("單位.使用率")}";//base.MaxLoadingValue.ToString() + "A";
              }
              return "";
            }, // 格式化為 0 A, 1 A, 2 A
            TextSize= 12,
            Padding = new LiveChartsCore.Drawing.Padding(20, 26, 0, 42), // 調整標籤與邊界的距離
            LabelsPaint = new SolidColorPaint(SKColors.Black)
            {
              SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字體
            }, // 標籤顏色
            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray), // 分隔線顏色
          }
        },
        DrawMargin = new LiveChartsCore.Measure.Margin(20, 30, 5, 15),//設定 左、上、右、下 的邊界大小
        Title = new LabelVisual
        {
          Text = MainTitle,
          TextSize = 24,
          Paint = new SolidColorPaint(SKColors.Black)
          {
            SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字體
          }, // 標籤顏色
        },
      };
      single_form_.AddToPanel(cartesianChart_, base.DetailInfo, "");

      return single_form_;
    }



    protected override (string LeftText, string RightInfo) UpdateDetailData()
    {
      return base.GetText();
    }




  }
}
