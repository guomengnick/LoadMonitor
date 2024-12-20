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
      components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      flowLayoutPanel1 = new FlowLayoutPanel();
      DummyPanel = new Panel();
      DetailChartPanel = new Panel();
      menuStrip1 = new MenuStrip();
      toolStripMenuItem2 = new ToolStripMenuItem();
      ToolStripMenuItemLanguege = new ToolStripMenuItem();
      簡體中文ToolStripMenuItem = new ToolStripMenuItem();
      繁體中文ToolStripMenuItem = new ToolStripMenuItem();
      englishToolStripMenuItem = new ToolStripMenuItem();
      COMPortToolStripMenuItem1 = new ToolStripMenuItem();
      panel1 = new Panel();
      notifyIcon1 = new NotifyIcon(components);
      contextMenuStrip1 = new ContextMenuStrip(components);
      關閉監控軟體ToolStripMenuItem = new ToolStripMenuItem();
      LabelMainMessage = new Label();
      flowLayoutPanel1.SuspendLayout();
      menuStrip1.SuspendLayout();
      contextMenuStrip1.SuspendLayout();
      SuspendLayout();
      // 
      // flowLayoutPanel1
      // 
      flowLayoutPanel1.AutoScroll = true;
      flowLayoutPanel1.Controls.Add(DummyPanel);
      flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      flowLayoutPanel1.Location = new Point(-12, 4);
      flowLayoutPanel1.Margin = new Padding(0);
      flowLayoutPanel1.Name = "flowLayoutPanel1";
      flowLayoutPanel1.Size = new Size(336, 760);
      flowLayoutPanel1.TabIndex = 3;
      flowLayoutPanel1.WrapContents = false;
      // 
      // DummyPanel
      // 
      DummyPanel.Location = new Point(3, 3);
      DummyPanel.Name = "DummyPanel";
      DummyPanel.Size = new Size(310, 10);
      DummyPanel.TabIndex = 8;
      // 
      // DetailChartPanel
      // 
      DetailChartPanel.BackColor = Color.Transparent;
      DetailChartPanel.Location = new Point(327, 32);
      DetailChartPanel.Name = "DetailChartPanel";
      DetailChartPanel.Size = new Size(1027, 733);
      DetailChartPanel.TabIndex = 8;
      // 
      // menuStrip1
      // 
      menuStrip1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      menuStrip1.Dock = DockStyle.None;
      menuStrip1.ImageScalingSize = new Size(25, 25);
      menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem2 });
      menuStrip1.Location = new Point(1300, 1);
      menuStrip1.Name = "menuStrip1";
      menuStrip1.ShowItemToolTips = true;
      menuStrip1.Size = new Size(55, 33);
      menuStrip1.TabIndex = 11;
      menuStrip1.Text = "menuStrip1";
      // 
      // toolStripMenuItem2
      // 
      toolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItemLanguege, COMPortToolStripMenuItem1 });
      toolStripMenuItem2.Image = Properties.Resources.SETT26472;
      toolStripMenuItem2.Name = "toolStripMenuItem2";
      toolStripMenuItem2.Size = new Size(47, 29);
      toolStripMenuItem2.Text = " ";
      // 
      // ToolStripMenuItemLanguege
      // 
      ToolStripMenuItemLanguege.DropDownItems.AddRange(new ToolStripItem[] { 簡體中文ToolStripMenuItem, 繁體中文ToolStripMenuItem, englishToolStripMenuItem });
      ToolStripMenuItemLanguege.Name = "ToolStripMenuItemLanguege";
      ToolStripMenuItemLanguege.Size = new Size(140, 22);
      ToolStripMenuItemLanguege.Text = "語言";
      // 
      // 簡體中文ToolStripMenuItem
      // 
      簡體中文ToolStripMenuItem.Name = "簡體中文ToolStripMenuItem";
      簡體中文ToolStripMenuItem.Size = new Size(122, 22);
      簡體中文ToolStripMenuItem.Text = "简体中文";
      // 
      // 繁體中文ToolStripMenuItem
      // 
      繁體中文ToolStripMenuItem.Name = "繁體中文ToolStripMenuItem";
      繁體中文ToolStripMenuItem.Size = new Size(122, 22);
      繁體中文ToolStripMenuItem.Text = "繁體中文";
      // 
      // englishToolStripMenuItem
      // 
      englishToolStripMenuItem.Name = "englishToolStripMenuItem";
      englishToolStripMenuItem.Size = new Size(122, 22);
      englishToolStripMenuItem.Text = "English";
      // 
      // COMPortToolStripMenuItem1
      // 
      COMPortToolStripMenuItem1.Name = "COMPortToolStripMenuItem1";
      COMPortToolStripMenuItem1.Size = new Size(140, 22);
      COMPortToolStripMenuItem1.Text = "COM口設置";
      // 
      // panel1
      // 
      panel1.BackColor = Color.DimGray;
      panel1.Location = new Point(327, 20);
      panel1.Name = "panel1";
      panel1.Size = new Size(2, 730);
      panel1.TabIndex = 0;
      // 
      // notifyIcon1
      // 
      notifyIcon1.ContextMenuStrip = contextMenuStrip1;
      notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
      notifyIcon1.Text = "負載監控";
      notifyIcon1.Visible = true;
      notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
      // 
      // contextMenuStrip1
      // 
      contextMenuStrip1.Items.AddRange(new ToolStripItem[] { 關閉監控軟體ToolStripMenuItem });
      contextMenuStrip1.Name = "contextMenuStrip1";
      contextMenuStrip1.Size = new Size(147, 26);
      // 
      // 關閉監控軟體ToolStripMenuItem
      // 
      關閉監控軟體ToolStripMenuItem.Name = "關閉監控軟體ToolStripMenuItem";
      關閉監控軟體ToolStripMenuItem.Size = new Size(146, 22);
      關閉監控軟體ToolStripMenuItem.Text = "關閉監控軟體";
      關閉監控軟體ToolStripMenuItem.Click += 關閉監控軟體ToolStripMenuItem_Click;
      // 
      // LabelMainMessage
      // 
      LabelMainMessage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      LabelMainMessage.BackColor = Color.Transparent;
      LabelMainMessage.Font = new Font("Microsoft JhengHei UI", 18F, FontStyle.Bold);
      LabelMainMessage.ForeColor = Color.Red;
      LabelMainMessage.Location = new Point(401, 1);
      LabelMainMessage.Name = "LabelMainMessage";
      LabelMainMessage.Size = new Size(861, 28);
      LabelMainMessage.TabIndex = 12;
      LabelMainMessage.Text = "    ";
      LabelMainMessage.TextAlign = ContentAlignment.MiddleCenter;
      // 
      // MainForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1359, 766);
      Controls.Add(LabelMainMessage);
      Controls.Add(panel1);
      Controls.Add(menuStrip1);
      Controls.Add(flowLayoutPanel1);
      Controls.Add(DetailChartPanel);
      Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
      Icon = (Icon)resources.GetObject("$this.Icon");
      MainMenuStrip = menuStrip1;
      Name = "MainForm";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "  使用率監控";
      flowLayoutPanel1.ResumeLayout(false);
      menuStrip1.ResumeLayout(false);
      menuStrip1.PerformLayout();
      contextMenuStrip1.ResumeLayout(false);
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private FlowLayoutPanel flowLayoutPanel1;
    private Panel DetailChartPanel;
    private TextBox TextBoxDetailInfo;
    private RadioButton radioButton1;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem toolStripMenuItem2;
    private ToolStripMenuItem ToolStripMenuItemLanguege;
    private Label LabelMachineType;
    private ToolStripMenuItem 簡體中文ToolStripMenuItem;
    private ToolStripMenuItem 繁體中文ToolStripMenuItem;
    private ToolStripMenuItem englishToolStripMenuItem;
    private Panel DummyPanel;
    private ToolStripMenuItem COMPortToolStripMenuItem1;
    private Panel panel1;
    private NotifyIcon notifyIcon1;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem 關閉監控軟體ToolStripMenuItem;
    private Label LabelMainMessage;
  }

}
