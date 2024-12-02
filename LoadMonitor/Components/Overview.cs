using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.VisualElements;

using LoadMonitor.TEST;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMonitor.Components
{
  internal class Overview : PartBase
  {
    public Overview()
    {
      TEST.TEST.Add60EmptyData(data_);
    }

    private UserControl OverviewLoadingChart()
    {
      var overview_chart = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]{new LineSeries<ObservableValue>{
            Values = data_, Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
            GeometrySize = 0, // 无点标记
            Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
            LineSmoothness = 0, // 无弧度
          },
        },
      };
      overview_chart.Title = new LabelVisual
      {
        Text = "Overview",
        TextSize = 20,
        Paint = new SolidColorPaint(SKColors.Black),
      };
      return overview_chart;
    }


    private UserControl AverageWattPerWeekChart()
    {
      var overview_chart = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]{new LineSeries<ObservableValue>{
            Values = data_, Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
            GeometrySize = 0, // 无点标记
            Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
            LineSmoothness = 0, // 无弧度
          },
        },
      };
      overview_chart.Title = new LabelVisual
      {
        Text = "kWh / week",
        TextSize = 16,
        Paint = new SolidColorPaint(SKColors.Black),
      };
      var guage = new View()
      {

        Dock = DockStyle.Fill,
      }; ;
      return guage;
      return overview_chart;
    }

    private UserControl AverageWattPerMonthChart()
    {

      var guage = new View() {

        Dock = DockStyle.Fill,
      };
      return guage;
      var overview_chart = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]{new LineSeries<ObservableValue>{
            Values = data_, Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
            GeometrySize = 0, // 无点标记
            Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
            LineSmoothness = 0, // 无弧度
          },
        },
      };
      overview_chart.Title = new LabelVisual
      {
        Text = "kWh / month",
        TextSize = 16,
        Paint = new SolidColorPaint(SKColors.Black),
      };
      return overview_chart;
    }


    public override Form GetDetailForm()
    {

      LeftOneRightTwo form_3 = new LeftOneRightTwo();
      form_3.AddToPanel(OverviewLoadingChart(), AverageWattPerWeekChart(), AverageWattPerMonthChart());
      return form_3;
    }

  }
}
