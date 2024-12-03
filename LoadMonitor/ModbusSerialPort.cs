using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Log = Serilog.Log;

namespace LoadMonitor
{
  internal class ModbusSerialPort
  {
    private SerialPort serial_port_;

    public bool IsConnected => serial_port_?.IsOpen ?? false;

    // 初始化串口
    public void Initialize(string portName)
    {
      int baudRate = 9600, dataBits = 8;
      Parity parity = Parity.None;
      StopBits stopBits = StopBits.One;

      try
      {
        serial_port_ = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
        {
          ReadTimeout = 3000,
          WriteTimeout = 1000
        };
        Console.WriteLine("Serial port initialized.");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error initializing serial port: {ex.Message}");
      }
    }

    // 打開串口
    public void Open()
    {
      try
      {
        if (serial_port_ == null)
        {
          throw new InvalidOperationException("Serial port is not initialized.");
        }

        serial_port_.Open();
        Console.WriteLine("Serial port opened.");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error opening serial port: {ex.Message}");
      }
    }

    // 關閉串口
    public void Close()
    {
      try
      {
        serial_port_?.Close();
        Console.WriteLine("Serial port closed.");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error closing serial port: {ex.Message}");
      }
    }

    // 發送 Modbus 請求並讀取回應
    public Dictionary<int, ushort> ReadCurrents()
    {
      Dictionary<int, ushort> currents = new Dictionary<int, ushort>();
      try
      {
        if (serial_port_ == null || !serial_port_.IsOpen)
        {
          throw new InvalidOperationException("Serial port is not open.");
        }

        // 構造 Modbus 請求幀
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
        };// 發送請求幀
        serial_port_.Write(requestFrame, 0, requestFrame.Length);
        Console.WriteLine("Request sent.");
        // 接收數據幀
        byte[] responseFrame = new byte[37]; // 根據您提供的範例數據，應有 37 字節
        int bytesRead = serial_port_.Read(responseFrame, 0, responseFrame.Length);

        if (bytesRead < 5) // 最少要有頭部（3字節）和校驗碼（2字節）
        {
          //Log.Warning("Response too short or invalid.");
          return currents;
        }

        // 提取數據區域：去掉報文頭 (3字節) 和校驗碼 (2字節)
        int dataLength = bytesRead - 5;
        if (dataLength != 32) // 確保數據區域是 32 字節（16 通道，每通道 2 字節）
        {
          //Log.Warning("Invalid data length.");
          return currents;
        }
        var s = "";
        for (int i = 0; i < 16; i++) // 每個通道 2 字節，解析 16 次
        {
          int dataIndex = 3 + i * 2; // 數據區起始於索引 3，每通道占 2 字節
          ushort registerValue = (ushort)((responseFrame[dataIndex] << 8) | responseFrame[dataIndex + 1]);
          currents[i] = registerValue;

          // 計算實際電流值（寄存器值 / 100.0）
          double currentValue = registerValue / 100.0;

          // 記錄到日誌
          s += $"通道{i + 1}電流: {currentValue:00.00} A\n";
        }
        Log.Information(s + "\n\n");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error during Modbus communication: {ex.Message}");
      }
      return currents;
    }
  }
}
