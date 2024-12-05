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

    private TimeSpan operationTime; // 模拟运作时间的属性
    public Overview() : base(1.5)
    {
      TEST.TEST.Add60EmptyData(data_);
      operationTime = new TimeSpan(); // 初始化运作时间
    }

    public override (string Summary, string DetailInfo) GetText()
    {
      double latestValue = data_.Last().Value ?? 0.0; // 获取最新数据
      double loading = CalculateLoading(latestValue); // 计算负载百分比

      string summary = $"{loading:F1} %";
      // 格式化运作时间为 0:01:06:28 的形式
      string operation_time = $"運作時間: {operationTime.Hours}:{operationTime.Minutes:D2}:{operationTime.Seconds:D2}:{operationTime.Milliseconds:D3}";
      string detailInfo = $"當前附載: {loading:F1} % \n整機電流: {latestValue} A\n{operation_time}";
      return (summary, detailInfo);
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
      var guage = new AngularGauge()
      {

        Dock = DockStyle.Fill,
      }; ;
      return guage;
      return overview_chart;
    }

    private UserControl AverageWattPerMonthChart()
    {

      var guage = new AngularGauge() {

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
