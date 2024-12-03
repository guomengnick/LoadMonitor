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
  public partial class SerialPortForm : Form
  {
    private SerialPort serial_port_;
    private IModbusSerialMaster modbus_master_;

    public SerialPortForm()
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
          ReadTimeout = 1000,
          WriteTimeout = 1000
        };

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
        // 手動構造 Modbus 數據幀
        byte[] requestFrame = new byte[]
        {
            0x01,       // Slave ID
            0x03,       // Function Code: Read Holding Registers
            0x00, 0x00, // Start Address: 0
            0x00, 0x10, // Quantity of Registers: 16
            0x44, 0x06  // CRC
        };

        // 發送數據幀
        serial_port_.Write(requestFrame, 0, requestFrame.Length);
        // 接收數據幀
        byte[] responseFrame = new byte[37]; // 1 + 1 + 1 + (16 * 2) + 2 = 37 bytes
        int bytesRead = serial_port_.Read(responseFrame, 0, responseFrame.Length);

        // 驗證響應幀長度
        if (bytesRead < 5)
        {
          textBoxOutput.AppendText("Response too short or invalid\r\n");
          return;
        }

        // 顯示接收到的原始數據幀
        textBoxOutput.AppendText($"Response: {BitConverter.ToString(responseFrame, 0, bytesRead)}\r\n");

        // 解析數據區域（從第 3 字節開始）
        for (int i = 3; i < bytesRead - 2; i += 2)
        {
          ushort registerValue = (ushort)((responseFrame[i] << 8) | responseFrame[i + 1]);
          textBoxOutput.AppendText($"Register Value: {registerValue}\r\n");
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
      try
      {
        byte slave_id_ = 1; // 設置從機 ID
        ushort start_address_ = 0; // 起始寄存器地址
        ushort num_registers_ = 16; // 要讀取的寄存器數量
        textBoxOutput.AppendText($"開始讀\r\n");
        // 發送 Modbus 命令
        ushort[] registers = modbus_master_.ReadHoldingRegisters(slave_id_, start_address_, num_registers_);
        
        textBoxOutput.AppendText($"讀完\r\n");
        textBoxOutput.AppendText($"registers 長度{registers.Length}\r\n");

        // 顯示數據
        for (int i = 0; i < registers.Length; i++)
        {
          textBoxOutput.AppendText($"Register {start_address_ + i}: {registers[i]}\r\n");
        }
      }
      catch (Exception ex)
      {
        textBoxOutput.AppendText($"Error: {ex.Message}\r\n");
      }
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
      //buttonRead_Click(sender, e);
    }
  }
}
