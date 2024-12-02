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
      panel1 = new Panel();
      panel2 = new Panel();
      panel3 = new Panel();
      flowLayoutPanel1 = new FlowLayoutPanel();
      PartInfoPanel = new Panel();
      InfoPanel = new Panel();
      TitleLabel = new Label();
      SummaryLabel = new Label();
      ThumbnailPanel = new Panel();
      DetailChartPanel = new Panel();
      DetailTextPanel = new Panel();
      button1 = new Button();
      panel1.SuspendLayout();
      PartInfoPanel.SuspendLayout();
      InfoPanel.SuspendLayout();
      SuspendLayout();
      // 
      // panel1
      // 
      panel1.BorderStyle = BorderStyle.FixedSingle;
      panel1.Controls.Add(panel2);
      panel1.Location = new Point(705, 762);
      panel1.Name = "panel1";
      panel1.Size = new Size(92, 59);
      panel1.TabIndex = 0;
      // 
      // panel2
      // 
      panel2.Location = new Point(408, 171);
      panel2.Name = "panel2";
      panel2.Size = new Size(351, 238);
      panel2.TabIndex = 1;
      // 
      // panel3
      // 
      panel3.Location = new Point(318, 759);
      panel3.Name = "panel3";
      panel3.Size = new Size(371, 219);
      panel3.TabIndex = 2;
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
      flowLayoutPanel1.Paint += flowLayoutPanel1_Paint;
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
      SummaryLabel.Click += SummaryLabel_Click;
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
      DetailChartPanel.Location = new Point(335, 12);
      DetailChartPanel.Name = "DetailChartPanel";
      DetailChartPanel.Size = new Size(1000, 570);
      DetailChartPanel.TabIndex = 8;
      // 
      // DetailTextPanel
      // 
      DetailTextPanel.Location = new Point(335, 590);
      DetailTextPanel.Name = "DetailTextPanel";
      DetailTextPanel.Padding = new Padding(10);
      DetailTextPanel.Size = new Size(1000, 149);
      DetailTextPanel.TabIndex = 9;
      DetailTextPanel.Click += DetailTextPanel_Click;
      DetailTextPanel.Paint += DetailTextPanel_Paint;
      DetailTextPanel.MouseLeave += DetailTextPanel_MouseLeave;
      DetailTextPanel.MouseHover += DetailTextPanel_MouseHover;
      // 
      // button1
      // 
      button1.Location = new Point(819, 771);
      button1.Name = "button1";
      button1.Size = new Size(75, 23);
      button1.TabIndex = 0;
      button1.Text = "點擊測試";
      button1.UseVisualStyleBackColor = true;
      button1.Click += button1_Click;
      // 
      // MainForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1347, 746);
      Controls.Add(PartInfoPanel);
      Controls.Add(button1);
      Controls.Add(DetailTextPanel);
      Controls.Add(DetailChartPanel);
      Controls.Add(flowLayoutPanel1);
      Controls.Add(panel3);
      Controls.Add(panel1);
      Name = "MainForm";
      Text = "GAM 330AT";
      panel1.ResumeLayout(false);
      PartInfoPanel.ResumeLayout(false);
      InfoPanel.ResumeLayout(false);
      ResumeLayout(false);
    }
    #endregion

    private Panel panel1;
    private Panel panel2;
    private Panel panel3;
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel DetailChartPanel;
    private Panel DetailTextPanel;
    private Button button1;
    private Panel PartInfoPanel;
    private Panel InfoPanel;
    private Label TitleLabel;
    private Label SummaryLabel;
    private Panel ThumbnailPanel;
  }

}
