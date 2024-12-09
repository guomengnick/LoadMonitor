namespace LoadMonitor
{
  partial class SerialPortFormTEST
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
      textBoxOutput = new TextBox();
      comboBoxPorts = new ComboBox();
      buttonConnect = new Button();
      buttonRead = new Button();
      SuspendLayout();
      // 
      // textBoxOutput
      // 
      textBoxOutput.Location = new Point(12, 12);
      textBoxOutput.Multiline = true;
      textBoxOutput.Name = "textBoxOutput";
      textBoxOutput.Size = new Size(1065, 382);
      textBoxOutput.TabIndex = 0;
      // 
      // comboBoxPorts
      // 
      comboBoxPorts.FormattingEnabled = true;
      comboBoxPorts.Location = new Point(733, 415);
      comboBoxPorts.Name = "comboBoxPorts";
      comboBoxPorts.Size = new Size(121, 23);
      comboBoxPorts.TabIndex = 1;
      // 
      // buttonConnect
      // 
      buttonConnect.Location = new Point(622, 415);
      buttonConnect.Name = "buttonConnect";
      buttonConnect.Size = new Size(105, 23);
      buttonConnect.TabIndex = 2;
      buttonConnect.Text = "buttonConnect";
      buttonConnect.UseVisualStyleBackColor = true;
      buttonConnect.Click += buttonConnect_Click_1;
      // 
      // buttonRead
      // 
      buttonRead.Location = new Point(873, 415);
      buttonRead.Name = "buttonRead";
      buttonRead.Size = new Size(94, 23);
      buttonRead.TabIndex = 3;
      buttonRead.Text = "buttonRead";
      buttonRead.UseVisualStyleBackColor = true;
      buttonRead.Click += buttonRead_Click_1;
      // 
      // SerialPortFormTEST
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1089, 513);
      Controls.Add(buttonRead);
      Controls.Add(buttonConnect);
      Controls.Add(comboBoxPorts);
      Controls.Add(textBoxOutput);
      Name = "SerialPortFormTEST";
      Text = "SerialPortForm";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private TextBox textBoxOutput;
    private ComboBox comboBoxPorts;
    private Button buttonConnect;
    private Button buttonRead;
  }
}