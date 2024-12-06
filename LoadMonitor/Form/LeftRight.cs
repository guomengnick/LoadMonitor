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
  public partial class LeftRight : Form
  {
    public LeftRight()
    {
      InitializeComponent();
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

    public void AddToPanel(UserControl left_form, UserControl right_top_form,
       string left_text, string right_text)
    {
      // 清空 panel1 的内容，避免控件叠加
      panel1.Controls.Clear();
      panel1.Controls.Add(left_form);
      panel2.Controls.Clear();
      panel2.Controls.Add(right_top_form);

      // 更新左右兩邊的 TextBox
      UpdateText(left_text, right_text);

      this.TopLevel = false;
      this.Dock = DockStyle.Fill;
    }
  }
}
