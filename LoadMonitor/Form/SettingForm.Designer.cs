﻿namespace LoadMonitor
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
      ButtonSaveAndClose = new Button();
      ButtonReset = new Button();
      flowLayoutPanel1.SuspendLayout();
      SuspendLayout();
      // 
      // flowLayoutPanel1
      // 
      flowLayoutPanel1.AutoScroll = true;
      flowLayoutPanel1.Controls.Add(panel1);
      flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
      flowLayoutPanel1.Location = new Point(64, 12);
      flowLayoutPanel1.Name = "flowLayoutPanel1";
      flowLayoutPanel1.Size = new Size(553, 597);
      flowLayoutPanel1.TabIndex = 0;
      flowLayoutPanel1.WrapContents = false;
      // 
      // panel1
      // 
      panel1.BackColor = Color.Transparent;
      panel1.Location = new Point(3, 3);
      panel1.Name = "panel1";
      panel1.Size = new Size(515, 20);
      panel1.TabIndex = 0;
      // 
      // ButtonSaveAndClose
      // 
      ButtonSaveAndClose.Location = new Point(352, 615);
      ButtonSaveAndClose.Name = "ButtonSaveAndClose";
      ButtonSaveAndClose.Size = new Size(115, 28);
      ButtonSaveAndClose.TabIndex = 2;
      ButtonSaveAndClose.Text = "儲存";
      ButtonSaveAndClose.UseVisualStyleBackColor = true;
      // 
      // ButtonReset
      // 
      ButtonReset.Location = new Point(214, 615);
      ButtonReset.Name = "ButtonReset";
      ButtonReset.Size = new Size(115, 28);
      ButtonReset.TabIndex = 3;
      ButtonReset.Text = "恢復預設";
      ButtonReset.UseVisualStyleBackColor = true;
      ButtonReset.Click += ButtonReset_Click;
      // 
      // SettingForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      BackColor = SystemColors.ControlDark;
      ClientSize = new Size(675, 654);
      Controls.Add(ButtonReset);
      Controls.Add(ButtonSaveAndClose);
      Controls.Add(flowLayoutPanel1);
      FormBorderStyle = FormBorderStyle.FixedToolWindow;
      Name = "SettingForm";
      Text = "Settings";
      flowLayoutPanel1.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion

    public FlowLayoutPanel flowLayoutPanel1;
    private Button ButtonSaveAndClose;
    private Panel panel1;
    private Button ButtonReset;
  }
}