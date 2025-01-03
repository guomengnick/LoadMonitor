namespace LoadMonitor
{
  partial class SettingForm
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
      flowLayoutPanel1 = new FlowLayoutPanel();
      panel1 = new Panel();
      button2 = new Button();
      button3 = new Button();
      flowLayoutPanel1.SuspendLayout();
      SuspendLayout();
      // 
      // flowLayoutPanel1
      // 
      flowLayoutPanel1.AutoScroll = true;
      flowLayoutPanel1.Controls.Add(panel1);
      flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      flowLayoutPanel1.Location = new Point(12, 12);
      flowLayoutPanel1.Name = "flowLayoutPanel1";
      flowLayoutPanel1.Size = new Size(403, 475);
      flowLayoutPanel1.TabIndex = 0;
      flowLayoutPanel1.WrapContents = false;
      // 
      // panel1
      // 
      panel1.BackColor = Color.Transparent;
      panel1.Location = new Point(3, 3);
      panel1.Name = "panel1";
      panel1.Size = new Size(374, 15);
      panel1.TabIndex = 0;
      // 
      // button2
      // 
      button2.Location = new Point(232, 493);
      button2.Name = "button2";
      button2.Size = new Size(89, 27);
      button2.TabIndex = 2;
      button2.Text = "儲存並關閉";
      button2.UseVisualStyleBackColor = true;
      button2.Click += button2_Click;
      // 
      // button3
      // 
      button3.Location = new Point(98, 493);
      button3.Name = "button3";
      button3.Size = new Size(89, 27);
      button3.TabIndex = 3;
      button3.Text = "回覆預設";
      button3.UseVisualStyleBackColor = true;
      // 
      // SettingForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(424, 523);
      Controls.Add(button3);
      Controls.Add(button2);
      Controls.Add(flowLayoutPanel1);
      FormBorderStyle = FormBorderStyle.FixedToolWindow;
      Name = "SettingForm";
      Text = "SettingForm";
      flowLayoutPanel1.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion

    public FlowLayoutPanel flowLayoutPanel1;
    private Button button2;
    private Panel panel1;
    private Button button3;
  }
}