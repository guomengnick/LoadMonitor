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
      // ��l�� Serilog�A�ȿ�X�� Log ��Ƨ��U�����
      Log.Logger = new LoggerConfiguration()
          .WriteTo.File("Log\\application.log", // ��x�����|
                        rollingInterval: RollingInterval.Day, // �C��u�ʷs���
                        retainedFileCountLimit: 7, // �O�d�̪� 7 �Ѫ���x���
                        fileSizeLimitBytes: 10 * 1024 * 1024, // ��Ӥ�x���j�p����]10 MB�^
                        rollOnFileSizeLimit: true) // ���L�j�ɺu��
          .CreateLogger();

      try
      {
        Log.Information("Application is starting...");

        // ��l�����ε{��
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
        //Application.Run(new SerialPortFormTEST());
      }
      catch (Exception ex)
      {
        // �����ðO�����B�z�����`
        Log.Fatal(ex, "The application failed to start correctly.");
      }
      finally
      {
        // �T�O��x�w�İϤ��e�g�J���
        Log.CloseAndFlush();
      }
    }
  }
}

