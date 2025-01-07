namespace LoadMonitor
{
  partial class LeftRight
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
      panel1 = new Panel();
      panel2 = new Panel();
      PanelText = new Panel();
      LeftTextBoxDetail = new TextBox();
      RightTextBoxDetail = new TextBox();
      PanelText.SuspendLayout();
      SuspendLayout();
      // 
      // panel1
      // 
      panel1.Location = new Point(2, 3);
      panel1.Name = "panel1";
      panel1.Size = new Size(495, 565);
      panel1.TabIndex = 0;
      // 
      // panel2
      // 
      panel2.Location = new Point(502, 2);
      panel2.Name = "panel2";
      panel2.Size = new Size(495, 565);
      panel2.TabIndex = 1;
      // 
      // PanelText
      // 
      PanelText.Controls.Add(LeftTextBoxDetail);
      PanelText.Controls.Add(RightTextBoxDetail);
      PanelText.Location = new Point(2, 570);
      PanelText.Name = "PanelText";
      PanelText.Size = new Size(990, 151);
      PanelText.TabIndex = 6;
      // 
      // LeftTextBoxDetail
      // 
      LeftTextBoxDetail.BackColor = SystemColors.Control;
      LeftTextBoxDetail.BorderStyle = BorderStyle.None;
      LeftTextBoxDetail.Enabled = false;
      LeftTextBoxDetail.Font = new Font("Microsoft JhengHei UI", 14F);
      LeftTextBoxDetail.Location = new Point(7, 7);
      LeftTextBoxDetail.Multiline = true;
      LeftTextBoxDetail.Name = "LeftTextBoxDetail";
      LeftTextBoxDetail.ReadOnly = true;
      LeftTextBoxDetail.Size = new Size(484, 140);
      LeftTextBoxDetail.TabIndex = 3;
      LeftTextBoxDetail.Text = "aaaaaaaaaaaaaaaaaaa\r\niiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii\r\nBBBBBBBB\r\nCCCCCCC\r\nDDDDDD\r\nEEEEEE\r\nFFFFF";
      // 
      // RightTextBoxDetail
      // 
      RightTextBoxDetail.BackColor = SystemColors.Control;
      RightTextBoxDetail.BorderStyle = BorderStyle.None;
      RightTextBoxDetail.Enabled = false;
      RightTextBoxDetail.Font = new Font("Microsoft JhengHei UI", 14F);
      RightTextBoxDetail.Location = new Point(497, 7);
      RightTextBoxDetail.Multiline = true;
      RightTextBoxDetail.Name = "RightTextBoxDetail";
      RightTextBoxDetail.Size = new Size(490, 140);
      RightTextBoxDetail.TabIndex = 4;
      RightTextBoxDetail.Text = "RRRRR\r\nIIIII\r\nGGGGG\r\nHHHH\r\nTTTTT\r\nGGGGG";
      // 
      // LeftRight
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1000, 720);
      Controls.Add(PanelText);
      Controls.Add(panel2);
      Controls.Add(panel1);
      FormBorderStyle = FormBorderStyle.None;
      Name = "LeftRight";
      Text = "LeftRight";
      PanelText.ResumeLayout(false);
      PanelText.PerformLayout();
      ResumeLayout(false);
    }

    #endregion

    private Panel panel1;
    private Panel panel2;
    private Panel PanelText;
    private TextBox LeftTextBoxDetail;
    private TextBox RightTextBoxDetail;
  }
}