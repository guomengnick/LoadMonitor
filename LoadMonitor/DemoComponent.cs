using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LoadMonitor;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.WinForms;

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMonitor
{
  internal class DemoComponent : AutoUpdatePanel
  {
    public DemoComponent() { }

    private Single single_form_;
    public override Form GetDetailForm()
    {
      single_form_ = new Single();
      // 创建并配置要添加的 View 控件
      var view = new View()
      {
        Dock = DockStyle.Fill // 填充整个 Panel
      };
      single_form_.AddToPanel(view);
      return single_form_;
    }
  }


internal class DemoComponentQuadGrid : AutoUpdatePanel
{
  public DemoComponentQuadGrid() { }

  private QuadGrid quad_form_;
  public override Form GetDetailForm()
  {
    quad_form_ = new QuadGrid();

      // 初始化数据
      var data_ = new ObservableCollection<ObservableValue>
            {
                new ObservableValue(5),
                new ObservableValue(3),
                new ObservableValue(7),
                new ObservableValue(2),
                new ObservableValue(3),
                new ObservableValue(7),
                new ObservableValue(2),
                new ObservableValue(3),
                new ObservableValue(7),
                new ObservableValue(2),
                new ObservableValue(3),
                new ObservableValue(7),
                new ObservableValue(2),
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
    var view4 = new View()
    {
      Dock = DockStyle.Fill // 填充整个 Panel
    };

    quad_form_.AddToPanel(cartesianChart_, view2, view3, view4);
    return quad_form_;
  }
}





  internal class DemoComponentLeftOneRightTwo : AutoUpdatePanel
  {
    public DemoComponentLeftOneRightTwo() { }

    private LeftOneRightTwo form_3_;
    public override Form GetDetailForm()
    {
      form_3_ = new LeftOneRightTwo();

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
