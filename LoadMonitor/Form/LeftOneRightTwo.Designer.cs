namespace LoadMonitor
{
  partial class LeftOneRightTwo
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
      panel3 = new Panel();
      LeftTextBoxDetail = new TextBox();
      RightTextBoxDetail = new TextBox();
      PanelText = new Panel();
      PanelText.SuspendLayout();
      SuspendLayout();
      // 
      // panel1
      // 
      panel1.Location = new Point(5, 5);
      panel1.Name = "panel1";
      panel1.Size = new Size(697, 563);
      panel1.TabIndex = 0;
      // 
      // panel2
      // 
      panel2.Location = new Point(706, 5);
      panel2.Name = "panel2";
      panel2.Size = new Size(291, 278);
      panel2.TabIndex = 1;
      // 
      // panel3
      // 
      panel3.Location = new Point(705, 289);
      panel3.Name = "panel3";
      panel3.Size = new Size(291, 278);
      panel3.TabIndex = 2;
      // 
      // LeftTextBoxDetail
      // 
      LeftTextBoxDetail.BackColor = SystemColors.Control;
      LeftTextBoxDetail.BorderStyle = BorderStyle.None;
      LeftTextBoxDetail.Font = new Font("Microsoft JhengHei UI", 14F);
      LeftTextBoxDetail.Location = new Point(7, 7);
      LeftTextBoxDetail.Multiline = true;
      LeftTextBoxDetail.Name = "LeftTextBoxDetail";
      LeftTextBoxDetail.Size = new Size(484, 145);
      LeftTextBoxDetail.TabIndex = 3;
      LeftTextBoxDetail.Text = "aaaaaaaaaaaaaaaaaaa\r\niiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii\r\nBBBBBBBB\r\nCCCCCCC\r\nDDDDDD\r\nEEEEEE\r\nFFFFF";
      // 
      // RightTextBoxDetail
      // 
      RightTextBoxDetail.BackColor = SystemColors.Control;
      RightTextBoxDetail.BorderStyle = BorderStyle.None;
      RightTextBoxDetail.Font = new Font("Microsoft JhengHei UI", 14F);
      RightTextBoxDetail.Location = new Point(497, 7);
      RightTextBoxDetail.Multiline = true;
      RightTextBoxDetail.Name = "RightTextBoxDetail";
      RightTextBoxDetail.Size = new Size(490, 146);
      RightTextBoxDetail.TabIndex = 4;
      RightTextBoxDetail.Text = "RRRRR\r\nIIIII\r\nGGGGG\r\nHHHH\r\nTTTTT\r\nGGGGG";
      // 
      // PanelText
      // 
      PanelText.Controls.Add(LeftTextBoxDetail);
      PanelText.Controls.Add(RightTextBoxDetail);
      PanelText.Location = new Point(5, 569);
      PanelText.Name = "PanelText";
      PanelText.Size = new Size(990, 155);
      PanelText.TabIndex = 5;
      // 
      // LeftOneRightTwo
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1000, 727);
      Controls.Add(PanelText);
      Controls.Add(panel3);
      Controls.Add(panel2);
      Controls.Add(panel1);
      FormBorderStyle = FormBorderStyle.None;
      Name = "LeftOneRightTwo";
      Text = "TwoRow";
      PanelText.ResumeLayout(false);
      PanelText.PerformLayout();
      ResumeLayout(false);
    }

    #endregion

    private Panel panel1;
    private Panel panel2;
    private Panel panel3;
    private TextBox LeftTextBoxDetail;
    private TextBox RightTextBoxDetail;
    private Panel PanelText;
  }
}