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
  public partial class Single : Form
  {
    public Single()
    {
      InitializeComponent();
      Show();// 確保調用此form時, 不發生"視窗控制代碼建立後才能呼叫控制項上"
      Hide();
    }

    public void AddToPanel(UserControl form, string left_text, string right_text)
    {

      if (!IsHandleCreated)
      {
        CreateControl();
      }
      // 清空 panel1 的内容，避免控件叠加
      panel1.Controls.Clear();
      panel1.Controls.Add(form);

      // 更新左右兩邊的 TextBox
      UpdateText(left_text, right_text);

      this.TopLevel = false;
      this.Dock = DockStyle.Fill;
    }

    // 提供更新 TextBox 内容的方法
    public void UpdateText(string left_text, string right_text)
    {
      // 使用 Invoke 確保在主線程更新 UI
      if (LeftTextBoxDetail.InvokeRequired)
      {
        LeftTextBoxDetail.Invoke(new Action(() =>
        {
          LeftTextBoxDetail.Text = left_text;
          RightTextBoxDetail.Text = right_text;
        }));
      }
      else
      {
        LeftTextBoxDetail.Text = left_text;
        RightTextBoxDetail.Text = right_text;
      }

    }
  }
}
