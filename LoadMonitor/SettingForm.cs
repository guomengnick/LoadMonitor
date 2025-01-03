using LoadMonitor.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LoadMonitor
{
  public partial class SettingForm : Form
  {

    private readonly List<Thumbnail> thumbnails; // 用於存儲所有 Thumbnail
    public SettingForm(FlowLayoutPanel originalPanel)
    {
      InitializeComponent();
      thumbnails = new List<Thumbnail>(); // 初始化列表
      // 複製原始 FlowLayoutPanel 的內容
      foreach (Control control in originalPanel.Controls)
      {
        if (control is not Thumbnail thumbnail)
        {
          continue;//只顯示部件的UI
        }
        if(control is Overview overview)
        {
          continue;//不顯示整機負載設定值
        }


        thumbnails.Add(thumbnail); // 添加到列表
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
            var trackBar = new System.Windows.Forms.TrackBar
            {
              Minimum = thumbnail.trackBar1.Minimum,
              Maximum = thumbnail.trackBar1.Maximum,
              Value = thumbnail.trackBar1.Value,
              TickFrequency = 10,
              Height = 20,
              Width = 210 // 寬度固定為 200px
            };
            trackBar.Tag = thumbnail.PartBase;

            // TrackBar 基於 warnRatioLabelText 的右側
            trackBar.Location = new Point(
                warnRatioLabelText.Location.X + warnRatioLabelText.Width + 5, // warnRatioLabelText 的右側 10px
                warnRatioLabelText.Location.Y - 5 // 與文字對齊
            );
            thumbnailPanel.Controls.Add(trackBar);

            // 顯示警告比例值 (warnRatioLabel)
            var warnRatioLabel = new Label
            {
              Text = $"{thumbnail.WarnRatioLabel?.Text ?? "100"}",
              AutoSize = true,
              Font = new Font("Arial", 10, FontStyle.Regular),
              Location = new Point(
                    trackBar.Location.X + trackBar.Width + 5, // TrackBar 的右側 10px
                    warnRatioLabelText.Location.Y
                )
            };

            trackBar.ValueChanged += (s, e) => warnRatioLabel.Text = trackBar.Value.ToString();

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
      this.Controls.Add(flowLayoutPanel1);// 添加到 SettingForm

      // 添加底部按鈕
      AddBottomButtons();
    }

    private void AddBottomButtons()
    {
      ButtonReset.Click += (s, e) =>
      {
        foreach (var thumbnail in thumbnails)
        {
          thumbnail.PartBase.UpdateWarningThreshold(1.0); // 設定為 100%
        }
        MessageBox.Show("所有部件已回覆預設值 (100%)");
      };

      ButtonSaveAndClose.Click += (s, e) =>
      {
        // 遍歷所有 TrackBar 並更新其關聯的 PartBase
        foreach (var trackBar in flowLayoutPanel1.Controls.OfType<Panel>()
                 .SelectMany(panel => panel.Controls.OfType<System.Windows.Forms.TrackBar>()))
        {
          if (trackBar.Tag is PartBase partBase)
          {
            double thresholdRatio = trackBar.Value / 100.0; // 計算比率
            partBase.UpdateWarningThreshold(thresholdRatio); // 更新比率
          }
        }

        MessageBox.Show("所有設定已儲存");
      };

    }
  }
}
