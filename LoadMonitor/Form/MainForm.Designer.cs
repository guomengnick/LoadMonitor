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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      flowLayoutPanel1 = new FlowLayoutPanel();
      DummyPanel = new Panel();
      DetailChartPanel = new Panel();
      radioButton1 = new RadioButton();
      panel1 = new Panel();
      label1 = new Label();
      menuStrip1 = new MenuStrip();
      toolStripMenuItem1 = new ToolStripMenuItem();
      toolStripMenuItem2 = new ToolStripMenuItem();
      ddadasToolStripMenuItem = new ToolStripMenuItem();
      dasdasdToolStripMenuItem = new ToolStripMenuItem();
      flowLayoutPanel1.SuspendLayout();
      panel1.SuspendLayout();
      menuStrip1.SuspendLayout();
      SuspendLayout();
      // 
      // flowLayoutPanel1
      // 
      flowLayoutPanel1.AutoScroll = true;
      flowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
      flowLayoutPanel1.Controls.Add(DummyPanel);
      flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      flowLayoutPanel1.Location = new Point(12, 42);
      flowLayoutPanel1.Margin = new Padding(0);
      flowLayoutPanel1.Name = "flowLayoutPanel1";
      flowLayoutPanel1.Size = new Size(306, 727);
      flowLayoutPanel1.TabIndex = 3;
      flowLayoutPanel1.WrapContents = false;
      // 
      // DummyPanel
      // 
      DummyPanel.Location = new Point(3, 3);
      DummyPanel.Name = "DummyPanel";
      DummyPanel.Size = new Size(280, 10);
      DummyPanel.TabIndex = 8;
      // 
      // DetailChartPanel
      // 
      DetailChartPanel.BackColor = Color.Transparent;
      DetailChartPanel.Location = new Point(335, 42);
      DetailChartPanel.Name = "DetailChartPanel";
      DetailChartPanel.Size = new Size(1000, 727);
      DetailChartPanel.TabIndex = 8;
      // 
      // radioButton1
      // 
      radioButton1.AutoSize = true;
      radioButton1.Location = new Point(335, 788);
      radioButton1.Name = "radioButton1";
      radioButton1.Size = new Size(105, 19);
      radioButton1.TabIndex = 3;
      radioButton1.TabStop = true;
      radioButton1.Text = "radioButton1";
      radioButton1.UseVisualStyleBackColor = true;
      // 
      // panel1
      // 
      panel1.Controls.Add(label1);
      panel1.Location = new Point(510, 810);
      panel1.Name = "panel1";
      panel1.Size = new Size(341, 100);
      panel1.TabIndex = 10;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Microsoft JhengHei UI", 18F, FontStyle.Bold);
      label1.Location = new Point(123, 9);
      label1.Name = "label1";
      label1.Size = new Size(204, 30);
      label1.TabIndex = 0;
      label1.Text = "label1一歇歇的自";
      // 
      // menuStrip1
      // 
      menuStrip1.ImageScalingSize = new Size(25, 25);
      menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2 });
      menuStrip1.Location = new Point(0, 0);
      menuStrip1.Name = "menuStrip1";
      menuStrip1.ShowItemToolTips = true;
      menuStrip1.Size = new Size(1347, 33);
      menuStrip1.TabIndex = 11;
      menuStrip1.Text = "menuStrip1";
      // 
      // toolStripMenuItem1
      // 
      toolStripMenuItem1.Name = "toolStripMenuItem1";
      toolStripMenuItem1.Size = new Size(12, 29);
      // 
      // toolStripMenuItem2
      // 
      toolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] { ddadasToolStripMenuItem, dasdasdToolStripMenuItem });
      toolStripMenuItem2.Image = (Image)resources.GetObject("toolStripMenuItem2.Image");
      toolStripMenuItem2.Name = "toolStripMenuItem2";
      toolStripMenuItem2.Size = new Size(53, 29);
      toolStripMenuItem2.Text = "   ";
      // 
      // ddadasToolStripMenuItem
      // 
      ddadasToolStripMenuItem.Name = "ddadasToolStripMenuItem";
      ddadasToolStripMenuItem.Size = new Size(180, 22);
      ddadasToolStripMenuItem.Text = "語言";
      // 
      // dasdasdToolStripMenuItem
      // 
      dasdasdToolStripMenuItem.Name = "dasdasdToolStripMenuItem";
      dasdasdToolStripMenuItem.Size = new Size(180, 22);
      dasdasdToolStripMenuItem.Text = "設置";
      // 
      // MainForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1347, 787);
      Controls.Add(panel1);
      Controls.Add(radioButton1);
      Controls.Add(DetailChartPanel);
      Controls.Add(flowLayoutPanel1);
      Controls.Add(menuStrip1);
      Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
      Icon = (Icon)resources.GetObject("$this.Icon");
      MainMenuStrip = menuStrip1;
      Name = "MainForm";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "GAM 330AT";
      flowLayoutPanel1.ResumeLayout(false);
      panel1.ResumeLayout(false);
      panel1.PerformLayout();
      menuStrip1.ResumeLayout(false);
      menuStrip1.PerformLayout();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel DetailChartPanel;
    private Panel DummyPanel;
    private TextBox TextBoxDetailInfo;
    private RadioButton radioButton1;
    private Panel panel1;
    private Label label1;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem toolStripMenuItem1;
    private ToolStripMenuItem toolStripMenuItem2;
    private ToolStripMenuItem ddadasToolStripMenuItem;
    private ToolStripMenuItem dasdasdToolStripMenuItem;
  }

}
