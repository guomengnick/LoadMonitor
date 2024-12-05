using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System.Collections.ObjectModel;
using System.Windows.Forms; // 如果是 WinForms 的 Timer


namespace LoadMonitor
{
  partial class MainForm
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      flowLayoutPanel1 = new FlowLayoutPanel();
      PartInfoPanel = new Panel();
      InfoPanel = new Panel();
      TitleLabel = new Label();
      SummaryLabel = new Label();
      ThumbnailPanel = new Panel();
      DetailChartPanel = new Panel();
      DetailTextPanel = new Panel();
      TextBoxDetailInfo = new TextBox();
      PartInfoPanel.SuspendLayout();
      InfoPanel.SuspendLayout();
      DetailTextPanel.SuspendLayout();
      SuspendLayout();
      // 
      // flowLayoutPanel1
      // 
      flowLayoutPanel1.AutoScroll = true;
      flowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
      flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      flowLayoutPanel1.Location = new Point(12, 12);
      flowLayoutPanel1.Margin = new Padding(0);
      flowLayoutPanel1.Name = "flowLayoutPanel1";
      flowLayoutPanel1.Size = new Size(307, 727);
      flowLayoutPanel1.TabIndex = 3;
      flowLayoutPanel1.WrapContents = false;
      // 
      // PartInfoPanel
      // 
      PartInfoPanel.Controls.Add(InfoPanel);
      PartInfoPanel.Controls.Add(ThumbnailPanel);
      PartInfoPanel.Location = new Point(8, 749);
      PartInfoPanel.Name = "PartInfoPanel";
      PartInfoPanel.Size = new Size(269, 100);
      PartInfoPanel.TabIndex = 8;
      // 
      // InfoPanel
      // 
      InfoPanel.Controls.Add(TitleLabel);
      InfoPanel.Controls.Add(SummaryLabel);
      InfoPanel.Location = new Point(117, 3);
      InfoPanel.Name = "InfoPanel";
      InfoPanel.Size = new Size(148, 79);
      InfoPanel.TabIndex = 1;
      // 
      // TitleLabel
      // 
      TitleLabel.Font = new Font("Microsoft JhengHei UI", 14F);
      TitleLabel.Location = new Point(3, 9);
      TitleLabel.Name = "TitleLabel";
      TitleLabel.Size = new Size(100, 33);
      TitleLabel.TabIndex = 2;
      TitleLabel.Text = "主軸";
      // 
      // SummaryLabel
      // 
      SummaryLabel.Location = new Point(18, 38);
      SummaryLabel.Name = "SummaryLabel";
      SummaryLabel.Size = new Size(127, 32);
      SummaryLabel.TabIndex = 1;
      SummaryLabel.Text = "轉速 : 37000 rmp\r\n溫度 : 35 度\r\n";
      // 
      // ThumbnailPanel
      // 
      ThumbnailPanel.Location = new Point(3, 3);
      ThumbnailPanel.Name = "ThumbnailPanel";
      ThumbnailPanel.Size = new Size(108, 97);
      ThumbnailPanel.TabIndex = 0;
      // 
      // DetailChartPanel
      // 
      DetailChartPanel.BackColor = Color.Transparent;
      DetailChartPanel.Location = new Point(335, 12);
      DetailChartPanel.Name = "DetailChartPanel";
      DetailChartPanel.Size = new Size(1000, 570);
      DetailChartPanel.TabIndex = 8;
      // 
      // DetailTextPanel
      // 
      DetailTextPanel.Controls.Add(TextBoxDetailInfo);
      DetailTextPanel.Location = new Point(335, 590);
      DetailTextPanel.Name = "DetailTextPanel";
      DetailTextPanel.Padding = new Padding(10);
      DetailTextPanel.Size = new Size(1000, 150);
      DetailTextPanel.TabIndex = 9;
      // 
      // TextBoxDetailInfo
      // 
      TextBoxDetailInfo.BackColor = SystemColors.ButtonFace;
      TextBoxDetailInfo.BorderStyle = BorderStyle.None;
      TextBoxDetailInfo.Font = new Font("Microsoft JhengHei", 13F, FontStyle.Regular, GraphicsUnit.Point, 136);
      TextBoxDetailInfo.Location = new Point(13, 5);
      TextBoxDetailInfo.Multiline = true;
      TextBoxDetailInfo.Name = "TextBoxDetailInfo";
      TextBoxDetailInfo.Size = new Size(883, 137);
      TextBoxDetailInfo.TabIndex = 0;
      TextBoxDetailInfo.Text = "預設部件的附載甚麼的\r\n都顯示在這裡\r\n大約預計\r\n是有 6 行\r\n比如電流、電壓、附載\r\n之類什麼的\r\n";
      // 
      // MainForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1347, 748);
      Controls.Add(PartInfoPanel);
      Controls.Add(DetailTextPanel);
      Controls.Add(DetailChartPanel);
      Controls.Add(flowLayoutPanel1);
      Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
      Name = "MainForm";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "GAM 330AT";
      PartInfoPanel.ResumeLayout(false);
      InfoPanel.ResumeLayout(false);
      DetailTextPanel.ResumeLayout(false);
      DetailTextPanel.PerformLayout();
      ResumeLayout(false);
    }

    #endregion
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel DetailChartPanel;
    private Panel DetailTextPanel;
    private Panel PartInfoPanel;
    private Panel InfoPanel;
    private Label TitleLabel;
    private Label SummaryLabel;
    private Panel ThumbnailPanel;
    private TextBox TextBoxDetailInfo;
  }

}
