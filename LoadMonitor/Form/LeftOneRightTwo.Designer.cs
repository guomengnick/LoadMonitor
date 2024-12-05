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
      // LeftOneRightTwo
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1000, 570);
      Controls.Add(panel3);
      Controls.Add(panel2);
      Controls.Add(panel1);
      FormBorderStyle = FormBorderStyle.None;
      Name = "LeftOneRightTwo";
      Text = "TwoRow";
      ResumeLayout(false);
    }

    #endregion

    private Panel panel1;
    private Panel panel2;
    private Panel panel3;
  }
}