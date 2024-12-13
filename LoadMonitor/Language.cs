using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace LoadMonitor
{
  public static class Language
  {
    private static ResourceManager resource_manager_;
    public static string current_culture_code_ = "zh-TW"; // 默认语言

    // 初始化资源管理器
    static Language()
    {
      resource_manager_ = new ResourceManager("LoadMonitor.MainForm", typeof(MainForm).Assembly);

      current_culture_code_ = Settings.Default.語言;
    }

    // 设置语言
    public static void SetLanguage(string cultureCode)
    {
      current_culture_code_ = cultureCode;
      Settings.Default.語言 = cultureCode;

      /// 以下更新的設定值，會更新到以下目錄
      /// C:\Users\user\AppData\Local\LoadMonitor\LoadMonitor_Url_imvlq2gi0qmpb3f1d5soieup1dlra1pn\1.0.0.0
      /// user.config
      /// 以下只針對"語言" 寫在使用者設定內
      Settings.Default.Save();
      Serilog.Log.Information(Settings.Default.語言);
    }

    // 获取当前语言的翻译
    public static string GetString(string key)
    {
      var culture = new CultureInfo(current_culture_code_);
      return resource_manager_.GetString(key, culture);
    }
  }
}
