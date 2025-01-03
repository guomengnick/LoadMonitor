using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoadMonitor
{
  public partial class SettingForm : Form
  {
    public SettingForm(FlowLayoutPanel originalPanel)
    {
      InitializeComponent();
      // 複製原始 FlowLayoutPanel 的內容
      foreach (Control control in originalPanel.Controls)
      {
        if (control is Thumbnail thumbnail)
        {
          // 創建容器 Panel，用於顯示該 Thumbnail 的屬性
          var thumbnailPanel = new Panel
          {
            Dock = DockStyle.Top,
            Height = 109, // 可根據需要調整高度
            Width = 400,
            BorderStyle = BorderStyle.FixedSingle
          };

          // 顯示部件標題 (LabelTitle)
          if (thumbnail.LabelTitle != null)
          {
            var titleLabel = new Label
            {
              Text = thumbnail.LabelTitle.Text,
              AutoSize = true,
              TextAlign = ContentAlignment.MiddleCenter,
              Font = new Font("Microsoft JhengHei UI", 16, FontStyle.Regular),
              Location = new Point(5, 5) // 左上角 (距左側 5px，距頂部 5px)
            };
            thumbnailPanel.Controls.Add(titleLabel);
          }

          // 顯示警告比例文字 (warnRatioLabelText)
          var warnRatioLabelText = new Label
          {
            Text = "負載率提示值",
            AutoSize = true,
            Font = new Font("Arial", 10, FontStyle.Regular),
            Location = new Point(10, 60) // 左下角 (距左側 5px，距頂部 50px)
          };
          thumbnailPanel.Controls.Add(warnRatioLabelText);

          // 添加滑動條 (TrackBar)
          if (thumbnail.trackBar1 != null)
          {
            var trackBar = new TrackBar
            {
              Minimum = thumbnail.trackBar1.Minimum,
              Maximum = thumbnail.trackBar1.Maximum,
              Value = thumbnail.trackBar1.Value,
              TickFrequency = 10,
              Height = 20,
              Width = 210 // 寬度固定為 200px
            };

            // TrackBar 基於 warnRatioLabelText 的右側
            trackBar.Location = new Point(
                warnRatioLabelText.Location.X + warnRatioLabelText.Width + 5, // warnRatioLabelText 的右側 10px
                warnRatioLabelText.Location.Y - 5 // 與文字對齊
            );
            trackBar.ValueChanged += (s, e) =>
            {

            };
            thumbnailPanel.Controls.Add(trackBar);

            // 顯示警告比例值 (warnRatioLabel)
            var warnRatioLabel = new Label
            {
              Text = $"{thumbnail.WarnRatioLabel?.Text ?? "100"} %",
              AutoSize = true,
              Font = new Font("Arial", 10, FontStyle.Regular),
              Location = new Point(
                    trackBar.Location.X + trackBar.Width + 5, // TrackBar 的右側 10px
                    warnRatioLabelText.Location.Y
                )
            };
            thumbnailPanel.Controls.Add(warnRatioLabel);
          }


          // 複製 Thumbnail 控件（需要根據你的需求實現複製邏輯）
          var newThumbnail = new Thumbnail(thumbnail.image_, thumbnail.PartBase);
          newThumbnail.TopLevel = false;
          newThumbnail.Click += (s, e) => { };
          newThumbnail.ShowWarnningThreshold(true); // 隱藏細節
          newThumbnail.Dock = DockStyle.Fill;
          newThumbnail.Show();

          //flowLayoutPanel1.Controls.Add(newThumbnail);
          flowLayoutPanel1.Controls.Add(thumbnailPanel);
        }
      }
      this.Controls.Add(flowLayoutPanel1);// 添加到 SettingForm
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
