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
            menuStrip1 = new MenuStrip();
            toolStripMenuItem2 = new ToolStripMenuItem();
            ToolStripMenuItemLanguege = new ToolStripMenuItem();
            簡體中文ToolStripMenuItem = new ToolStripMenuItem();
            繁體中文ToolStripMenuItem = new ToolStripMenuItem();
            englishToolStripMenuItem = new ToolStripMenuItem();
            ToolStripMenuItemSetting = new ToolStripMenuItem();
            flowLayoutPanel1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel1.Controls.Add(DummyPanel);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(1, 4);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(336, 733);
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
            DetailChartPanel.Location = new Point(337, 6);
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
            menuStrip1.Location = new Point(1312, 4);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.ShowItemToolTips = true;
            menuStrip1.Size = new Size(57, 33);
            menuStrip1.TabIndex = 11;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] { ToolStripMenuItemLanguege, ToolStripMenuItemSetting });
            toolStripMenuItem2.Image = (Image)resources.GetObject("toolStripMenuItem2.Image");
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(49, 29);
            toolStripMenuItem2.Text = " ";
            // 
            // ToolStripMenuItemLanguege
            // 
            ToolStripMenuItemLanguege.DropDownItems.AddRange(new ToolStripItem[] { 簡體中文ToolStripMenuItem, 繁體中文ToolStripMenuItem, englishToolStripMenuItem });
            ToolStripMenuItemLanguege.Name = "ToolStripMenuItemLanguege";
            ToolStripMenuItemLanguege.Size = new Size(100, 22);
            ToolStripMenuItemLanguege.Text = "語言";
            // 
            // 簡體中文ToolStripMenuItem
            // 
            簡體中文ToolStripMenuItem.Name = "簡體中文ToolStripMenuItem";
            簡體中文ToolStripMenuItem.Size = new Size(124, 22);
            簡體中文ToolStripMenuItem.Text = "简体中文";
            簡體中文ToolStripMenuItem.Click += 簡體中文ToolStripMenuItem_Click;
            // 
            // 繁體中文ToolStripMenuItem
            // 
            繁體中文ToolStripMenuItem.Name = "繁體中文ToolStripMenuItem";
            繁體中文ToolStripMenuItem.Size = new Size(124, 22);
            繁體中文ToolStripMenuItem.Text = "繁體中文";
            繁體中文ToolStripMenuItem.Click += 繁體中文ToolStripMenuItem_Click;
            // 
            // englishToolStripMenuItem
            // 
            englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            englishToolStripMenuItem.Size = new Size(124, 22);
            englishToolStripMenuItem.Text = "English";
            englishToolStripMenuItem.Click += 英文ToolStripMenuItem_Click;
            // 
            // ToolStripMenuItemSetting
            // 
            ToolStripMenuItemSetting.Name = "ToolStripMenuItemSetting";
            ToolStripMenuItemSetting.Size = new Size(100, 22);
            ToolStripMenuItemSetting.Text = "設置";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1359, 740);
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlowLayoutPanel flowLayoutPanel1;
    private Panel DetailChartPanel;
    private Panel DummyPanel;
    private TextBox TextBoxDetailInfo;
    private RadioButton radioButton1;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem toolStripMenuItem2;
    private ToolStripMenuItem ToolStripMenuItemLanguege;
    private ToolStripMenuItem ToolStripMenuItemSetting;
    private Label LabelMachineType;
    private ToolStripMenuItem 簡體中文ToolStripMenuItem;
    private ToolStripMenuItem 繁體中文ToolStripMenuItem;
    private ToolStripMenuItem englishToolStripMenuItem;
  }

}
