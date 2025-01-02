namespace LoadMonitor
{
  partial class Thumbnail
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Thumbnail));
      PanelThumbnail = new Panel();
      LabelTitle = new Label();
      ButtonRemindBell = new Button();
      panel1 = new Panel();
      Labelsummary = new Label();
      trackBar1 = new TrackBar();
      WarnRatioLabel = new Label();
      label2 = new Label();
      label3 = new Label();
      panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
      SuspendLayout();
      // 
      // PanelThumbnail
      // 
      PanelThumbnail.BackColor = SystemColors.Control;
      PanelThumbnail.Location = new Point(5, 4);
      PanelThumbnail.Margin = new Padding(0);
      PanelThumbnail.Name = "PanelThumbnail";
      PanelThumbnail.Size = new Size(119, 101);
      PanelThumbnail.TabIndex = 2;
      // 
      // LabelTitle
      // 
      LabelTitle.AutoSize = true;
      LabelTitle.Font = new Font("Microsoft JhengHei UI", 14F);
      LabelTitle.Location = new Point(131, 7);
      LabelTitle.Name = "LabelTitle";
      LabelTitle.Size = new Size(71, 24);
      LabelTitle.TabIndex = 3;
      LabelTitle.Text = "切割X1";
      // 
      // ButtonRemindBell
      // 
      ButtonRemindBell.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      ButtonRemindBell.BackColor = Color.Transparent;
      ButtonRemindBell.FlatAppearance.BorderColor = Color.Silver;
      ButtonRemindBell.FlatAppearance.BorderSize = 0;
      ButtonRemindBell.FlatAppearance.MouseDownBackColor = Color.Transparent;
      ButtonRemindBell.FlatAppearance.MouseOverBackColor = Color.Transparent;
      ButtonRemindBell.FlatStyle = FlatStyle.Flat;
      ButtonRemindBell.ForeColor = Color.Transparent;
      ButtonRemindBell.Image = (Image)resources.GetObject("ButtonRemindBell.Image");
      ButtonRemindBell.Location = new Point(310, 0);
      ButtonRemindBell.Name = "ButtonRemindBell";
      ButtonRemindBell.Size = new Size(31, 37);
      ButtonRemindBell.TabIndex = 5;
      ButtonRemindBell.UseVisualStyleBackColor = false;
      ButtonRemindBell.Visible = false;
      ButtonRemindBell.MouseClick += ButtonRemindBellClick;
      // 
      // panel1
      // 
      panel1.Controls.Add(Labelsummary);
      panel1.Location = new Point(138, 30);
      panel1.Name = "panel1";
      panel1.Size = new Size(146, 30);
      panel1.TabIndex = 6;
      // 
      // Labelsummary
      // 
      Labelsummary.Location = new Point(3, 5);
      Labelsummary.Name = "Labelsummary";
      Labelsummary.Size = new Size(63, 19);
      Labelsummary.TabIndex = 5;
      Labelsummary.Text = "5%";
      // 
      // trackBar1
      // 
      trackBar1.Location = new Point(174, 77);
      trackBar1.Margin = new Padding(0);
      trackBar1.Maximum = 150;
      trackBar1.Minimum = 50;
      trackBar1.Name = "trackBar1";
      trackBar1.Size = new Size(123, 45);
      trackBar1.TabIndex = 7;
      trackBar1.TickFrequency = 20;
      trackBar1.Value = 100;
      trackBar1.Visible = false;
      // 
      // WarnRatioLabel
      // 
      WarnRatioLabel.AutoSize = true;
      WarnRatioLabel.Location = new Point(297, 81);
      WarnRatioLabel.Margin = new Padding(0);
      WarnRatioLabel.Name = "WarnRatioLabel";
      WarnRatioLabel.Size = new Size(28, 15);
      WarnRatioLabel.TabIndex = 8;
      WarnRatioLabel.Text = "100";
      WarnRatioLabel.Visible = false;
      WarnRatioLabel.Click += label1_Click;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(127, 81);
      label2.Name = "label2";
      label2.Size = new Size(55, 15);
      label2.TabIndex = 9;
      label2.Text = "負載警示";
      label2.Visible = false;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(322, 81);
      label3.Margin = new Padding(0);
      label3.Name = "label3";
      label3.Size = new Size(18, 15);
      label3.TabIndex = 10;
      label3.Text = "%";
      label3.Visible = false;
      // 
      // Thumbnail
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(357, 109);
      Controls.Add(WarnRatioLabel);
      Controls.Add(label3);
      Controls.Add(label2);
      Controls.Add(trackBar1);
      Controls.Add(panel1);
      Controls.Add(ButtonRemindBell);
      Controls.Add(LabelTitle);
      Controls.Add(PanelThumbnail);
      FormBorderStyle = FormBorderStyle.None;
      Name = "Thumbnail";
      Text = "Thumbnail";
      Load += Thumbnail_Load;
      Click += Thumbnail_Click;
      panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private Panel PanelThumbnail;
    private Label LabelTitle;
    private Button ButtonRemindBell;
    private Panel panel1;
    private Label Labelsummary;
    private TrackBar trackBar1;
    private Label WarnRatioLabel;
    private Label label2;
    private Label label3;
  }
}