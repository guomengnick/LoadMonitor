using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoadMonitor.Components
{
    internal class Overview : PartBase
  {

    private DateTime startTime;
    public Overview(string mainTitle, string subTitle, string detailInfo, Panel DetailChartPanel) : 
      base(mainTitle, subTitle, detailInfo, 5.0, DetailChartPanel)
    {
      startTime = DateTime.Now;
    }


    public override (string Summary, string DetailInfo) GetText()
    {
      double latestValue = data_.Last().Value ?? 0.0; // 获取最新数据
      double loading = CalculateLoading(latestValue); // 计算负载百分比

      string summary = $"{loading:F1} %";

      // 获取当前日期和时间
      DateTime now = DateTime.Now;
      string dateTimeString = now.ToString("yyyy/MM/dd HH:mm:ss");

      // 累計運行時間
      TimeSpan operationTime = now - startTime; // 使用初始化的 startTime 計算時間差

      // 格式化运作时间
      string operationTimeFormatted = $"運作時間: {operationTime.Hours}:{operationTime.Minutes:D2}:{operationTime.Seconds:D2}:{operationTime.Milliseconds:D3}";

      // 計算瞬時功率 (假設公式為: 電流 x 電壓)
      double voltage = 24.0; // 假設電壓值
      double power = latestValue * voltage; // 功率 (W)

      // 平均每日功率 (kW·h)
      var random = new Random();
      double averageDailyPower = random.NextDouble() * 1000; // 平均每日功率

      // 格式化詳細信息
      string detailInfo = $@"{dateTimeString}
{operationTimeFormatted}
整機附載: {loading:F1} %
總電流: {latestValue:F3} A
";

      var right_text = $@"瞬時功率: {power:F1} W



平均日功率: {averageDailyPower:F1} kWh";

      base.DetailInfo = detailInfo;
      current_watt_.UpdateValue(power);
      daily_average_watt_.UpdateValue(averageDailyPower);
      form_3.UpdateText(detailInfo, right_text);

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
        Text = "整機",
        TextSize = 20,
        Paint = new SolidColorPaint(SKColors.Black)
        {
          SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字體
        }, // 標籤顏色
      };
      return overview_chart;
    }

    private AngularGauge current_watt_;
    private UserControl AverageWattPerWeekChart()
    {
      current_watt_ = new AngularGauge("當前功率(kWh)")//TODO 給單位
      {
        Dock = DockStyle.Fill,
      };
      return current_watt_;
    }

    private AngularGauge daily_average_watt_;

    private UserControl AverageWattPerMonthChart()
    {
      daily_average_watt_ = new AngularGauge("日功率(kWh)") {//TODO 給單位

        Dock = DockStyle.Fill,
      };
      return daily_average_watt_;
    }

    private LeftOneRightTwo form_3;

    public override Form GetDetailForm()
    {
      form_3 = new LeftOneRightTwo();
      var left_text = base.DetailInfo;
      var right_text = "";//右邊給空

      form_3.AddToPanel(OverviewLoadingChart(), AverageWattPerWeekChart(), 
          AverageWattPerMonthChart(), left_text, right_text);
      return form_3;
    }

  }
}
