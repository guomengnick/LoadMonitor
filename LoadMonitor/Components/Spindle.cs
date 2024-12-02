using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LoadMonitor.TEST;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.VisualElements;

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMonitor.Components
{
  internal class Spindle : PartBase
  {
    private string motor_name_;

    public Spindle(string motor_name)
    {
      motor_name_ = motor_name;
      TEST.TEST.Add60EmptyData(data_);
    }

    private UserControl RPM()
    {
      var rpm_chart = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]{new LineSeries<ObservableValue>{
            Values = data_,
            Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
            GeometrySize = 0, // 无点标记
            Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
            LineSmoothness = 0, // 无弧度
          },
        },
      };

      rpm_chart.Title = new LabelVisual
      {
        Text = "RPM (Radius per minute)",
        TextSize = 16,
        Paint = new SolidColorPaint(SKColors.Black),
      };
      return rpm_chart;
    }


    private UserControl Voltage()
    {
      return new View()
      {
        Dock = DockStyle.Fill,
      };
      var rpm_chart = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]{new LineSeries<ObservableValue>{
            Values = data_,
            Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
            GeometrySize = 0, // 无点标记
            Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
            LineSmoothness = 0, // 无弧度
          },
        },
      };

      rpm_chart.Title = new LabelVisual
      {
        Text = "Voltage (V)",
        TextSize = 16,
        Paint = new SolidColorPaint(SKColors.Black),
      };
      return rpm_chart;
    }


    private UserControl MotorTemperature()
    {
      var rpm_chart = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]{new LineSeries<ObservableValue>{
            Values = data_,
            Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
            GeometrySize = 0, // 无点标记
            Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
            LineSmoothness = 0, // 无弧度
          },
        },
      };

      rpm_chart.Title = new LabelVisual
      {
        Text = "Motor Temperature (° C)",
        TextSize = 16,
        Paint = new SolidColorPaint(SKColors.Black),
      };
      return rpm_chart;
    }



    private UserControl MotorLoading()
    {
      return new View()
      {
        Dock = DockStyle.Fill,
      };
      var rpm_chart = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]{new LineSeries<ObservableValue>{
            Values = data_,
            Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
            GeometrySize = 0, // 无点标记
            Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
            LineSmoothness = 0, // 无弧度
          },
        },
      };

      rpm_chart.Title = new LabelVisual
      {
        Text = "Motor Loading (%)",
        TextSize = 16,
        Paint = new SolidColorPaint(SKColors.Black),
      };
      return rpm_chart;
    }

    public override Form GetDetailForm()
    {
      QuadGrid quad_grid_form = new QuadGrid();

      // 创建并配置要添加的 View 控件
      quad_grid_form.AddToPanel(RPM(), Voltage(), MotorTemperature(), MotorLoading());
      return quad_grid_form;
    }
  }
}
