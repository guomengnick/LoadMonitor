using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.VisualElements;

using SkiaSharp;
using System;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IniParser;
using IniParser.Model;
using System.Collections.ObjectModel;
using System.Timers;



using ThreadTimer = System.Timers.Timer;
using Log = Serilog.Log;
using static SkiaSharp.HarfBuzz.SKShaper;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts.Wpf.Charts.Base;


namespace LoadMonitor.Components
{
  internal class Spindle : PartBase
  {
    private string ini_path_ = "spindle_info.ini";
    protected ObservableCollection<ObservableValue> motor_temperature_data_ = new ObservableCollection<ObservableValue>();

    protected AngularGauge power_;
    protected AngularGauge rpm_;
    private ThreadTimer timer_ = new ThreadTimer(3000);
    public Spindle(string name, 
      Panel DetailChartPanel, double max_current, SKColor chart_color) :
      base(name, max_current, DetailChartPanel, chart_color) // 主轴最大负载值为 10A
    {
      TEST.TEST.Add60EmptyData(motor_temperature_data_);
      

      rpm_ = new AngularGauge("x10000rpm")
      {
        Dock = DockStyle.Fill,
      };

      power_ = new AngularGauge("功率(kW)")
      {
        Dock = DockStyle.Fill,
      };

      timer_.Elapsed += TimerElapsed;
      timer_.Start();
    }

    private QuadGrid quad_grid_form_ = new QuadGrid();

    protected override Action<string, string> DetailFormUpdater => (leftText, rightText) =>
    {
      quad_grid_form_.Invoke(new Action(() => quad_grid_form_.UpdateText(leftText, rightText)));
    };

    private UserControl RPM()
    {
      // 如果 rpm_ 為空，則初始化
      if (rpm_ == null)
      {
        rpm_ = new AngularGauge("x10000rpm")
        {
          Dock = DockStyle.Fill,
        };
        rpm_.SetGaugeMaxValue(10);
        rpm_.pie_chart_.AnimationsSpeed = TimeSpan.FromSeconds(3.0);
      }
      return rpm_;
    }
    private UserControl Power()
    {
      // 如果 rpm_ 為空，則初始化
      if (power_ == null)
      {
        power_ = new AngularGauge("x10000rpm")
        {
          Dock = DockStyle.Fill,
        };
        power_.SetGaugeMaxValue(10);
        power_.pie_chart_.AnimationsSpeed = TimeSpan.FromSeconds(3.0);
      }
      return power_;
    }


    protected override CartesianChart CreateThumbnail()
    {
      var chart = base.CreateThumbnail();//用基類製作一個出來
      if (chart.YAxes?.FirstOrDefault() is Axis yAxis)
      {
        yAxis.MaxLimit = 6; // 修改 MaxLimit 属性
      }
      return chart;
    }


