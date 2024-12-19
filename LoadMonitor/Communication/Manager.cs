using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMonitor.Communication
{

  internal class Manager
  {
    private ModbusSerialPort modbus_serial_port_;

    private readonly Action update_action_;

    public Manager(ToolStripMenuItem com_port_menu_item)
    {
      InitializeComPorts(com_port_menu_item);
    }


    public void InitializeComPorts(ToolStripMenuItem com_port_menu_item)
    {
      com_port_menu_item.DropDownItems.Clear();
      List<string> available_ports = System.IO.Ports.SerialPort.GetPortNames().ToList();

      available_ports.AddRange(new[] { "COM2", "COM3", "COM4" });

      foreach (string port in available_ports)
      {
        ToolStripMenuItem port_item = new ToolStripMenuItem(port);
        port_item.Click += (sender, e) =>
        {
          if (sender is ToolStripMenuItem selected_item &&
            selected_item.Text != Settings.Default.ComPort)
          {
            string selected_port = selected_item.Text;
            Settings.Default.ComPort = selected_port;
            Settings.Default.Save();

            // 用以下方式把文字檔內的 '{0}' 換成 'selected_port'
            string message = string.Format(
                Language.GetString("切換COM口彈窗內文"),
                selected_port
            );
            Helper.ShowRestartDialog($"{Language.GetString("是否重啟")}", message);
          }
        };

        com_port_menu_item.DropDownItems.Add(port_item);
      }

      if (!string.IsNullOrEmpty(Settings.Default.ComPort))
      {
        InitializeModbusPort(Settings.Default.ComPort);
        // 把預設值，對combo box更新
        var selectedItem = com_port_menu_item.DropDownItems
          .OfType<ToolStripMenuItem>()
          .FirstOrDefault(item => item.Text == Settings.Default.ComPort);

        if (selectedItem != null)
        {
          selectedItem.Checked = true;
          selectedItem.Enabled = false;
        }

      }
    }

    private void InitializeModbusPort(string port)
    {
      modbus_serial_port_ = new ModbusSerialPort(port);
    }

    public Dictionary<int, double> ReadCurrents(bool is_test_mode = false)
    {
      return modbus_serial_port_.ReadCurrents(is_test_mode);
    }

    public bool IsConnected()
    {
      return modbus_serial_port_.IsConnected;
    }
  }
}
