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
using static LoadMonitor.HistoryData;

using Log = Serilog.Log;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveCharts.Wpf.Charts.Base;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LoadMonitor.Components
{
  internal class Overview : PartBase
  {
    private Dictionary<int, PartBase> components_; // 用于保存组件数据

    private DateTime startTime;
    public Overview(string name,
      Panel DetailChartPanel, double max_current, SKColor color) :
      base(name, max_current, DetailChartPanel, color)
    {
      startTime = DateTime.Now;
    }
    private LeftOneRightTwo form_3;

    protected override Action<string, string> DetailFormUpdater => (leftText, rightText) =>
    {
      //if (!form_3.IsHandleCreated)
      //{
      //  form_3.Show(); // 強制創建 Handle
      //}
      form_3.Invoke(new Action(() => form_3.UpdateText(leftText, rightText)));
    };

    public void AddAllParts(Dictionary<int, PartBase> components)
    {
      components_ = components;
    }

    // 获取所有部件的负载信息
    public string DisplayLoadSummary()
    {
      StringBuilder summaryBuilder = new StringBuilder();

      // 添加表头
      summaryBuilder.AppendLine(string.Format("{0,-8} | {1,10} | {2,10}", "部件名稱", "1Hr 負載", "6Hr 負載"));
      summaryBuilder.AppendLine(new string('-', 35));

      // 遍历组件获取负载信息
      foreach (var component in components_.Values)
      {
        double oneHourAverage = component.history_data_.GetAverage(TimeUnit.OneHour);
        double sixHourAverage = component.history_data_.GetAverage(TimeUnit.SixHours);

        // 添加到字符串构建器
        summaryBuilder.AppendLine(string.Format("{0,-8} | {1,10:F2}% | {2,10:F2}%",
                                                component.MainTitle, oneHourAverage, sixHourAverage));
      }

      string result = summaryBuilder.ToString();

      // 同时记录日志
      Log.Information("\n" + result);

      return result;

    }

    public override void Update(double motor_current)
    {
      base.Update(motor_current);//要計算整機的電流
    }

    private UserControl OverviewLoadingChart()
    {
      var chart =  new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]{new LineSeries<ObservableValue>{
            Values = data_,Fill = new SolidColorPaint(0x556eddff), // 填充颜色
            GeometrySize = 0, // 无点标记
            Stroke = new SolidColorPaint(0xff3a7ec7, 1), // 线条颜色和粗细
            LineSmoothness = 0, // 无弧度
          },
        },
        // 設置 X 軸
        XAxes = new Axis[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = 60,
                UnitWidth = 2,
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
            MaxLimit = base.MaxLoadingValue, // 最大值（安培）
            UnitWidth = 2,
            Labeler = value =>
            {
              if (value == base.MaxLoadingValue)
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
            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) // 分隔線顏色
          }
        },
        DrawMargin = new LiveChartsCore.Measure.Margin(20, 30, 5, 15),//設定 左、上、右、下 的邊界大小
        Title = new LabelVisual
        {
          Text = $"{Language.GetString("整機")}",
          TextSize = 24,
          Paint = new SolidColorPaint(SKColors.Black)
          {
            SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字體
          }, // 標籤顏色
        }
      };

      // 创建容器 Panel
      var container = new Panel
      {
        Dock = DockStyle.Fill
      };

      // 添加 Chart 到容器中
      container.Controls.Add(chart);

      // 创建上下左右的滑动条
      var topBar = CreateTrackBar(Orientation.Horizontal, 50, 0);    // 顶部
      var bottomBar = CreateTrackBar(Orientation.Horizontal, 50, 1); // 底部
      var leftBar = CreateTrackBar(Orientation.Vertical, 50, 2);     // 左侧
      var rightBar = CreateTrackBar(Orientation.Vertical, 50, 3);    // 右侧

      // 添加事件，用于实时调整 Chart 的 DrawMargin
      void UpdateDrawMargin()
      {
        chart.DrawMargin = new LiveChartsCore.Measure.Margin(
            leftBar.Value,  // 左边距
            topBar.Value,   // 上边距
            rightBar.Value, // 右边距
            bottomBar.Value // 下边距
        );
      }

      topBar.ValueChanged += (s, e) => UpdateDrawMargin();
      bottomBar.ValueChanged += (s, e) => UpdateDrawMargin();
      leftBar.ValueChanged += (s, e) => UpdateDrawMargin();
      rightBar.ValueChanged += (s, e) => UpdateDrawMargin();

      // 添加滑动条到容器（覆盖在 Chart 上）
      container.Controls.Add(topBar);
      container.Controls.Add(bottomBar);
      container.Controls.Add(leftBar);
      container.Controls.Add(rightBar);

      return chart;
    }



  // 创建 TrackBar
  private System.Windows.Forms.TrackBar CreateTrackBar(Orientation orientation, int initialValue, int position)
  {
    var trackBar = new System.Windows.Forms.TrackBar
    {
      Orientation = orientation,
      Minimum = 0,
      Maximum = 100,
      Value = initialValue,
      TickFrequency = 10,
      Size = orientation == Orientation.Horizontal
            ? new Size(150, 20) // 水平滑动条大小
            : new Size(20, 150), // 垂直滑动条大小
      Anchor = position == 0
            ? AnchorStyles.Top
            : position == 1
                ? AnchorStyles.Bottom
                : position == 2
                    ? AnchorStyles.Left
                    : AnchorStyles.Right
    };

    // 设置位置
    if (position == 0) trackBar.Location = new Point(20, 0); // 顶部
    if (position == 1) trackBar.Location = new Point(20, 300); // 底部
    if (position == 2) trackBar.Location = new Point(0, 20); // 左侧
    if (position == 3) trackBar.Location = new Point(380, 20); // 右侧

    return trackBar;
  }


  private AngularGauge current_watt_;
    private UserControl AverageWattPerWeekChart()
    {
      current_watt_ = new AngularGauge($"{Language.GetString("當前功率")}(kWh)") // TODO: 給單位
      {
        Dock = DockStyle.Fill,
      };
      current_watt_.SetGaugeMaxValue(3);
      return current_watt_;
    }


    private AngularGauge daily_average_watt_;

    private UserControl AverageWattPerMonthChart()
    {
      daily_average_watt_ = new AngularGauge($"{Language.GetString("日功率")}(kWh)")
      {

        Dock = DockStyle.Fill,
      };
      daily_average_watt_.SetGaugeMaxValue(3);
      return daily_average_watt_;
    }


    public override Form GetDetailForm()
    {
      form_3 = new LeftOneRightTwo();
      var left_text = base.DetailInfo;
      var right_text = "";//右邊給空

      form_3.AddToPanel(OverviewLoadingChart(), AverageWattPerWeekChart(),
          AverageWattPerMonthChart(), left_text, right_text);
      return form_3;
    }



    protected override (string LeftText, string RightInfo) UpdateDetailData()
    {
      double latestValue = data_.Last().Value ?? 0.0; // 获取最新数据
      double loading = CalculateLoading(latestValue); // 计算负载百分比

      string summary = $"{loading:F1} %";

      DateTime now = DateTime.Now;// 获取当前日期和时间
      string dateTimeString = now.ToString("yyyy/MM/dd HH:mm:ss");

      TimeSpan operationTime = now - startTime;// 累計運行時間

      // 格式化运作时间
      string operationTimeFormatted = 
        $"{Language.GetString("運作時間")}: {operationTime.Hours}:{operationTime.Minutes:D2}:{operationTime.Seconds:D2}:{operationTime.Milliseconds:D3}";

      // 計算瞬時功率 (假設公式為: 電流 x 電壓)
      double voltage = 220.0; // 假設電壓值
      double power = latestValue * voltage / 1000; // 功率 (kW)

      // 平均每日功率 (kW·h)
      var random = new Random();
      double averageDailyPower = random.NextDouble() * 0.1 + power; // 平均每日功率

      // 格式化詳細信息
      string detailInfo = $@"{dateTimeString}
{operationTimeFormatted}
{Language.GetString("整機使用率")}: {loading:F1} %
{Language.GetString("總電流")}: {latestValue:F3} A
{Language.GetString("瞬時功率")}: {power:F1} kW
{Language.GetString("日功率")}: {averageDailyPower:F1} kWh
";


      base.DetailInfo = detailInfo;
      current_watt_.UpdateValue(power);
      daily_average_watt_.UpdateValue(averageDailyPower);

      var right_text = "";//DisplayLoadSummary();

      return (detailInfo, right_text);
    }

  }
}
