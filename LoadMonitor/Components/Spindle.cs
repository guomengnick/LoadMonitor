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
    private ThreadTimer timer_ = new ThreadTimer(1000);
    public Spindle(string mainTitle, string subTitle, string detailInfo, Panel DetailChartPanel, double max_current) : 
      base(mainTitle, subTitle, detailInfo, max_current, DetailChartPanel) // 主轴最大负载值为 10A
    {

      quad_grid_form_ = new QuadGrid();

      power_ = new AngularGauge("功率(kW)")
      {
        Dock = DockStyle.Fill,
      };
      rpm_ = new AngularGauge("x10000rpm")
      {
        Dock = DockStyle.Fill,
      };

      timer_.Elapsed += TimerElapsed;
      timer_.Start();
    }


    public override (string Summary, string DetailInfo) GetText()
    {
      quad_grid_form_.UpdateText(DetailInfo, "");
      return (SubTitle, DetailInfo);
    }

    private UserControl RPM()
    {
      return rpm_;
    }
    private UserControl Power()
    {
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
      return new CartesianChart
      {
        /// 如果圖表要設定中文, 需要以下這樣設定才行
        //XAxes = new Axis[]{
        //  new Axis{
        //    Name = "Salesman/woman",
        //    Labels = new string[] { "主軸", "赵", "张" },
        //    LabelsPaint = new SolidColorPaint(SKColors.Black){
        //        SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字體
        //    },
        //  }
        //},
        /// 如果圖表要設定中文, 需要以下這樣設定才行
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]{new LineSeries<ObservableValue>{
            Values = motor_temperature_data_,
            Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
            GeometrySize = 0, // 无点标记
            Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
            LineSmoothness = 0, // 无弧度
        },
        },
        Title = new LabelVisual
        {
          Text = "馬達溫度 (° C)",
          TextSize = 16,
          Paint = new SolidColorPaint(SKColors.Black)
          {
            SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字體
          },
        },
      };

    }
    private UserControl MotorCurrent()
    {
      return new CartesianChart
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
        Title = new LabelVisual
        {
          Text = "主軸附載 (%)",
          TextSize = 16,
          Paint = new SolidColorPaint(SKColors.Black)
          {
            SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字體
          },
        }
      };
    }
    private QuadGrid quad_grid_form_;
    public override Form GetDetailForm()
    {
      // 创建并配置要添加的 AngularGauge 控件
      quad_grid_form_.AddToPanel(MotorCurrent(), RPM(), MotorTemperature(), Power(), DetailInfo, "");
      return quad_grid_form_;
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
            power_.UpdateValue(value / 1000);

            // 保持數據點數量不超過 60
            if (data_.Count > 60) data_.RemoveAt(0);
            if (motor_temperature_data_.Count > 60) motor_temperature_data_.RemoveAt(0);
            DetailInfo = $@"轉速:         {speedValue,6:F0} RPM    
電流:          {motorCurrent.ToString("F1"),6} A
馬達溫度:     {motorTemperature.ToString("F1"),6} °C
功率:         {powerValue.ToString("F1"),6} kW
匯流排電壓:   {busVoltageValue.ToString("F1"),6} V    
變頻器溫度:   {busVoltageValue.ToString("F1"),6} °C
";

            quad_grid_form_.UpdateText(DetailInfo, "");

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


  }
}
