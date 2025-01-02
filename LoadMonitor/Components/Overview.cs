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
using static LoadMonitor.Data.HistoryData;

using Log = Serilog.Log;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveCharts.Wpf.Charts.Base;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO.Pipes;

namespace LoadMonitor.Components
{
    internal class Overview : PartBase
  {
    private const string PipeName = "LoadMonitorPipe"; // 命名管道名稱，用於識別管道通訊
    private static int latest_warning_count_; // 最新的警告數量
    private Dictionary<int, PartBase> components_; // 保存所有組件的集合
    private static CancellationTokenSource cts_ = new CancellationTokenSource(); // 控制後台伺服器的執行取消
    private Data.DailyValueCalculator power_calculator_;
    private Data.DailyValueCalculator usage_calculator_;

    private DateTime startTime;
    public Overview(string name,
      Panel DetailChartPanel, double max_current, SKColor color, MainForm owner, string image_path, string settingName) :
      base(name, max_current, DetailChartPanel, color, owner, image_path, settingName)
    {
      startTime = DateTime.Now;

      Task.Run(() => StartPipeServer(cts_.Token)); // 啟動後台任務處理管道通訊

      power_calculator_ = new Data.DailyValueCalculator();
      usage_calculator_ = new Data.DailyValueCalculator();
    }
    private LeftOneRightTwo form_3;

    protected override Action<string, string> DetailFormUpdater => (leftText, rightText) =>
    {
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

      var warning_count = 0; // 本次更新的警告計數

      // 使用 LINQ 簡化迴圈計算，避免重複迴圈邏輯
      warning_count = components_.Values.Count(component =>
      {
        if (component.IsExceedingMax())
        {
          // 記錄類名稱和超出警示的訊息
          Log.Information($"類名稱: {component.GetType().Name} 超出警示範圍");
          return true; // 符合條件
        }
        return false; // 不符合條件
      });

      // 使用 Interlocked.Exchange 保證執行緒安全地更新最新的警告數量
      Interlocked.Exchange(ref latest_warning_count_, warning_count);
    }


    // 開啟 NamedPipeServer，等待接收端連接並發送最新警告數據
    private static void StartPipeServer(CancellationToken token)
    {
      while (!token.IsCancellationRequested) // 檢查取消令牌，確保可以停止後台任務
      {
        try
        {
          using (var pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.Out))
          {
            Log.Information("等待客戶端連接...");
            pipeServer.WaitForConnection(); // 阻塞等待客戶端連接

            // 將最新的警告數量轉換為字串，準備發送
            string message = latest_warning_count_.ToString();
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            Log.Information($"寫入數據到LoadMonitorPipe: {message}");

            pipeServer.Write(buffer, 0, buffer.Length); // 發送數據給客戶端
            pipeServer.Flush(); // 確保資料寫入管道
            Log.Information($"已發送最新警告數量: {message}");
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"管道錯誤: {ex.Message}");
        }
      }
    }

    // 釋放資源，停止 NamedPipeServer 執行緒
    public void Dispose()
    {
      cts_.Cancel(); // 取消後台任務
    }




    private UserControl OverviewLoadingChart()
    {
      return new CartesianChart
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
      double averageDailyPower = power_calculator_.AddValue(power, 1); // 平均每日功率
      double average_daily_usage = usage_calculator_.AddValue(power, 1); // 平均每日功率

      // 格式化詳細信息
      string detailInfo = $@"{dateTimeString}
{operationTimeFormatted}
{Language.GetString("整機使用率")}: {loading:F1} %
{Language.GetString("總電流")}: {latestValue:F3} A
{Language.GetString("瞬時功率")}: {power:F1} kW
";


      base.DetailInfo = detailInfo;
      current_watt_.UpdateValue(power);
      daily_average_watt_.UpdateValue(averageDailyPower);

      var right_text = $@"{Language.GetString("日整機使用率")}: {average_daily_usage:F1} %
{Language.GetString("日功率")}: {averageDailyPower:F1} kWh

";
      return (detailInfo, right_text);
    }

  }
}
