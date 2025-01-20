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
using System.Runtime.InteropServices;

using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using LoadMonitor.Components;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Serilog;


namespace LoadMonitor
{
  public partial class Thumbnail : Form
  {
    public PartBase PartBase => part_base_;
    private PartBase part_base_;

    private CartesianChart chart_; // 保存圖表引用
    public Color UnactiveColor { get; private set; }

    private bool is_selected_ = false; // 新增屬性標記選中狀態

    public System.Drawing.Image image_;
    public Thumbnail(CartesianChart thumbnail_chart, PartBase part_base)
    {
      thumbnail_chart.Dock = DockStyle.Fill; // 確保圖表填滿 Panel
      InitializeForm(part_base);
      PanelThumbnail.Controls.Add(thumbnail_chart);
      image_ = new System.Drawing.Bitmap(1, 1);
    }

    public Thumbnail(CartesianChart thumbnail_chart, System.Drawing.Image image, PartBase part_base)
    {
      thumbnail_chart.Dock = DockStyle.Fill; // 確保圖表填滿 Panel
      InitializeForm(part_base);
      PanelThumbnail.Controls.Add(thumbnail_chart);
      image_ = image;
    }


    public Thumbnail(System.Drawing.Image image, PartBase part_base)
    {
      InitializeForm(part_base);
      image_ = image;

      PanelThumbnail.BackgroundImage = image_;
      PanelThumbnail.BackgroundImageLayout = ImageLayout.Stretch; // 確保圖片適應面板大小
    }

    // 創建圖表的方法
    private CartesianChart CreateChart()
    {
      chart_ = part_base_.CreateThumbnail();
      return chart_;
    }


    // 重置圖表
    public void ResetChart()
    {
      // 釋放舊圖表資源
      if (chart_ != null)
      {
        Controls.Remove(chart_); // 從 UI 移除
        chart_.Dispose(); // 釋放資源
        Serilog.Log.Information("釋放縮圖的圖表資源");

        chart_ = null;
      }

      // 創建新圖表並添加到 Panel
      chart_ = CreateChart();
      chart_.Dock = DockStyle.Fill;
      Controls.Add(chart_);
      Serilog.Log.Information("重置了縮圖的圖表資源");
    }


    private void InitializeForm(PartBase part_base)
    {
      InitializeComponent();
      unactive_color_ = this.BackColor;

      // 初始化未選中狀態的底色
      UnactiveColor = this.BackColor;


      RegisterMouseEvents(this); // 為所有子元件註冊事件
      LabelTitle.Text = part_base.MainTitle;
      PanelThumbnail.Controls.Clear();


      part_base_ = part_base;
      //var read_current_timer_ = new System.Timers.Timer(200);
      //bool isHighlighted = false; // 用于切换状态
      //read_current_timer_.Elapsed += (object? sender, ElapsedEventArgs e) =>
      //{
      //  // 切换背景颜色
      //  if (part_base.thumbnail_ != null)
      //  {
      //    part_base.thumbnail_.Invoke(new Action(() =>
      //    {
      //      part_base.thumbnail_.BackColor = isHighlighted ? Color.LightGreen : Color.LightPink;
      //      isHighlighted = !isHighlighted; // 切换状态
      //    }));
      //  }
      //};

      // 启动定时器

      //read_current_timer_.Start();
      unactive_color_ = this.BackColor;


      trackBar1.ValueChanged += (s, e) =>
      {
        // 獲取當前的 TrackBar 值
        int currentValue = trackBar1.Value;

        // 更新 WarnRatioLabel 的文本
        WarnRatioLabel.Text = $"{currentValue}";
        part_base_.UpdateWarningThreshold(currentValue);
      };
      LabelLoadingWarnning.Text = Language.GetString("負載警示Text");
    }

    public void UpdateSummary(string text)
    {

      if (Labelsummary.Text == text)
      {// 沒變化就return
        return;
      }
      // 更新 UI 控件
      if (Labelsummary.InvokeRequired)
      {
        Labelsummary.Invoke(new Action(() => Labelsummary.Text = text));
      }
      else
      {
        Labelsummary.Text = text;
      }
    }


    private Color unactive_color_;
    private void Thumbnail_Load(object sender, EventArgs e)
    {
      this.MouseEnter += Thumbnail_MouseEnter;
      this.MouseLeave += Thumbnail_MouseLeave;

    }



    public void SetSelected(bool isSelected)
    {
      is_selected_ = isSelected;
      this.BackColor = isSelected ? Color.LightBlue : UnactiveColor; // 選中或未選中設定對應顏色
      Log.Information($"{part_base_.MainTitle} 選中:{isSelected}");
    }


    // 父元件變色
    private void Thumbnail_MouseEnter(object sender, EventArgs e)
    {
      if (!is_selected_) // 未選中時才改變背景色
      {
        this.BackColor = Color.LightBlue;
      }
    }

    private void Thumbnail_MouseLeave(object sender, EventArgs e)
    {
      if (!is_selected_) // 未選中時恢復原始背景色
      {
        this.BackColor = UnactiveColor;
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
        child.Click += (s, e) => Thumbnail_Click(s, e);

        // 如果有子控件，遞迴註冊
        if (child.HasChildren)
        {
          RegisterMouseEvents(child);
        }

      }
    }

    public void Thumbnail_Click(object sender, EventArgs e)
    {
      part_base_.MainForm.OnThumbnailClicked(this);

      part_base_.DetailChartPanel.Controls.Clear();

      part_base_.DetailForm.TopLevel = false; // 設置為非頂層窗口
      part_base_.DetailForm.Dock = DockStyle.Fill; // 填充父控件
      part_base_.DetailForm.Show(); // 顯示詳細信息
      part_base_.DetailChartPanel.Controls.Add(part_base_.DetailForm);


      var mainForm = this.FindForm() as MainForm;
      //Debug.WriteLine($"點擊 當前頁面{this.FindForm().ToString()}    副頁面:{this.FindForm().Parent.ToString()}");
      if (mainForm != null)
      {
        mainForm.OnThumbnailClicked(this);
      }
    }

    public void ShowRemindBell()
    {
      this.ButtonRemindBell.Visible = true;
    }

    private void ButtonRemindBellClick(object sender, MouseEventArgs e)
    {
      this.ButtonRemindBell.Visible = false;
      var text = part_base_.OnReminderBellClick();
      var form = new RemindForm(text);

      var reset_reminder_ = form.ShowDialogInfo();//獲取使用者點擊'關閉'或'清除提醒'

      if (reset_reminder_)
      {
        part_base_.ResetData();
      }
      this.ButtonRemindBell.Visible = !reset_reminder_;
    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    public void ShowWarnningThreshold(bool show)
    {
      LabelLoadingWarnning.Visible = show;
      LabelRatio.Visible = show;
      WarnRatioLabel.Visible = show;
      trackBar1.Visible = show;
    }
  }
}
