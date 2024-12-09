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
    public ModbusSerialPort(string portName)
    {
      Initialize(portName);
      Open();
    }


    private SerialPort serial_port_;

    public bool IsConnected => serial_port_?.IsOpen ?? false;

    // 初始化串口
    private void Initialize(string portName)
    {
      int baudRate = 9600, dataBits = 8;
      Parity parity = Parity.None;
      StopBits stopBits = StopBits.One;

      try
      {
        serial_port_ = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
        {
          ReadTimeout = 1000,
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
    private void Open()
    {
      try
      {
        if (serial_port_ == null)
        {
          throw new InvalidOperationException("Serial port is not initialized.");
        }

        // 如果埠已經開啟，先關閉並釋放資源
        if (serial_port_.IsOpen)
        {
          Log.Warning($"Serial port {serial_port_.PortName} is already open. Attempting to close and reopen.");
          serial_port_.Close();
          serial_port_.Dispose(); // 釋放現有資源
          serial_port_ = new SerialPort(serial_port_.PortName, serial_port_.BaudRate, serial_port_.Parity, serial_port_.DataBits, serial_port_.StopBits)
          {
            ReadTimeout = serial_port_.ReadTimeout,
            WriteTimeout = serial_port_.WriteTimeout
          };
        }

        // 嘗試打開串口
        serial_port_.Open();
        Log.Information($"Serial port {serial_port_.PortName} opened.");

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
      if (serial_port_ == null || !serial_port_.IsOpen)
      {
        throw new InvalidOperationException("Serial port is not open.");
      }

      // 構造 Modbus 請求幀
      byte[] requestFrame = new byte[]
      {
                    0x01,  // 地址碼 (0x01)
                    0x03,  // 功能碼 (0x03)
                    0x00,  // 寄存器起始地址高字節 (0x00)
                    0x00,  // 寄存器起始地址低字節 (0x00)
                    0x00,  // 寄存器長度高字節 (0x00)
                    0x10, // 寄存器長度低字節 (0x10)
                    0x44, // CRC 低字節 (0x44)
                    0x06   // CRC 高字節 (0x06)
      };// 發送請求幀
        // 實際發送的數據: 0x0103000000104406
        //                            ^ 10 代表要收16個byte, 1個通道用2個byte表示
      try
      {
        serial_port_.Write(requestFrame, 0, requestFrame.Length);
      }
      catch (Exception ex)
      {
        Log.Error($"Error during Modbus write: {ex.Message}");
      }


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


      Dictionary<int, double> currents = new Dictionary<int, double>();
      // 接收數據幀
      byte[] responseFrame = new byte[37]; // 根據您提供的範例數據，應有 37 字節
      int bytesRead = 0;
      try
      {
        bytesRead = serial_port_.Read(responseFrame, 0, 37);// 打印接收到的數據幀到日誌
      }
      catch (TimeoutException)
      {
        Log.Warning("Modbus read timeout. No response from device.");
        return currents;
      }
      catch (Exception ex)
      {
        Log.Error($"Error during Modbus Read: {ex.Message}");
      }

      var hexResponse = BitConverter.ToString(responseFrame).Replace("-", " ");
      Log.Information($"Received Response Frame: {hexResponse}");


      if (bytesRead < 5) // 最少要有頭部（3字節）和校驗碼（2字節）
      {
        //Log.Warning("Response too short or invalid.");
        return currents;
      }
      if (responseFrame[0] == 0x03 && responseFrame[1] == 0x20)
      {
        // 將數據向後移動一位（從末尾開始防止覆蓋）
        for (int i = responseFrame.Length - 1; i > 0; i--)
        {
          responseFrame[i] = responseFrame[i - 1];
        }

        // 設置第一個字節為 0x01
        responseFrame[0] = 0x01;
      }


      if (responseFrame[0] != 0x01 || responseFrame[1] != 0x03 || responseFrame[2] != 0x20)
      {
        return currents;//TODO: 有時候第0個值會跑到最後一位，全部的數據往前一格
      }

      // 提取數據區域：去掉報文頭 (3字節) 和校驗碼 (2字節)
      int dataLength = bytesRead - 5;
      if (dataLength != 32) // 確保數據區域是 32 字節（16 通道，每通道 2 字節）
      {
        //Log.Warning("Invalid data length.");
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
        if(i <= 8)
        {
          s += $"通道{i}: {currentValue:00.00} A\n";
        }
      }
      currents[0/*0 用來裝總電流*/] = sum_current;

      Log.Information($"總電流:{sum_current} A\n" + s + "\n\n");

      // 最後清空緩衝區
      //serial_port_.DiscardInBuffer();
      //serial_port_.DiscardOutBuffer();
      return currents;
    }

  }
}
