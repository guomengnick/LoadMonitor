using System;
using System.Timers;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;
using static LoadMonitor.HistoryData;
using Serilog;


namespace LoadMonitor.Components
{
  public abstract class PartBase : UserControl
  {

    public string MainTitle { get; set; } // 主標題
    public string SubTitle { get; set; }  // 副標題
    public string DetailInfo { get; set; } // 詳細信息
    public bool IsSelected { get; set; }  // 是否被選中
    public double MaxLoadingValue { get; protected set; }// 最大负载值

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
        Panel DetailChartPanel, double max_current_value, SKColor color)
    {
      return type switch
      {
        "Spindle" => new Spindle(name, DetailChartPanel, max_current_value, color),
        "CutMotor" => new CutMotor(name, DetailChartPanel, max_current_value, color),
        "TransferRack" => new TransferRack(name, DetailChartPanel, max_current_value, color),
        "Overview" => new Overview(name, DetailChartPanel, max_current_value, color),
        _ => throw new ArgumentException($"Unknown component type: {type}")
      };
    }

    public PartBase(string name,
        double maxLoadingValue, Panel DetailChartPanel, SKColor chart_color)
    {
      MaxLoadingValue = maxLoadingValue;
      this.DetailChartPanel = DetailChartPanel;

      line_color_ = new SKColor(chart_color.Red, chart_color.Green, chart_color.Blue, 0xee);
      fill_color_ = new SKColor(chart_color.Red, chart_color.Green, chart_color.Blue, 0x60);

      MainTitle = name;
      thumbnail_ = new Thumbnail(CreateThumbnail(), this);//對縮圖賦值

      TEST.TEST.Add60EmptyData(data_);
      //TEST.TEST.AddData(data_);

      IsSelected = false;

      Initialize(maxLoadingValue);
      history_data_ = new HistoryData(maxLoadingValue, 1/*每個部件都預設取3筆就好*/);
      //對此部件更新色彩
      GetDetailForm();
    }



    // 获取小时和 6 小时的平均负载信息
    public string GetLoadSummary()
    {
      double oneHourAverage = Math.Ceiling(history_data_.GetAverage(TimeUnit.OneHour));
      double sixHoursAverage = Math.Ceiling(history_data_.GetAverage(TimeUnit.SixHours));

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

    public double GetCurrentLoad() {
      return data_.LastOrDefault()?.Value ?? 0.0; 
    }

    public virtual (string Summary, string DetailInfo) GetText()
    {
      double current_loading = history_data_.GetAverage(TimeUnit.Seconds);
      string summary = $"{Math.Round(current_loading * 100)}%";


      string detailInfo = $"{Language.GetString("電流")}: {current_loading:F1} A";
      //Log.Information($"基類:{this.GetType().Name} 更新電流");


      return (summary, detailInfo);
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
              Stroke = new SolidColorPaint(line_color_, 1), // 线条颜色和粗细
              LineSmoothness = 0, // 无弧度
            },
          },
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
      MaxLoadingValue = maxLoadingValue;
    }


    //點擊了提醒鈴鐺
    public virtual string OnReminderBellClick()
    {
      //如果要換行的話，要加上@
      var s = $@"{MainTitle} {Language.GetString("電機保養提醒文字")}";
      Log.Information(s);
      return $@"{MainTitle} {Language.GetString("電機保養提醒文字")}";
    }

    /// <summary>
    /// 更新組件數據，並通知派生類更新詳細頁面
    /// </summary>
    virtual public void Update(double motor_current)
    {
      data_.Add(new ObservableValue(motor_current));
      if (data_.Count > 60) data_.RemoveAt(0); // 限制最多 60 个点

      history_data_.AddDataPoint(motor_current);
      var current_loading = history_data_.GetAverage(TimeUnit.Seconds);
      string summary = $"{Math.Round(current_loading * 100)}%";

      GetLoadSummary();

      thumbnail_.UpdateSummary(summary);

      var detail_texts = UpdateDetailData();
      //Log.Information($"detail_texts:{detail_texts}");
      DetailFormUpdater(detail_texts.LeftText, detail_texts.RightInfo);
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
