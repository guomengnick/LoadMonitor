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
        Log.Information("Serial port initialized.");
      }
      catch (Exception ex)
      {
        Log.Error($"Error initializing serial port: {ex.Message}");
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
        Log.Warning("Serial port opened.");
      }
      catch (Exception ex)
      {
        Log.Error($"Error opening serial port: {ex.Message}");
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
    public Dictionary<int, double> ReadCurrents()
    {
      Dictionary<int, double> currents = new Dictionary<int, double>();
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
        // 實際發送的數據: 0x0103000000104406
        //                            ^ 10 代表要收16個byte, 1個通道用2個byte表示
        serial_port_.Write(requestFrame, 0, requestFrame.Length);



        /// 模擬接收到的數據幀///
        //byte[] responseFrame = new byte[]
        //{
        //    0x01, 0x03, 0x20, 0x00, 0x87, 0x00, 0x12, 0x00, 0x1E, 0x00, 0x00, 0x00, 0x23, 0x00, 0x4E, 0x00,
        //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        //    0x00, 0x00, 0x00, 0x8D, 0x0E
        //};
        //int bytesRead = 37;
        //實際收到的數據: 01032000000000000000000000000A000000000000000000000000000000000000000038D0
        //                   ^ 數據有0x20個(共32 個)byte, 一個通道用2個byte表示
        //                     ^ 0x0000表示第一個通道的值 比如0xFFFF 為 65535/100 = 65.535A 
        /// 模擬接收到的數據幀///


        // 接收數據幀
        byte[] responseFrame = new byte[37]; // 根據您提供的範例數據，應有 37 字節
        int bytesRead = serial_port_.Read(responseFrame, 0, responseFrame.Length);// 打印接收到的數據幀到日誌
        var hexResponse = BitConverter.ToString(responseFrame).Replace("-", " ");
        Log.Information($"Received Response Frame: {hexResponse}");


        if (bytesRead < 5) // 最少要有頭部（3字節）和校驗碼（2字節）
        {
          Log.Warning("Response too short or invalid.");
          return currents;
        }
        if (responseFrame[0] != 0x01 || responseFrame[1] != 0x03 || responseFrame[2] != 0x20)
        {
          return currents;
        }

        // 提取數據區域：去掉報文頭 (3字節) 和校驗碼 (2字節)
        int dataLength = bytesRead - 5;
        if (dataLength != 32) // 確保數據區域是 32 字節（16 通道，每通道 2 字節）
        {
          Log.Warning("Invalid data length.");
          return currents;
        }
        var s = "";
        double sum_current = 0;

        for (int i = 1; i <= 16; i++) // 從 1 開始，解析 16 次
        {
          int dataIndex = 3 + (i - 1) * 2; // 索引需要調整，i-1 對應原始的 0~15
          ushort registerValue = (ushort)((responseFrame[dataIndex] << 8) | responseFrame[dataIndex + 1]);

          // 計算實際電流值（寄存器值 / 100.0）
          double currentValue = registerValue / 100.0;
          currents[i] = currentValue; // 字典的鍵值對應通道號（從1開始）
          sum_current += currentValue;

          // 記錄到日誌
          s += $"通道{i}: {currentValue:00.00} A\n";
        }
        currents[0/*0 用來裝總電流*/] = sum_current;

        Log.Information($"總電流:{sum_current} A\n" + s + "\n\n");
      }
      catch (Exception ex)
      {
        Log.Error($"Error during Modbus communication: {ex.Message}");
      }
      return currents;
    }

  }
}
