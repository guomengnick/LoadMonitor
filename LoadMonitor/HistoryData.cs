using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMonitor
{

  public class HistoryData
  {
    public enum TimeUnit
    {
      OneHour,
      SixHours
    }

    private readonly int one_hour_capacity_ = 3600;  // 1小时容量
    private readonly int six_hour_capacity_ = 3600 * 6; // 6小时容量
    private Queue<double> one_hour_data_;    // 存储1小时数据
    private Queue<double> six_hour_data_;    // 存储6小时数据

    // 构造函数，初始化两种时间范围的容量
    public HistoryData()
    {
      one_hour_data_ = new Queue<double>(one_hour_capacity_);
      six_hour_data_ = new Queue<double>(six_hour_capacity_);
    }

    // 添加新的数据点
    public void AddDataPoint(double value)
    {
      // 添加到1小时队列
      if (one_hour_data_.Count >= one_hour_capacity_)
      {
        one_hour_data_.Dequeue(); // 移除最旧数据
      }
      one_hour_data_.Enqueue(value);

      // 添加到6小时队列
      if (six_hour_data_.Count >= six_hour_capacity_)
      {
        six_hour_data_.Dequeue(); // 移除最旧数据
      }
      six_hour_data_.Enqueue(value);
    }

    // 使用枚举获取所有历史数据点
    public List<double> GetDataPoints(TimeUnit timeUnit)
    {
      return timeUnit switch
      {
        TimeUnit.OneHour => new List<double>(one_hour_data_),
        TimeUnit.SixHours => new List<double>(six_hour_data_),
        _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), "Unsupported TimeUnit.")
      };
    }

    // 使用枚举返回所有时间范围的平均值
    public Dictionary<TimeUnit, double> GetDataPoints()
    {
      return new Dictionary<TimeUnit, double>
        {
          { TimeUnit.OneHour, GetAverage(TimeUnit.OneHour) },
          { TimeUnit.SixHours, GetAverage(TimeUnit.SixHours) }
        };
    }


    // 使用枚举获取平均值
    public double GetAverage(TimeUnit timeUnit)
    {
      var dataQueue = timeUnit switch
      {
        TimeUnit.OneHour => one_hour_data_,
        TimeUnit.SixHours => six_hour_data_,
        _ => throw new ArgumentOutOfRangeException(nameof(timeUnit), "Unsupported TimeUnit.")
      };

      if (dataQueue.Count == 0)
      {
        return 0.0;
      }

      double sum = 0.0;
      foreach (var value in dataQueue)
      {
        sum += value;
      }
      return sum / dataQueue.Count;
    }

    // 清空数据
    public void Clear()
    {
      one_hour_data_.Clear();
      six_hour_data_.Clear();
    }
  }
}
