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
      panel1.SuspendLayout();
      SuspendLayout();
      // 
      // PanelThumbnail
      // 
      PanelThumbnail.BackColor = SystemColors.Control;
      PanelThumbnail.Location = new Point(5, 5);
      PanelThumbnail.Margin = new Padding(0);
      PanelThumbnail.Name = "PanelThumbnail";
      PanelThumbnail.Size = new Size(121, 99);
      PanelThumbnail.TabIndex = 2;
      // 
      // LabelTitle
      // 
      LabelTitle.AutoSize = true;
      LabelTitle.Font = new Font("Microsoft JhengHei UI", 14F);
      LabelTitle.Location = new Point(131, 14);
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
      ButtonRemindBell.Location = new Point(299, 0);
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
      panel1.Location = new Point(138, 43);
      panel1.Name = "panel1";
      panel1.Size = new Size(179, 46);
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
      // Thumbnail
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(346, 109);
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
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private Panel PanelThumbnail;
    private Label LabelTitle;
    private Button ButtonRemindBell;
    private Panel panel1;
    private Label Labelsummary;
  }
}