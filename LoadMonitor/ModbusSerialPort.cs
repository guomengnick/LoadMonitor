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
          ReadTimeout = 300,
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
    public Dictionary<int, double> ReadCurrents(bool is_test_mode = false)
    {
      if (is_test_mode)
      {
        var TEST_currents = new Dictionary<int, double>();
        var random = new Random();
        for(int i = 0; i < 30; i++)
        {
          TEST_currents[i] = random.NextDouble() * 1;
        }
        return TEST_currents;
      }

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
                    0x08,   // 0x10, // 寄存器長度低字節 (0x10:收16個通道  0x08:收8個通道) 
                    0x44, // CRC 低字節 (0x44)
                    0x0C   // CRC 高字節 (0x06收16個通道  0x0C:收8個通道)
      };// 發送請求幀
      try
      {
        serial_port_.Write(requestFrame, 0, requestFrame.Length);
        System.Threading.Thread.Sleep(200);
      }
      catch (Exception ex)
      {
        Log.Error($"Error during Modbus write: {ex.Message}");
      }

      Dictionary<int, double> currents = new Dictionary<int, double>();

      byte[] respond = ReadCompleteResponse(21);// 接收數據幀
      if (respond.Length != 21 || respond[0] != 0x01 || respond[1] != 0x03 || respond[2] != 0x10)
      {
        return currents;//TODO: 有時候第0個值會跑到最後一位，全部的數據往前一格
      }

      var s = "";
      double sum_current = 0;
      for (int i = 1; i <= 8; i++) // 從 1 開始，解析 8 個通道
      {
        int dataIndex = 3 + (i - 1) * 2; // 索引需要調整，i-1 對應原始的 0~7
        ushort registerValue = (ushort)((respond[dataIndex] << 8) | respond[dataIndex + 1]);

        double currentValue = registerValue / 100.0; // 計算實際電流值（寄存器值 / 100.0）
        currents[i] = currentValue; // 字典的鍵值對應通道號（從1開始）
        sum_current += currentValue;

        s += $"{i} : {currentValue:0.00} A\t";// 記錄到日誌
      }
      currents[0/*0 用來裝總電流*/] = sum_current;
      Log.Information($"總電流:{sum_current} A\n" + s + "\n\n");

      return currents;
    }



    // 接收並處理數據幀的方法
    byte[] ReadCompleteResponse(int expected_length)
    {
      List<byte> response_buffer = new List<byte>();
      byte[] temp_buffer = new byte[255];
      int total_bytes_read = 0;
      int re_read_max_time = 2;
      int current_read_time = 0;
      try
      {
        while (total_bytes_read < expected_length)
        {
          current_read_time++;
          int bytes_read = serial_port_.Read(temp_buffer, 0, temp_buffer.Length);

          string hexResponse = BitConverter.ToString(temp_buffer, 0, bytes_read).Replace("-", " ");
          Log.Information($"當前在 while 讀取到的長度:{bytes_read} Received Response: {hexResponse}");

          if (bytes_read > 0)
          {
            // 添加新接收的數據到緩衝區
            response_buffer.AddRange(temp_buffer.Take(bytes_read));
            total_bytes_read += bytes_read;

            // 檢查是否包含指定標頭數據
            byte[] header = { 0x01, 0x03, 0x10 };
            int headerIndex = response_buffer.FindIndex(0, response_buffer.Count, b => response_buffer.Skip(b).Take(header.Length).SequenceEqual(header));

            if (headerIndex != -1 && response_buffer.Count - headerIndex >= 21)
            {
              // 擷取標頭開始的21字節數據並更新 response_buffer
              response_buffer = response_buffer.Skip(headerIndex).Take(21).ToList();
              Log.Information("已找到指定標頭數據並擷取21字節。");
              break;
            }
          }

          // 如果收到的數據超過預期長度，跳出循環
          if (response_buffer.Count >= expected_length)
          {
            Log.Information($"嘗試 {current_read_time} 次，達到21長度，目前讀的長度:{response_buffer.Count}，跳出迴圈");
            break;
          }

          if(current_read_time >= re_read_max_time)
            break;

          System.Threading.Thread.Sleep(150);
        }

        // 修剪多餘的數據（去掉超過 expected_length 的部分）
        if (response_buffer.Count > expected_length)
        {
          response_buffer = response_buffer.Take(expected_length).ToList();
        }

        // 將數據轉換為陣列返回
        string totalhexResponse = BitConverter.ToString(response_buffer.ToArray()).Replace("-", " ");
        Log.Information($"最後數據長度:{response_buffer.Count} Received Response: {totalhexResponse}");

        return response_buffer.ToArray();
      }
      catch (TimeoutException)
      {
        Log.Warning("Modbus read timeout. No response from device.");
        return Array.Empty<byte>();
      }
      catch (Exception ex)
      {
        Log.Error($"Error during Modbus Read: {ex.Message}");
        return Array.Empty<byte>();
      }
    }




  }
}
