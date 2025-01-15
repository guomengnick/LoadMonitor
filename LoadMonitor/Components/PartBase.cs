using System;
using System.Timers;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;
using static LoadMonitor.Data.HistoryData;
using Serilog;
using LoadMonitor.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;


namespace LoadMonitor.Components
{
  public abstract class PartBase : UserControl
  {

    public string MainTitle { get; set; } // 主標題
    public string SubTitle { get; set; }  // 副標題
    public string DetailInfo { get; set; } // 詳細信息
    public bool IsSelected { get; set; }  // 是否被選中
    public double MaxLoadingValue { get; protected set; }// 最大负载值

    public string setting_name_ { get; set; }
    public MainForm MainForm { get; set; }

    public Thumbnail thumbnail_;

    private Form detailForm_; // 延遲初始化字段

    private Action<string, string> updateDetailTextAction_;

    public HistoryData history_data_;

    readonly protected SKColor line_color_;
    readonly protected SKColor fill_color_;
    public Form DetailForm
    {
      get
      {
        // 延遲初始化，只有在首次取用時才調用 
        if (detailForm_ == null)
        {
          detailForm_ = GetDetailForm();
        }
        return detailForm_;
      }
    }

    public Panel DetailChartPanel;

    protected ObservableCollection<ObservableValue> data_ = new ObservableCollection<ObservableValue>();

    public static PartBase Create(string type, string name,
        Panel DetailChartPanel, double max_current_value, SKColor color,
        MainForm owner, string image_path, string setting_name)
    {
      return type switch
      {
        "Spindle" => new Spindle(name, DetailChartPanel, max_current_value, color, owner, image_path, setting_name),
        "CutMotor" => new CutMotor(name, DetailChartPanel, max_current_value, color, owner, image_path, setting_name),
        "TransferRack" => new TransferRack(name, DetailChartPanel, max_current_value, color, owner, image_path, setting_name),
        "Overview" => new Overview(name, DetailChartPanel, max_current_value, color, owner, image_path, setting_name),
        _ => throw new ArgumentException($"Unknown component type: {type}")
      };
    }

    public PartBase(string name,
        double maxLoadingValue, Panel DetailChartPanel, SKColor chart_color,
        MainForm owner, string image_path, string setting_name)
    {
      MainForm = owner;


      MaxLoadingValue = maxLoadingValue;
      this.setting_name_ = setting_name;
      this.DetailChartPanel = DetailChartPanel;

      line_color_ = new SKColor(chart_color.Red, chart_color.Green, chart_color.Blue, 0xee);
      fill_color_ = new SKColor(chart_color.Red, chart_color.Green, chart_color.Blue, 0x60);

      MainTitle = name;

      var image = Image.FromFile(image_path);
      //thumbnail_ = new Thumbnail(image, this);//對縮圖賦值, 給機器的縮圖
      thumbnail_ = new Thumbnail(CreateThumbnail(), image, this);//對縮圖賦值, 給的是笛卡兒座標


      TEST.TEST.Add60EmptyData(data_);
      //TEST.TEST.AddData(data_);

      IsSelected = false;

      Initialize(maxLoadingValue);
      history_data_ = new HistoryData(maxLoadingValue, 6/*每個部件都預設取6筆就好*/, 
        MainForm.update_timer_.Interval/*讀取頻率*/);
      //對此部件更新色彩
      GetDetailForm();
    }



    // 获取小时和 6 小时的平均负载信息
    public string GetLoadSummary()
    {
      double oneHourAverage = Math.Ceiling(history_data_.GetAveragePercentage(TimeUnit.OneHour));
      double sixHoursAverage = Math.Ceiling(history_data_.GetAveragePercentage(TimeUnit.SixHours));

      if (history_data_.IsExceedingMax())
      {
        Serilog.Log.Information($"{MainTitle} : 超出附載");
        thumbnail_.ShowRemindBell();
      }

      return $"1小時 平均附載 : {oneHourAverage:F0}% \r\n\r\n6小時平均附載 : {sixHoursAverage:F0}%";
    }

