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

      foreach (string port in available_ports)
      {
        ToolStripMenuItem port_item = new ToolStripMenuItem(port)
        {
          Checked = port == Settings.Default.ComPort, // 初始化時選中當前設定的 COM 埠
          Enabled = port != Settings.Default.ComPort // 如果是當前選定的埠，禁用選項
        };

        port_item.Click += (sender, e) =>
        {
          if (sender is not ToolStripMenuItem selected_item ||
              selected_item.Text == Settings.Default.ComPort)
          {
            return;
          }

          // 更新設定
          string selected_port = selected_item.Text;
          Settings.Default.ComPort = selected_port;
          Settings.Default.Save();

          // 更新所有選項的狀態
          foreach (ToolStripMenuItem item in com_port_menu_item.DropDownItems.OfType<ToolStripMenuItem>())
          {
            item.Checked = item == selected_item;
            item.Enabled = item != selected_item;
          }

          // 顯示切換彈窗
          string message = string.Format(
              Language.GetString("切換COM口彈窗內文"),
              selected_port
          );
          Helper.ShowRestartDialog($"{Language.GetString("是否重啟")}", message);

          // 初始化 Modbus 埠
          InitializeModbusPort(selected_port);
        };

        com_port_menu_item.DropDownItems.Add(port_item);
      }

      // 如果當前 COM 埠存在，初始化 Modbus 埠
      if (!string.IsNullOrEmpty(Settings.Default.ComPort))
      {
        InitializeModbusPort(Settings.Default.ComPort);
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
