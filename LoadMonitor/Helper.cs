using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMonitor
{

  public static class Helper
  {
    public static void ShowRestartDialog(string title, string message)
    {
      DialogResult result = MessageBox.Show(
          message, title,
          MessageBoxButtons.YesNo, MessageBoxIcon.Question
      );

      if (result == DialogResult.Yes)
      {
        Settings.Default.Save();
        Application.Restart();
        Environment.Exit(0);
      }
    }
  }
}
