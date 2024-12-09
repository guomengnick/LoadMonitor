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
    public RemindForm(string warnning_info)
    {
      InitializeComponent();
      this.TextBoxRemindInfo.Text = warnning_info;
    }

    private void RemindForm_Load(object sender, EventArgs e)
    {

    }

    private void label2_Click(object sender, EventArgs e)
    {

    }

    private void OKButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void TextBoxRemindInfo_TextChanged(object sender, EventArgs e)
    {

    }
  }
}
