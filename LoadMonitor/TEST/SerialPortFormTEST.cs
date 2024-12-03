using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modbus;
using System.IO.Ports;
using Modbus.Device;
using Microsoft.Win32;



namespace LoadMonitor
{
  public partial class SerialPortFormTEST : Form
  {
    private SerialPort serial_port_;
    private IModbusSerialMaster modbus_master_;
    public Dictionary<int, ushort> register_values_ = new Dictionary<int, ushort>();
    public SerialPortFormTEST()
    {
      InitializeComponent();
      LoadAvailablePorts();
      buttonRead.Enabled = false; // 初始時禁用讀取按鈕
    }


    // 加載可用串口到下拉框
    private void LoadAvailablePorts()
    {
      var ports = SerialPort.GetPortNames(); // 獲取系統中的串口名稱
      comboBoxPorts.Items.Clear();
      comboBoxPorts.Items.AddRange(ports);
      if (ports.Length > 0)
      {
        comboBoxPorts.SelectedIndex = 0; // 預設選中第一個端口
      }
    }

    // 連接按鈕事件
    private void buttonConnect_Click(object sender, EventArgs e)
    {
      if (serial_port_ == null || !serial_port_.IsOpen)
      {
        ConnectToPort(comboBoxPorts.SelectedItem.ToString());
      }
      else
      {
        DisconnectPort();
      }
    }

    // 連接到串口
    private void ConnectToPort(string portName)
    {
      try
      {
        // 創建串口
        serial_port_ = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One)
        {
          ReadTimeout = 3000,
          WriteTimeout = 1000
        };

        //serial_port_.DataReceived += SerialPort_DataReceived;
        // 打開串口

        serial_port_.Open();

        // 創建 Modbus 主站
        modbus_master_ = ModbusSerialMaster.CreateRtu(serial_port_);

        buttonConnect.Text = "Disconnect";
        buttonRead.Enabled = true; // 啟用 "Read" 按鈕
        textBoxOutput.AppendText($"Connected to {portName}\r\n");
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Failed to connect: {ex.Message}");
      }
    }

    public void Log(string log)
    {
      textBoxOutput.AppendText(log + "\r\n");
    }
    // 斷開串口
    private void DisconnectPort()
    {
      try
      {
        modbus_master_?.Dispose();
        serial_port_?.Close();

        buttonConnect.Text = "Connect";
        buttonRead.Enabled = false; // 禁用讀取按鈕
        textBoxOutput.AppendText("Disconnected\r\n");
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Failed to disconnect: {ex.Message}");
      }
    }

    private void SendCustomModbusRequest()
    {
      try
      {
        byte[] requestFrame = new byte[]
        {
          1,  // 地址碼 (0x01)
          3,  // 功能碼 (0x03)
          0,  // 寄存器起始地址高字節 (0x00)
          0,  // 寄存器起始地址低字節 (0x00)
          0,  // 寄存器長度高字節 (0x00)
          16, // 寄存器長度低字節 (0x10)
          68, // CRC 低字節 (0x44)
          6   // CRC 高字節 (0x06)
        };

        // 發送數據幀
        serial_port_.Write(requestFrame, 0, requestFrame.Length);
        // 接收數據幀
        byte[] responseFrame = new byte[37]; // 1 + 1 + 1 + (16 * 2) + 2 = 37 bytes
        int bytesRead = 0;
        bytesRead = serial_port_.Read(responseFrame, 0, responseFrame.Length);
        // 驗證響應幀長度
        if (bytesRead < 5)
        {
          textBoxOutput.AppendText("Response too short or invalid\r\n");
          return;
        }

        // 顯示接收到的原始數據幀
        textBoxOutput.AppendText($"Response: {BitConverter.ToString(responseFrame, 0, bytesRead)}\r\n");

        // 解析數據區域（從第 3 字節開始）
        for (int i = 3, registerIndex = 0; i < bytesRead - 2; i += 2, registerIndex++)
        {
          // 高字節在前，低字節在後
          ushort registerValue = (ushort)((responseFrame[i] << 8) | responseFrame[i + 1]);

          // 存入字典
          register_values_[registerIndex] = registerValue;

          // 顯示寄存器值
          textBoxOutput.AppendText($"Register {registerIndex}: {registerValue}\r\n");
        }
      }
      catch (Exception ex)
      {
        textBoxOutput.AppendText($"Error: {ex.Message}\r\n");
      }
    }




    // 發送讀取命令按鈕事件
    private void buttonRead_Click(object sender, EventArgs e)
    {
    }

    // 窗口關閉事件處理
    private void ModbusForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      DisconnectPort();
    }

    private void buttonConnect_Click_1(object sender, EventArgs e)
    {
      buttonConnect_Click(sender, e);
    }

    private void buttonRead_Click_1(object sender, EventArgs e)
    {
      SendCustomModbusRequest();
    }
  }
}
