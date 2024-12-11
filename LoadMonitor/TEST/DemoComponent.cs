using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.WinForms;

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LoadMonitor.TEST
{
  // 工具类，包含 AddTESTData 方法
  public static class TEST
  {
    public static void AddData(ObservableCollection<ObservableValue> data)
    {
      Random random = new Random(); // 在循环外创建 Random 实例
      for (int i = 0; i < 59; i++)
      {
        var newValue = random.Next(0, 3); // 假设范围是 0~100
        data.Add(new ObservableValue(newValue));
      }

    }


    public static void Add60EmptyData(ObservableCollection<ObservableValue> data)
    {
      Random random = new Random(); // 在循环外创建 Random 实例
      for (int i = 0; i < 59; i++)
      {
        data.Add(new ObservableValue(0.1));
      }
    }



    public static CartesianChart CreateThumbnailTEST()
    {
      ObservableCollection<ObservableValue> data = new ObservableCollection<ObservableValue>();
      AddData(data);
    // 初始化图表
    CartesianChart cartesianChart_ = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]
                {
                    new LineSeries<ObservableValue>
                    {
                        Values = data,
                       Fill = new SolidColorPaint(0x556eddff), // 填充颜色
                        GeometrySize = 0, // 无点标记
                        Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
                        LineSmoothness = 0, // 无弧度
                    },
                },
        XAxes = new[] { new Axis { IsVisible = false, SeparatorsPaint = null } }, // 隐藏 X 轴
        YAxes = new[] { new Axis { IsVisible = false, SeparatorsPaint = null } }, // 隐藏 Y 轴
        DrawMargin = null, // 移除绘图区域边距
        Padding = new Padding(0), // 移除内部边距
        Margin = new Padding(0), // 移除外部边距
        LegendPosition = LiveChartsCore.Measure.LegendPosition.Hidden, // 移除图例
        Legend = null,
      };
      return cartesianChart_;
    }


  }
}
