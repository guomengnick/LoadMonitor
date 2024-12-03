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
        var newValue = random.Next(0, 100); // 假设范围是 0~100
        data.Add(new ObservableValue(newValue));
      }

    }


    public static void Add60EmptyData(ObservableCollection<ObservableValue> data)
    {
      Random random = new Random(); // 在循环外创建 Random 实例
      for (int i = 0; i < 59; i++)
      {
        data.Add(new ObservableValue(0));
      }
    }
  }



  internal class DemoComponent : PartBase
  {
    public DemoComponent()
    {
      data_ = new System.Collections.ObjectModel.ObservableCollection<ObservableValue> ();
      TEST.AddData(data_);
    }
    public override Form GetDetailForm()
    {
      var single_form_ = new Single();
      // 创建并配置要添加的 View 控件

      CartesianChart cartesianChart_ = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]
                {
                    new LineSeries<ObservableValue>
                    {
                        Values = data_,
                        Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
                        GeometrySize = 0, // 无点标记
                        Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
                        LineSmoothness = 0, // 无弧度
                    },
                },
      };
      single_form_.AddToPanel(cartesianChart_);
      return single_form_;
    }
  }


  internal class DemoCartesianComponent : PartBase
  {
    public DemoCartesianComponent() { }

    public override Form GetDetailForm()
    {
      var single_form_ = new Single();
      // 创建并配置要添加的 View 控件

      // 初始化数据
      //var data_ = new ObservableCollection<ObservableValue>
      //      {
      //          new ObservableValue(5),
      //          new ObservableValue(3),
      //          new ObservableValue(2),
      //          new ObservableValue(3),
      //          new ObservableValue(4),
      //          new ObservableValue(3),
      //          new ObservableValue(9),
      //          new ObservableValue(2),
      //          new ObservableValue(3),
      //          new ObservableValue(1),
      //      };

      // 初始化图表
      //var cartesianChart_ = new CartesianChart
      //{
      //  Dock = DockStyle.Fill, // 填满 Panel
      //  Series = new ISeries[]
      //          {
      //              new LineSeries<ObservableValue>
      //              {
      //                  Values = data_,
      //                  Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
      //                  GeometrySize = 0, // 无点标记
      //                  Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
      //                  LineSmoothness = 0, // 无弧度
      //                  Name = "X1", // 设定系列的图例名称
      //              },
      //          },
      //  DrawMargin = null, // 移除绘图区域边距
      //  LegendPosition = LiveChartsCore.Measure.LegendPosition.Top, // 移除图例
      //};
      //cartesianChart_.XAxes = new[] { new Axis { IsVisible = true, SeparatorsPaint = null } };
      //cartesianChart_.YAxes = new[] { new Axis { IsVisible = true, SeparatorsPaint = null } };
      //single_form_.AddToPanel(cartesianChart_);
      return single_form_;
    }
  }

  internal class DemoComponentQuadGrid : PartBase
  {
    public DemoComponentQuadGrid()
    {
      data_ = new System.Collections.ObjectModel.ObservableCollection<ObservableValue> ();
      TEST.AddData(data_);
    }

    public override Form GetDetailForm()
    {
      var quad_form_ = new QuadGrid();

      // 初始化图表
      var cartesianChart_ = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]
                {
                    new LineSeries<ObservableValue>
                    {
                        Values = data_,
                        Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
                        GeometrySize = 0, // 无点标记
                        Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
                        LineSmoothness = 0, // 无弧度
                    },
                },
      };

      // 创建并配置要添加的 View 控件
      var view1 = new View()
      {
        Dock = DockStyle.Fill // 填充整个 Panel
      };
      var view2 = new View()
      {
        Dock = DockStyle.Fill // 填充整个 Panel
      };
      var view3 = new View()
      {
        Dock = DockStyle.Fill // 填充整个 Panel
      };
      var view4 = new View()
      {
        Dock = DockStyle.Fill // 填充整个 Panel
      };

      quad_form_.AddToPanel(cartesianChart_, view2, view3, view4);
      return quad_form_;
    }
  }





  internal class DemoComponentLeftOneRightTwo : PartBase
  {
    public DemoComponentLeftOneRightTwo()
    {
      data_ = new System.Collections.ObjectModel.ObservableCollection<ObservableValue> ();
      TEST.AddData(data_);
    }

    public override Form GetDetailForm()
    {
      var form_3_ = new LeftOneRightTwo();

      // 初始化数据
      var data_ = new ObservableCollection<ObservableValue>
            {
                new ObservableValue(5),
                new ObservableValue(3),
                new ObservableValue(2),
                new ObservableValue(3),
                new ObservableValue(4),
                new ObservableValue(3),
                new ObservableValue(9),
                new ObservableValue(2),
                new ObservableValue(3),
                new ObservableValue(1),
            };

      // 初始化图表
      var cartesianChart_ = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]
                {
                    new LineSeries<ObservableValue>
                    {
                        Values = data_,
                        Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
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

      // 创建并配置要添加的 View 控件
      var view1 = new View()
      {
        Dock = DockStyle.Fill // 填充整个 Panel
      };
      var view2 = new View()
      {
        Dock = DockStyle.Fill // 填充整个 Panel
      };
      var view3 = new View()
      {
        Dock = DockStyle.Fill // 填充整个 Panel
      };

      form_3_.AddToPanel(cartesianChart_, view2, view3);
      return form_3_;
    }
  }














}
