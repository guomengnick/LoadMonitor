using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LoadMonitor.TEST;
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
    private string motor_name_;

    public TransferRack(string motor_name)
    {
      motor_name_ = motor_name;
      TEST.TEST.Add60EmptyData(data_);
    }

    public override Form GetDetailForm()
    {
      Single single_form = new Single();
      // 创建并配置要添加的 View 控件

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
  }
}
