using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMonitor.Data
{
  public class DailyValueCalculator
  {
    private double totalValue_; // 累積值
    private int count_; // 累積的數據點數量
    private DateTime currentDate_; // 用於追踪當前日期

    public DailyValueCalculator()
    {
      Reset(); // 初始化累積數據
    }

    /// <summary>
    /// 添加最新的數據值並計算每日平均值
    /// </summary>
    /// <param name="currentValue">最新數據值</param>
    /// <param name="multiplier">倍率（例如：電壓或其他因子）</param>
    /// <returns>每日平均值</returns>
    public double AddValue(double currentValue, double multiplier)
    {
      // 如果跨日，自動重置
      if (DateTime.Now.Date != currentDate_.Date)
      {
        Reset();
      }

      // 計算當前數據
      double value = currentValue * multiplier;
      totalValue_ += value; // 累加數據
      count_++; // 累加數據點

      // 返回每日平均值
      return GetDailyAverageValue();
    }

    /// <summary>
    /// 獲取每日平均值
    /// </summary>
    /// <returns>每日平均值</returns>
    private double GetDailyAverageValue()
    {
      return count_ > 0 ? totalValue_ / count_ : 0.0;
    }

    /// <summary>
    /// 重置每日數據
    /// </summary>
    private void Reset()
    {
      totalValue_ = 0.0;
      count_ = 0;
      currentDate_ = DateTime.Now.Date;
    }
  }
}
