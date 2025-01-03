using Serilog;
using Serilog.Sinks.File;
using System.Windows.Forms;
using LoadMonitor.Components;

namespace LoadMonitor
{
  internal static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      // 註冊全域退出事件
      //Application.ApplicationExit += Application_Exit;

      // 初始化 Serilog，僅輸出到 Log 資料夾下的文件
      Log.Logger = new LoggerConfiguration()
          .WriteTo.File("Log\\application.log", // 日誌文件路徑
                        rollingInterval: RollingInterval.Day, // 每日滾動新文件
                        retainedFileCountLimit: 7, // 保留最近 7 天的日誌文件
                        fileSizeLimitBytes: 10 * 1024 * 1024, // 單個日誌文件大小限制（10 MB）
                        rollOnFileSizeLimit: true) // 文件過大時滾動
          .CreateLogger();

      int machineTypeValue = 0; // 預設值
      MachineType machineTypeEnum = (MachineType)Settings.Default.MachineType;

      try
      {
        if (args.Length > 0)
        {
          // 如果有傳入的參數, 將傳入的參數轉換成 int
          machineTypeValue = int.Parse(args[0]);

          // 檢查 int 是否對應到 MachineType 枚舉
          if (Enum.IsDefined(typeof(MachineType), machineTypeValue))
          {
            machineTypeEnum = (MachineType)machineTypeValue;
          }
          else
          {
            Log.Warning($"無效的參數值: {machineTypeValue}");
          }
          // 儲存 MachineType 到設定檔
          Settings.Default.MachineType = (int)machineTypeEnum;
          Settings.Default.Save();

          // 紀錄日誌
          Log.Information($"傳入的參數: {args[0]}, 轉換後的 MachineType: {machineTypeEnum}");
        }

        var is_spindle_show = 0;//主軸不顯示
        //if (args.Length > 1)
        {//有傳入主軸是否啟動
          // 如果有傳入的參數, 將傳入的參數轉換成 int
          //is_spindle_show = int.Parse(args[1]);
          is_spindle_show = 1;
          // 紀錄日誌
          //Log.Information($"傳入的主軸參數為: {args[1]}, 1為顯示主軸部件，0為不顯示");
        }

        // 初始化應用程序
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm(machineTypeEnum, is_spindle_show));
        //Application.Run(new SerialPortFormTEST());
        //Application.Run(new TEST_Comm());
      }
      catch (Exception ex)
      {
        // 捕捉並記錄未處理的異常
        Log.Fatal(ex, "The application failed to start correctly.");
      }
      finally
      {
        // 確保日誌緩衝區內容寫入文件
        Log.CloseAndFlush();
      }
    }
  }
}

