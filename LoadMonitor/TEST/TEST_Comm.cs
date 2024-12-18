using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;
using Log = Serilog.Log;

namespace LoadMonitor
{
  public partial class TEST_Comm : Form
  {
    public TEST_Comm()
    {
      InitializeComponent();
      Console.WriteLine("Named Pipe Server is starting...");
    }

    public void button1_Click(object? sender, EventArgs e)
    {
      StartPipeServer();

    }

    private const string PipeName = "LoadMonitorPipe";


    static void StartPipeServer(string publish_msg = "")
    {
      while (true)
      {
        try
        {
          using (var pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.Out))
          {
            Log.Information("Waiting for client connection...");
            pipeServer.WaitForConnection(); // 等待客户端连接

            // 模拟发送通知
            string message = new System.Random().Next(1, 10).ToString(); // 通知内容，"1" 表示有通知

            if (publish_msg != "")
            {
              message = publish_msg;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(message);
            pipeServer.Write(buffer, 0, buffer.Length);
            pipeServer.Flush();

            Log.Information($"Sent message: {message}");

          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Error: {ex.Message}");
        }
      }
    }



  }
}
