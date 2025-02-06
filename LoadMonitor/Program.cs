using Serilog;
using Serilog.Sinks.File;
using System.Windows.Forms;
using LoadMonitor.Components;
using SQLitePCL;
using Microsoft.Data.Sqlite;

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

      
      System.Threading.Thread.Sleep(2000);// 延遲一下，避免與上一次的啟動衝突，導致windows畫面閃爍

      Batteries.Init();
      // 创建并测试数据库连接
      using (var connection = new SqliteConnection("Data Source=history.db"))
      {
        connection.Open();

        // 创建表
        var command = connection.CreateCommand();
        command.CommandText = @"
                CREATE TABLE IF NOT EXISTS PartHistory (
                    PartName TEXT NOT NULL,
                    TimeUnit TEXT NOT NULL,
                    Timestamp DATETIME NOT NULL,
                    Value REAL NOT NULL,
                    PRIMARY KEY (PartName, TimeUnit, Timestamp)
                );
            ";
        command.ExecuteNonQuery();

        Console.WriteLine("Database initialized successfully.");
      }




      // 初始化 Serilog，僅輸出到 Log 資料夾下的文件
      Log.Logger = new LoggerConfiguration()
          .WriteTo.File("Log\\application.log", // 日誌文件路徑
                        rollingInterval: RollingInterval.Day, // 每日滾動新文件
                        retainedFileCountLimit: 7, // 保留最近 7 天的日誌文件
                        fileSizeLimitBytes: 10 * 1024 * 1024, // 單個日誌文件大小限制（10 MB）
                        rollOnFileSizeLimit: true) // 文件過大時滾動
          .CreateLogger();


      string settingsFilePath = System.Configuration.ConfigurationManager.OpenExeConfiguration(
          System.Configuration.ConfigurationUserLevel.None).FilePath;
      string exePath = AppDomain.CurrentDomain.BaseDirectory;

      Log.Information($"參數位置: {settingsFilePath}");
      Log.Information($"執行檔位置: {exePath}");

      string modelType = "5";
      string iniPath = "";
      if (args.Length == 1)
      {
        Log.Information($"啟動參數: {args[0]}");
        // 使用 % 分割參數
        string[] splitArgs = args[0].Split('%');

        if (splitArgs.Length == 2)
        {
          modelType = splitArgs[0];
          iniPath = splitArgs[1];
          Log.Information($"ModelType: {modelType}");
          Log.Information($"IniPath: {iniPath}");
        }
      }
      else
      {
        iniPath = "C:\\Program1\\GAM320AT\\LoadMonitor\\spindle_info.ini";
      }


      int machineTypeValue = 0; // 預設值
      MachineType machineTypeEnum = (MachineType)Settings.Default.MachineType;

      string settingsdefaultFilePath = System.Configuration.ConfigurationManager.OpenExeConfiguration(
      System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
      Log.Information($"Settings.Default 配置文件位置: {settingsdefaultFilePath}");

      try
      {
        if (args.Length > 0)
        {
          // 如果有傳入的參數, 將傳入的參數轉換成 int
          machineTypeValue = int.Parse(modelType);

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

        // 初始化應用程序
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm(machineTypeEnum, iniPath));
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

