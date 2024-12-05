using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using LoadMonitor.Components;

namespace LoadMonitor
{
  public partial class Thumbnail : Form
  {
    private PartBase part_base_;
    public Thumbnail(CartesianChart thumbnail_chart, PartBase part_base, string mainTitle, string subTitle)
    {
      InitializeComponent();
      RegisterMouseEvents(this); // 為所有子元件註冊事件
      LabelTitle.Text = mainTitle;
      Labelsummary.Text = subTitle;
      PanelThumbnail.Controls.Clear();
      thumbnail_chart.Dock = DockStyle.Fill; // 確保圖表填滿 Panel
      PanelThumbnail.Controls.Add(thumbnail_chart);
      part_base_ = part_base;
    }

    private void Thumbnail_Load(object sender, EventArgs e)
    {
      this.MouseEnter += Thumbnail_MouseEnter;
      this.MouseLeave += Thumbnail_MouseLeave;

      // 為 PanelThumbnail 添加點擊事件
      //PanelThumbnail.Click += PanelThumbnail_Click;
      //this.OnClick += PanelThumbnail_Click;

    }
    // 父元件變色
    private void Thumbnail_MouseEnter(object sender, EventArgs e)
    {
      this.BackColor = Color.LightBlue; // 背景變藍色
    }

    private void Thumbnail_MouseLeave(object sender, EventArgs e)
    {
      this.BackColor = Color.White; // 恢復背景顏色
    }

    // 註冊所有子元件的事件
    private void RegisterMouseEvents(Control control)
    {
      foreach (Control child in control.Controls)
      {
        // 讓子元件的滑鼠事件觸發父元件事件
        child.MouseEnter += (s, e) => Thumbnail_MouseEnter(s, e);
        child.MouseLeave += (s, e) => Thumbnail_MouseLeave(s, e);

        // 如果有子控件，遞迴註冊
        if (child.HasChildren)
        {
          RegisterMouseEvents(child);
        }
      }
    }


    // 點擊 PanelThumbnail 時顯示詳細信息
    private void PanelThumbnail_Click(EventArgs e)
    {
      part_base_.DetailForm.TopLevel = false; // 設置為非頂層窗口
      part_base_.DetailForm.Dock = DockStyle.Fill; // 填充父控件
      part_base_.DetailForm.Show(); // 顯示詳細信息
    }

    private void Thumbnail_Click(object sender, EventArgs e)
    {
      part_base_.DetailChartPanel.Controls.Clear();
      part_base_.DetailForm.TopLevel = false; // 設置為非頂層窗口
      part_base_.DetailForm.Dock = DockStyle.Fill; // 填充父控件
      part_base_.DetailForm.Show(); // 顯示詳細信息
      part_base_.DetailChartPanel.Controls.Add(part_base_.DetailForm);

    }
  }
}
