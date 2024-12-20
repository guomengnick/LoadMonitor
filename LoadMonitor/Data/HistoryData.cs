using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadMonitor.Data
{

    public class HistoryData
    {
        public enum TimeUnit
        {
            Seconds,
            OneHour,
            SixHours
        }

        private readonly int one_hour_capacity_ = 3600;  // 1小时容量
        private readonly int six_hour_capacity_ = 3600 * 6; // 6小时容量
        private Queue<double> one_hour_data_;    // 存储1小时数据
        private Queue<double> six_hour_data_;    // 存储6小时数据

        private readonly double max_value_; // 最大值
        private readonly int sample_count_; // 預設取幾筆
                                            // 构造函数，初始化两种时间范围的容量，以及最大值與預設取樣筆數

        public HistoryData(double maxValue, int sample_count = 3)
        {
            max_value_ = maxValue;
            sample_count_ = sample_count;

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
            if (timeUnit == TimeUnit.Seconds)
            {
                double lastest_value = one_hour_data_.LastOrDefault(0.0);
                return lastest_value / max_value_;//返回最新的負載率
            }
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


        // 判斷是否超出最大值（根據數據中是否有 sample_count_ 筆數據超過 max_value_）
        public bool IsExceedingMax()
        {
            if (one_hour_data_.Count == 0)
            {
                return false; // 沒有數據則直接返回 false
            }

            int exceedCount = 0; // 計算超出或等於最大值的數據筆數

            foreach (var value in one_hour_data_)
            {
                if (value >= max_value_)
                {
                    exceedCount++;
                    if (exceedCount >= sample_count_) // 如果達到設定的筆數，提前返回 true
                    {
                        return true;
                    }
                }
            }

            return false; // 未達到設定的筆數，返回 false
        }

        public void ResetData()
        {
            one_hour_data_.Clear();
            six_hour_data_.Clear();
        }
    }
}
