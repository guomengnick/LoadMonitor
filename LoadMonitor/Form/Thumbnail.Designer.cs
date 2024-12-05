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
      PanelThumbnail = new Panel();
      LabelTitle = new Label();
      Labelsummary = new Label();
      SuspendLayout();
      // 
      // PanelThumbnail
      // 
      PanelThumbnail.BackColor = SystemColors.Control;
      PanelThumbnail.Location = new Point(2, 2);
      PanelThumbnail.Margin = new Padding(0);
      PanelThumbnail.Name = "PanelThumbnail";
      PanelThumbnail.Size = new Size(118, 90);
      PanelThumbnail.TabIndex = 2;
      // 
      // LabelTitle
      // 
      LabelTitle.AutoSize = true;
      LabelTitle.Font = new Font("Microsoft JhengHei UI", 14F);
      LabelTitle.Location = new Point(123, 6);
      LabelTitle.Name = "LabelTitle";
      LabelTitle.Size = new Size(71, 24);
      LabelTitle.TabIndex = 3;
      LabelTitle.Text = "切割X1";
      // 
      // Labelsummary
      // 
      Labelsummary.AutoSize = true;
      Labelsummary.Location = new Point(126, 31);
      Labelsummary.Name = "Labelsummary";
      Labelsummary.Size = new Size(73, 15);
      Labelsummary.TabIndex = 4;
      Labelsummary.Text = "15%  10.2%";
      // 
      // Thumbnail
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(280, 93);
      Controls.Add(Labelsummary);
      Controls.Add(LabelTitle);
      Controls.Add(PanelThumbnail);
      FormBorderStyle = FormBorderStyle.None;
      Name = "Thumbnail";
      Text = "Thumbnail";
      Load += Thumbnail_Load;
      Click += Thumbnail_Click;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private Panel PanelThumbnail;
    private Label LabelTitle;
    private Label Labelsummary;
  }
}