    public void ResetData()
    {
      history_data_.ResetData();
    }

    public bool IsExceedingMax()
    {
      return history_data_.IsExceedingMax();
    }

    // 获取加载百分比
    protected double CalculateLoading(double currentValue)
    {
      if (MaxLoadingValue <= 0)
        throw new InvalidOperationException("MaxLoadingValue must be greater than 0.");

      return currentValue / MaxLoadingValue * 100.0; // 返回百分比
    }

    public double GetCurrentLoad()
    {
      return data_.LastOrDefault()?.Value ?? 0.0;
    }

    public virtual (string Summary, string DetailInfo) GetText()
    {
      double current_loading = history_data_.GetAveragePercentage(TimeUnit.Seconds);
      double current_loading_1h = history_data_.GetAveragePercentage(TimeUnit.OneHour);
      double current_loading_6h = history_data_.GetAveragePercentage(TimeUnit.SixHours);
      string left = $@"  {Language.GetString("負載")} 
  {string.Format(Language.GetString("過去{}小時"), "1")}       {(int)Math.Round(CalculateLoading(current_loading))}%
  {string.Format(Language.GetString("過去{}小時"), "6")}       {(int)CalculateLoading(current_loading_1h)} %
  {string.Format(Language.GetString("過去{}小時"), "24")}     {(int)CalculateLoading(current_loading_6h)} %
";

      

      string right = $@"  {Language.GetString("峰值負荷")} 
  {string.Format(Language.GetString("過去{}小時"), "1")}       {history_data_.GetPeakValue(TimeUnit.OneHour):F1} %
  {string.Format(Language.GetString("過去{}小時"), "6")}       {history_data_.GetPeakValue(TimeUnit.SixHours):F1} %
  {string.Format(Language.GetString("過去{}小時"), "24")}     {history_data_.GetPeakValue(TimeUnit.Day):F1} %
";
      //Log.Information($"基類:{this.GetType().Name} 更新電流");


      return (left, right);
    }


