using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoadMonitor
{
  public partial class RemindForm : Form
  {
    private bool reset_reminder_;
    public RemindForm(string warnning_info)
    {
      InitializeComponent();
      this.TextBoxRemindInfo.Text = warnning_info;
      this.ButtonResetRemind.Text = Language.GetString("清除警示並關閉");
      this.OKButton.Text = Language.GetString("關閉");

      Show();// 確保調用此form時, 不發生"視窗控制代碼建立後才能呼叫控制項上"
      Hide();
    }

    private void RemindForm_Load(object sender, EventArgs e)
    {

    }

    private void label2_Click(object sender, EventArgs e)
    {

    }

    private void OKButton_Click(object sender, EventArgs e)
    {
      reset_reminder_ = false;
      this.Close();
    }

    private void ButtonResetRemindClick(object sender, EventArgs e)
    {
      reset_reminder_ = true;
      this.Close();
    }

    public bool ShowDialogInfo()
    {
      this.ShowDialog();//顯示對話UI框
      return reset_reminder_;
    }
  }
}
