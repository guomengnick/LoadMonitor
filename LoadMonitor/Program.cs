using Serilog;
using Serilog.Sinks.File;
using Serilog;

namespace LoadMonitor
{
  internal static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      // 初始化 Serilog，僅輸出到 Log 資料夾下的文件
      Log.Logger = new LoggerConfiguration()
          .WriteTo.File("Log\\application.log", // 日誌文件路徑
                        rollingInterval: RollingInterval.Day, // 每日滾動新文件
                        retainedFileCountLimit: 7, // 保留最近 7 天的日誌文件
                        fileSizeLimitBytes: 10 * 1024 * 1024, // 單個日誌文件大小限制（10 MB）
                        rollOnFileSizeLimit: true) // 文件過大時滾動
          .CreateLogger();

      try
      {
        Log.Information("Application is starting...");

        // 初始化應用程序
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
        //Application.Run(new SerialPortFormTEST());
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

