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

    }
    
    public void AddToPanel(UserControl form)
    {

      // 清空 panel1 的内容，避免控件叠加
      panel1.Controls.Clear();
      panel1.Controls.Add(form);
      this.TopLevel = false;
      this.Dock = DockStyle.Fill;
    }


  }
}
