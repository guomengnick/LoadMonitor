using System;
using System.Timers;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;


using FormsTimer = System.Windows.Forms.Timer;
using ThreadingTimer = System.Threading.Timer;
using SkiaSharp;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace LoadMonitor.Components
{
  public abstract class PartBase : UserControl
  {

    public string MainTitle { get; set; } // 主標題
    public string SubTitle { get; set; }  // 副標題
    public string DetailInfo { get; set; } // 詳細信息

    public Thumbnail thumbnail_;

    private Form detailForm_; // 延遲初始化字段
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
    public bool IsSelected { get; set; }  // 是否被選中

    public Panel DetailChartPanel;

    public static PartBase Create(string type, string name, string subTitle, 
        string detailInfo, Panel DetailChartPanel)
    {
      return type switch
      {
        "Spindle" => new Spindle(name, subTitle, detailInfo, DetailChartPanel),
        "CutMotor" => new CutMotor(name, subTitle, detailInfo, DetailChartPanel),
        "TransferRack" => new TransferRack(name, subTitle, detailInfo, DetailChartPanel),
        "Overview" => new Overview(name, subTitle, detailInfo, DetailChartPanel),
        _ => throw new ArgumentException($"Unknown component type: {type}")
      };
    }
    public PartBase(string mainTitle, string subTitle, string detailInfo,
        double maxLoadingValue, Panel DetailChartPanel)
    {
      this.DetailChartPanel = DetailChartPanel;
      thumbnail_ = new Thumbnail(CreateThumbnail(), this, mainTitle, subTitle);//對縮圖賦值
      //TEST.TEST.Add60EmptyData(data_);
      TEST.TEST.AddData(data_);

      MainTitle = mainTitle;
      SubTitle = subTitle;
      DetailInfo = detailInfo;

      IsSelected = false;
      Initialize(maxLoadingValue);
    }

    // 最大负载值
    public double MaxLoadingValue { get; protected set; }
    // 获取加载百分比
    protected double CalculateLoading(double currentValue)
    {
      if (MaxLoadingValue <= 0)
        throw new InvalidOperationException("MaxLoadingValue must be greater than 0.");

      return currentValue / MaxLoadingValue * 100.0; // 返回百分比
    }
    protected ObservableCollection<ObservableValue> data_ = new ObservableCollection<ObservableValue>();

    protected FormsTimer updateTimer_;
    public virtual string summary_ { get; protected set; } = "Default Overview";
    public virtual string detailInfo_ { get; protected set; } = "Default Details";

    public double GetCurrentLoad() { return data_.Last().Value ?? 0.0; }


    public virtual (string Summary, string DetailInfo) GetText()
    {
      double latestValue = data_.Last().Value ?? 0.0; // 获取最新数据
      double loading = CalculateLoading(latestValue); // 计算负载百分比
      string summary = $"{loading:F1} %";
      string detailInfo = $"{MainTitle} 附載: {loading:F1} % \r\n電流: {latestValue} A";
      return (summary, detailInfo);
    }

    virtual protected CartesianChart CreateThumbnail()
    {
      // 初始化图表
      CartesianChart cartesianChart_ = new CartesianChart
      {
        Dock = DockStyle.Fill, // 填满 Panel
        Series = new ISeries[]
                {
                    new LineSeries<ObservableValue>
                    {
                        Values = data_,
                        Fill = new SolidColorPaint(SKColors.LightBlue), // 填充颜色
                        GeometrySize = 0, // 无点标记
                        Stroke = new SolidColorPaint(SKColors.Blue, 1), // 线条颜色和粗细
                        LineSmoothness = 0, // 无弧度
                    },
                },
        XAxes = new[] { new Axis { IsVisible = false, SeparatorsPaint = null } }, // 隐藏 X 轴
        YAxes = new[] { new Axis { IsVisible = false, SeparatorsPaint = null } }, // 隐藏 Y 轴
        DrawMargin = null, // 移除绘图区域边距
        Padding = new Padding(0), // 移除内部边距
        Margin = new Padding(0), // 移除外部边距
        LegendPosition = LiveChartsCore.Measure.LegendPosition.Hidden, // 移除图例
        Legend = null,
      };
      return cartesianChart_;
    }

    private void Initialize(double maxLoadingValue)
    {
      if (maxLoadingValue <= 0)
        throw new ArgumentException("MaxLoadingValue must be greater than 0.");
      MaxLoadingValue = maxLoadingValue;

      detailInfo_ = string.Empty;
      summary_ = string.Empty;

      //Controls.Add(CreateThumbnail());//縮圖
      //Controls.Add(thumbnail_);//縮圖
      

      // 初始化定时器
      updateTimer_ = new FormsTimer
      {
        Interval = 1300
      };
      updateTimer_.Tick += UpdateData;
    }

    public void Update(double motor_current)
    {
      // 随机生成一个新的数据点（模拟实时数据）
      data_.Add(new ObservableValue(motor_current));
      if (data_.Count > 60) data_.RemoveAt(0); // 限制最多 60 个点
                                               // 更新概要信息
                                               // 更新详细信息
      detailInfo_ = GenerateDetailInfo((int)motor_current);
    }

    protected void UpdateData(object sender, EventArgs e)
    {
      if (data_ == null)
      {
        return;
      }
      Update(new Random().Next(0, 100));
    }

    private void InitializeComponent()
    {

    }


    // 生成详细信息的方法
    private string GenerateDetailInfo(int newValue)
    {
      return $@"
Query Speed: {newValue + 1} RPM
Query Status: Normal
Query Internal Status: OK
Query Power: {newValue * 1.2:F1} kW
Query Bus Voltage: {newValue * 2.3:F1} V
Query Current: {newValue * 0.8:F1} A
Query Motor Temperature: {20 + newValue / 10} °C
Query Inverter Temperature: {25 + newValue / 15} °C
";
    }

    // 詳細頁面
    public abstract Form GetDetailForm();
  }

}
