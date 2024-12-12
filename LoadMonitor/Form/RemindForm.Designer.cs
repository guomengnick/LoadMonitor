namespace LoadMonitor
{
  partial class RemindForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemindForm));
      OKButton = new Button();
      TextBoxRemindInfo = new TextBox();
      label1 = new Label();
      pictureBox1 = new PictureBox();
      label2 = new Label();
      ButtonResetRemind = new Button();
      ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
      SuspendLayout();
      // 
      // OKButton
      // 
      OKButton.Anchor = AnchorStyles.Bottom;
      OKButton.BackColor = SystemColors.Highlight;
      OKButton.FlatAppearance.BorderSize = 0;
      OKButton.FlatStyle = FlatStyle.Flat;
      OKButton.Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold);
      OKButton.ForeColor = SystemColors.Info;
      OKButton.Location = new Point(97, 227);
      OKButton.Name = "OKButton";
      OKButton.Size = new Size(146, 32);
      OKButton.TabIndex = 0;
      OKButton.Text = "關閉";
      OKButton.UseVisualStyleBackColor = false;
      OKButton.Click += OKButton_Click;
      // 
      // TextBoxRemindInfo
      // 
      TextBoxRemindInfo.BackColor = Color.FromArgb(64, 64, 64);
      TextBoxRemindInfo.BorderStyle = BorderStyle.None;
      TextBoxRemindInfo.Font = new Font("Microsoft JhengHei UI", 15F);
      TextBoxRemindInfo.ForeColor = SystemColors.Menu;
      TextBoxRemindInfo.Location = new Point(25, 63);
      TextBoxRemindInfo.Multiline = true;
      TextBoxRemindInfo.Name = "TextBoxRemindInfo";
      TextBoxRemindInfo.Size = new Size(471, 156);
      TextBoxRemindInfo.TabIndex = 1;
      // 
      // label1
      // 
      label1.BackColor = SystemColors.InfoText;
      label1.BorderStyle = BorderStyle.FixedSingle;
      label1.Location = new Point(6, 53);
      label1.Name = "label1";
      label1.Size = new Size(510, 2);
      label1.TabIndex = 2;
      // 
      // pictureBox1
      // 
      pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
      pictureBox1.Location = new Point(12, -1);
      pictureBox1.Name = "pictureBox1";
      pictureBox1.Size = new Size(60, 56);
      pictureBox1.TabIndex = 3;
      pictureBox1.TabStop = false;
      pictureBox1.Visible = false;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Font = new Font("Microsoft JhengHei UI", 18F, FontStyle.Bold);
      label2.Location = new Point(204, 9);
      label2.Name = "label2";
      label2.Size = new Size(128, 30);
      label2.TabIndex = 4;
      label2.Text = "Warnning";
      label2.Click += label2_Click;
      // 
      // ButtonResetRemind
      // 
      ButtonResetRemind.Anchor = AnchorStyles.Bottom;
      ButtonResetRemind.BackColor = SystemColors.Highlight;
      ButtonResetRemind.FlatAppearance.BorderSize = 0;
      ButtonResetRemind.FlatStyle = FlatStyle.Flat;
      ButtonResetRemind.Font = new Font("Microsoft JhengHei UI", 13F, FontStyle.Bold);
      ButtonResetRemind.ForeColor = SystemColors.ButtonHighlight;
      ButtonResetRemind.Location = new Point(276, 227);
      ButtonResetRemind.Name = "ButtonResetRemind";
      ButtonResetRemind.Size = new Size(146, 32);
      ButtonResetRemind.TabIndex = 5;
      ButtonResetRemind.Text = "清除警示並關閉";
      ButtonResetRemind.UseVisualStyleBackColor = false;
      ButtonResetRemind.Click += ButtonResetRemindClick;
      // 
      // RemindForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      BackColor = Color.FromArgb(64, 64, 64);
      ClientSize = new Size(520, 266);
      Controls.Add(ButtonResetRemind);
      Controls.Add(label2);
      Controls.Add(pictureBox1);
      Controls.Add(label1);
      Controls.Add(TextBoxRemindInfo);
      Controls.Add(OKButton);
      FormBorderStyle = FormBorderStyle.None;
      Name = "RemindForm";
      StartPosition = FormStartPosition.CenterParent;
      Text = "RemindForm";
      Load += RemindForm_Load;
      ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Button OKButton;
    private TextBox TextBoxRemindInfo;
    private Label label1;
    private PictureBox pictureBox1;
    private Label label2;
    private Button ButtonResetRemind;
  }
}