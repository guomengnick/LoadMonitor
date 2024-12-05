using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.WinForms;

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LoadMonitor.TEST
{
  // 工具类，包含 AddTESTData 方法
  public static class TEST
  {
    public static void AddData(ObservableCollection<ObservableValue> data)
    {
      Random random = new Random(); // 在循环外创建 Random 实例
      for (int i = 0; i < 59; i++)
      {
        var newValue = random.Next(0, 100); // 假设范围是 0~100
        data.Add(new ObservableValue(newValue));
      }

    }


    public static void Add60EmptyData(ObservableCollection<ObservableValue> data)
    {
      Random random = new Random(); // 在循环外创建 Random 实例
      for (int i = 0; i < 59; i++)
      {
        data.Add(new ObservableValue(0));
      }
    }
  }
}
