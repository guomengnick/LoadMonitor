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
  public partial class QuadGrid : Form
  {
    public QuadGrid()
    {
      InitializeComponent();
    }
    public void AddToPanel(UserControl left_top_form, UserControl left_down_form,
      UserControl right_top_form, UserControl right_down_form)
    {
      // 清空 panel1 的内容，避免控件叠加
      panel1.Controls.Clear();
      panel1.Controls.Add(left_top_form);
      panel2.Controls.Clear();
      panel2.Controls.Add(left_down_form);

      panel3.Controls.Clear();
      panel3.Controls.Add(right_top_form);
      panel4.Controls.Clear();
      panel4.Controls.Add(right_down_form);
      this.TopLevel = false;
      this.Dock = DockStyle.Fill;
    }
  }
}
