using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.VisualElements;

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMonitor.Components
{
  internal class TransferRack : PartBase
  {

    public TransferRack(string mainTitle, string subTitle, string detailInfo, Panel DetailChartPanel, double max_current) :
      base(mainTitle, subTitle, detailInfo, max_current, DetailChartPanel) // 主轴最大负载值为 10A
    {
      TEST.TEST.Add60EmptyData(data_);
      single_form_ = new Single();
    }
    private Single single_form_;


    public override (string Summary, string DetailInfo) GetText()
    {
      double latestValue = data_.Last().Value ?? 0.0; // 获取最新数据
      double loading = CalculateLoading(latestValue); // 计算负载百分比
      string summary = $"{loading:F1} %";
      string detailInfo = $"{MainTitle} 附載: {loading:F1} % \r\n電流: {latestValue} A";

      single_form_.UpdateText(detailInfo, GetLoadSummary());
      return (summary, detailInfo);
    }



    public override Form GetDetailForm()
    {
      // 创建并配置要添加的 AngularGauge 控件

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
        // 設置 X 軸
        XAxes = new Axis[]
        {
          new Axis
          {
              Labels = null, // 自動生成標籤
              MinLimit = 0, // 最小值（秒）
              MaxLimit = 60, // 最大值（秒）
              UnitWidth = 30,
              Labeler = value => $"{60 - value:F0} s", // 顯示反向標籤（60秒到0秒）
              LabelsPaint = new SolidColorPaint(SKColors.Black), // 標籤顏色
              SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) // 分隔線顏色
          }
        },
        // 設置 Y 軸
        YAxes = new Axis[]
        {
          new Axis
          {
              MinLimit = 0, // 最小值（安培）
              MaxLimit = Math.Ceiling(base.MaxLoadingValue), // 最大值（安培）
              Labels = new[] { "0", "1", "2" , "3" }, // 僅顯示 0, 1, 2
              Labeler = value => $"{value:F0} A", // 格式化為 0 A, 1 A, 2 A
              LabelsPaint = new SolidColorPaint(SKColors.Black), // 標籤顏色
              SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) // 分隔線顏色
          }
        }
      };
      cartesianChart_.Title = new LabelVisual
      {
        Text = MainTitle,
        TextSize = 24,
        Paint = new SolidColorPaint(SKColors.Black)
        {
          SKTypeface = SKFontManager.Default.MatchCharacter('汉') // 設定中文字體
        },
      };
      single_form_.AddToPanel(cartesianChart_, base.DetailInfo, "");

      return single_form_;
    }
  }
}