    private UserControl MotorTemperature()
    {
      // 創建 CartesianChart
      var temperature = 80;
      return new CartesianChart
      {
        Dock = DockStyle.Fill, 
        XAxes = new Axis[]
        {
          new Axis
          {
            MinLimit = 0,
            MaxLimit = 60,
            //UnitWidth = 60,
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
        YAxes = new Axis[]
        {
          new Axis
          {
            MinLimit = 20,
            MaxLimit = temperature,
            UnitWidth = 4,
            Labeler = value =>
            {
              if (value == temperature)
              {
                return $"{temperature}°C";
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
        Series = new ISeries[]
          {
            new LineSeries<ObservableValue>
            {
                Values = motor_temperature_data_,
                Fill = new SolidColorPaint(0x257002b0),
                GeometrySize = 0,
                Stroke = new SolidColorPaint(0xff7002b0, 1),
                LineSmoothness = 0
            }
          },
        Title = new LabelVisual
        {
          Text = $"{Language.GetString("馬達溫度")} (° C)",
          TextSize = 16,
          Paint = new SolidColorPaint(SKColors.Black)
          {
            SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字體
          }
        },
        DrawMargin = new LiveChartsCore.Measure.Margin(20, 30, 15, 15),//設定 左、上、右、下 的邊界大小
      };
    }



    private UserControl MotorCurrent()
    {
      return new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]{new LineSeries<ObservableValue>{
            Values = data_,
            Fill = new SolidColorPaint(0x257002b0), // 填充颜色
            GeometrySize = 0, // 无点标记
            Stroke = new SolidColorPaint(0xff7002b0, 1), // 线条颜色和粗细
            LineSmoothness = 0, // 无弧度
          },
        },
        XAxes = new Axis[]
        {
          new Axis
          {
            MinLimit = 0,
            MaxLimit = 60,
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
        YAxes = new Axis[]
        {
          new Axis
          {
            MinLimit = 0,
            MaxLimit = base.MaxLoadingValue,
            //UnitWidth = 2,
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
        DrawMargin = new LiveChartsCore.Measure.Margin(20, 30, 15, 15),//設定 左、上、右、下 的邊界大小
        Title = new LabelVisual
        {
          Text = $"{Language.GetString("主軸附載")} (%)",
          TextSize = 16,
          Paint = new SolidColorPaint(SKColors.Black)
          {
            SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字體
          },
        }
      };
    }
    public override Form GetDetailForm()
    {
      // 创建并配置要添加的 AngularGauge 控件
      quad_grid_form_.AddToPanel(MotorCurrent(), RPM(), MotorTemperature(), Power(), DetailInfo, "");
      return quad_grid_form_;
    }


    /// <summary>
    /// 因為主軸不透過
    /// </summary>
    /// <param name="motor_current"></param>
    override public void Update(double motor_current)
    {

    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
      try
      {
        // 使用 FileStream 以只讀模式打開文件
        using (var fileStream = new FileStream("spindle_info.ini", FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = new StreamReader(fileStream))
        {
          IniData data = new FileIniDataParser().ReadData(reader);

          // 獲取 INI 文件中的數據
          string currentStr = data["Spindle"]["Current"];
          string motorTempStr = data["Spindle"]["MotorTemperature"];
          string speed = data["Spindle"]["Speed"];
          double speedValue = int.TryParse(speed, out int rpm) ? rpm : 0;

          string status = data["Spindle"]["Status"];
          string internalStatus = data["Spindle"]["InternalStatus"];
          string power = data["Spindle"]["Power"];
          double powerValue = double.TryParse(power, out double value) ? value : 0.0;

          string busVoltage = data["Spindle"]["BusVoltage"];
          string inverterTemp = data["Spindle"]["InverterTemperature"];
          // 尝试解析 busVoltage 和 inverterTemp 为浮点数
          double busVoltageValue = double.TryParse(busVoltage, out double bv) ? bv : 0.0;
          double inverterTempValue = double.TryParse(inverterTemp, out double it) ? it : 0.0;

          // 嘗試解析數據
          double motorCurrent = double.TryParse(currentStr, out double current) ? current : 0.0;
          double motorTemperature = double.TryParse(motorTempStr, out double temperature) ? temperature : 0.0;

          // 記錄所有值到一行日誌
          //Log.Information("Current: {Current}\tMotor Temperature: {MotorTemperature}\tSpeed: {Speed}\tStatus: {Status}\tInternal Status: {InternalStatus}\tPower: {Power}\tBus Voltage: {BusVoltage}\tInverter Temperature: {InverterTemperature}",
          //    currentStr, motorTempStr, speed, status, internalStatus, power, busVoltage, inverterTemp);

          // 解析數據
          if (double.TryParse(currentStr, out double motor_current) &&
              double.TryParse(motorTempStr, out double motor_temperature))
          {
            // 更新圖表數據
            data_.Add(new ObservableValue(motor_current));
            motor_temperature_data_.Add(new ObservableValue(motor_temperature));
            rpm_.UpdateValue(rpm / 10000);
            power_.UpdateValue(powerValue / 1000);
            // 保持數據點數量不超過 60
            if (data_.Count > 60) data_.RemoveAt(0);
            if (motor_temperature_data_.Count > 60) motor_temperature_data_.RemoveAt(0);
            DetailInfo = $@"{Language.GetString("轉速")}:         {speedValue,6:F0} RPM    
{Language.GetString("電流")}:          {motorCurrent.ToString("F1"),6} A
{Language.GetString("馬達溫度")}:     {motorTemperature.ToString("F1"),6} °C
{Language.GetString("功率")}:         {powerValue.ToString("F1"),6} kW
{Language.GetString("匯流排電壓")}:   {busVoltageValue.ToString("F1"),6} V    
{Language.GetString("變頻器溫度")}:   {busVoltageValue.ToString("F1"),6} °C
";

            var right_text = $@"{Language.GetString("匯流排電壓")}:   {busVoltageValue.ToString("F1"),6} V 
{Language.GetString("變頻器溫度")}:   {busVoltageValue.ToString("F1"),6} °C";
            quad_grid_form_.UpdateText(DetailInfo, right_text);

            // 更新 SubTitle
            double latestValue = data_.Last().Value ?? 0.0; // 获取最新数据
            double loading = CalculateLoading(latestValue); // 计算负载百分比
            SubTitle = $"{loading:F1} % | {speedValue:F0} RPM";

          }
        }
      }
      catch (Exception ex)
      {
        // 處理文件讀取失敗的情況
        Console.WriteLine($"讀取 spindle_info.ini 時發生錯誤: {ex.Message}");
      }
    }



    protected override (string LeftText, string RightInfo) UpdateDetailData()
    {
      return base.GetText();
    }

  }
}
