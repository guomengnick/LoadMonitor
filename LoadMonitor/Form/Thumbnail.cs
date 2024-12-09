using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
      var read_current_timer_ = new System.Timers.Timer(200);
      bool isHighlighted = false; // 用于切换状态
      read_current_timer_.Elapsed += (object? sender, ElapsedEventArgs e) =>
      {
        // 切换背景颜色
        if (part_base.thumbnail_ != null)
        {
          part_base.thumbnail_.Invoke(new Action(() =>
          {
            part_base.thumbnail_.BackColor = isHighlighted ? Color.LightGreen : Color.LightPink;
            isHighlighted = !isHighlighted; // 切换状态
          }));
        }
      };

      // 启动定时器

      //read_current_timer_.Start();
      unactive_color_ = this.BackColor;
    }
    private Color unactive_color_;
    private bool is_showing = false;
    private void Thumbnail_Load(object sender, EventArgs e)
    {
      this.MouseEnter += Thumbnail_MouseEnter;
      this.MouseLeave += Thumbnail_MouseLeave;

    }
    // 父元件變色
    private void Thumbnail_MouseEnter(object sender, EventArgs e)
    {
      Debug.WriteLine("ENTER");
      this.BackColor = Color.LightBlue; // 背景變藍色
    }

    private void Thumbnail_MouseLeave(object sender, EventArgs e)
    {
      Debug.WriteLine("LEAVE");

      if (!is_showing)
      {// 沒有在顯示時, 才恢復顏色
        this.BackColor = unactive_color_; // 恢復背景顏色
      }

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



    private void Thumbnail_Click(object sender, EventArgs e)
    {

      Debug.WriteLine("點擊");
      //is_showing = true; // 设置标志位
      //part_base_.thumbnail_.BackColor = Color.DeepSkyBlue;//標記此縮圖為顯示中
      //part_base_.thumbnail_.Refresh();


      part_base_.DetailChartPanel.Controls.Clear();
      part_base_.DetailForm.Shown += (s, e) =>
      {
        this.BackColor = Color.LightBlue; // 背景變藍色
      };

      // 设置状态为激活状态
      part_base_.DetailForm.FormClosed += (s, e) =>
      {
        is_showing = false;
        part_base_.thumbnail_.BackColor = Color.LightGreen;
      };

      part_base_.DetailForm.TopLevel = false; // 設置為非頂層窗口
      part_base_.DetailForm.Dock = DockStyle.Fill; // 填充父控件
      part_base_.DetailForm.Show(); // 顯示詳細信息
      part_base_.DetailChartPanel.Controls.Add(part_base_.DetailForm);
    }

    public void ShowRemindBell()
    {
      this.ButtonRemindBell.Visible = true;
    }
    private void ButtonRemindBellClick(object sender, MouseEventArgs e)
    {
      this.ButtonRemindBell.Visible = false;
      part_base_.OnReminderBellClick();
    }
  }
}