    // 縮圖由基類創建，讓縮圖都長一樣
    virtual protected CartesianChart CreateThumbnail()
    {
      // 初始化图表
      return new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]
          {
            new LineSeries<ObservableValue>
            {
              Values = data_,
              Fill = new SolidColorPaint(fill_color_), // 填充颜色
              GeometrySize = 0, // 无点标记
              Stroke = new SolidColorPaint(line_color_, 2), // 线条颜色和粗细
              LineSmoothness = 0, // 无弧度
            },
          },
        AnimationsSpeed = TimeSpan.FromMilliseconds(5000), // 设置动画持续时间为1秒
        XAxes = new[] {
          new Axis {
            MinLimit = 0,
            MaxLimit = 60,
            IsVisible = false,
          }
        }, // 隐藏 X 轴
        YAxes = new[] { new Axis { IsVisible = false,
          SeparatorsPaint = null, MinLimit = 0, MaxLimit = Math.Ceiling(MaxLoadingValue * 3), } }, // 隐藏 Y 轴
        Padding = new Padding(0), // 移除内部边距
        Margin = new Padding(0), // 移除外部边距
        LegendPosition = LiveChartsCore.Measure.LegendPosition.Hidden,
        Legend = null,
      };
    }

    private void Initialize(double maxLoadingValue)
    {
      if (maxLoadingValue <= 0)
        throw new ArgumentException("MaxLoadingValue must be greater than 0.");

      if (setting_name_ == null)
      {
        MaxLoadingValue = maxLoadingValue;
        return;
      }


      // 讀取設定值 (報警百分比)
      var property = Settings.Default.GetType().GetProperty(setting_name_);
      int threshold_ratio = -1;
      if (property != null)
      {
        // 獲取設定值 (默認為 100%)
        threshold_ratio = Convert.ToInt32(property.GetValue(Settings.Default));
        // 根據設定值更新 MaxLoadingValue
        MaxLoadingValue = MaxLoadingValue * threshold_ratio/*這裡是整數*/ / 100/*轉成百分比*/;
      }
      else
      {
        //throw new ArgumentException($"設定名稱 {setting_name_} 無效！"); 
      }
      Log.Information($@"{MainTitle}, 最大值:{maxLoadingValue}, 警示值:{MaxLoadingValue},{MachineTypeHelper.ToString((MachineType)Settings.Default.MachineType)}({Settings.Default.MachineType}), 參數:{setting_name_}");

    }


    //點擊了提醒鈴鐺
    public virtual string OnReminderBellClick()
    {
      //如果要換行的話，要加上@
      var s = $@"{MainTitle} {Language.GetString("電機保養提醒文字")}";
      Log.Information(s);
      return $@"{MainTitle} {Language.GetString("電機保養提醒文字")}";
    }


    //取最新的5筆做平均
    virtual public double AverageData(double motor_current)
    {
      //return motor_current;
      // 獲取最近 5 筆數據
      var recentValues = data_.Skip(Math.Max(0, data_.Count - 5)).Select(value => value.Value).ToList();
      // 計算平均值
      double average = recentValues.Any() ? recentValues.Average().GetValueOrDefault() : 0;

      // 可以根據需求對 `motor_current` 和 `average` 進行操作
      double finalValue = (motor_current + average) / 2; // 例如：取平均值
      return finalValue;
    }
    protected void TrimCollection(ObservableCollection<ObservableValue> collection)
    {
      while (collection.Count > 60)
      {
        collection.RemoveAt(0);
      }
    }

    /// <summary>
    /// 更新組件數據，並通知派生類更新詳細頁面
    /// </summary>
    virtual public void Update(double motor_current)
    {
      motor_current = AverageData(motor_current);
      data_.Add(new ObservableValue(motor_current));
      // 將計算後的值加入到 data_

      TrimCollection(data_);

      history_data_.AddDataPoint(motor_current);
      var current_loading = history_data_.GetAveragePercentage(TimeUnit.Seconds);
      string summary = $"{Math.Round(current_loading * 100)}%";

      GetLoadSummary();

      thumbnail_.UpdateSummary(summary);

      var detail_texts = UpdateDetailData();
      //Log.Information($"部件:{MainTitle}detail_texts:{detail_texts}");
      DetailFormUpdater(detail_texts.LeftText, detail_texts.RightInfo);
    }


    private double GetWarningThreshold()//獲取此部件的"報警提示%數"
    {
      var property = Settings.Default.GetType().GetProperty(setting_name_);
      if (property != null)
      {
        return Convert.ToDouble(property.GetValue(Settings.Default));
      }
      throw new ArgumentException($"設定名稱 {setting_name_} 無效！");
    }



    public void UpdateWarningThreshold(int threshold_ratio/*%數*/)
    {

      if (setting_name_ == null)
      {
        return;//沒有要設定的值的話，就return 掉
      }

      // 同步更新 Settings.Default
      var property = Settings.Default.GetType().GetProperty(setting_name_);
      if (property == null)
      {
        return;
      }

      MaxLoadingValue *= (Convert.ToDouble(threshold_ratio) / 100.0/*轉成 0~1 的倍數*/);
      property.SetValue(Settings.Default, Convert.ChangeType(threshold_ratio, property.PropertyType));
      Settings.Default.Save(); // 保存修改
      history_data_.max_value_ = MaxLoadingValue;
    }



    /// <summary>
    /// 派生類需實現的方法，用於創建對應的詳細頁面
    /// </summary>
    public abstract Form GetDetailForm();

    /// <summary>
    /// 派生類需實現的方法，用於更新數據
    /// </summary>
    protected abstract (string LeftText, string RightInfo) UpdateDetailData();

    protected abstract Action<string, string> DetailFormUpdater { get; }
  }

}
