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
  public partial class DetailTextForm : Form
  {
    public DetailTextForm()
    {
      InitializeComponent();
      Show();// 確保調用此form時, 不發生"視窗控制代碼建立後才能呼叫控制項上"
    }
  }
}
