using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.VisualElements;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LoadMonitor.TEST;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LoadMonitor.Components
{
  // 切割軸XYZ 馬達
  internal class CutMotor : PartBase
  {
    private string motor_name_;
    public CutMotor(string motor_name) : base(1) // 主轴最大负载值为 10A
    {
      motor_name_ = motor_name;
      TEST.TEST.Add60EmptyData(data_);
    }
    public override Form GetDetailForm()
    {

      Single single_form = new Single();
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
            //MaxLimit = 5, // 最大值（安培）
            //UnitWidth = 1,

            Labeler = value => $"{value:F1} A", // 格式化為 0.0 A
            LabelsPaint = new SolidColorPaint(SKColors.Black), // 標籤顏色
            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) // 分隔線顏色
        }
    }
      };
      cartesianChart_.Title = new LabelVisual
      {
        Text = motor_name_,
        TextSize = 24,
        Paint = new SolidColorPaint(SKColors.Black),
      };
      single_form.AddToPanel(cartesianChart_);

      return single_form;
    }


    //public override (string Summary, string DetailInfo) GetText()
    //{
    //  if (data_ == null || data_.Count == 0)
    //    return ("No Data", "No data available for calculation.");

    //  double latestValue = data_.Last().Value ?? 0.0; // 获取最新数据
    //  double loading = CalculateLoading(latestValue); // 计算负载百分比

    //  string summary = $"Load: {latestValue:F1} A";
    //  string detailInfo = $"Current Load: {loading:F1}%";

    //  return (summary, detailInfo);
    //}


  }
}
