using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LoadMonitor.Components
{




  internal class Factory
  {

    public struct Config
    {
      public string Type;
      public string Name;
      public int Key;
      public double MaxCurrentValue;
      public SKColor Color
      {
        get
        {
          return Type switch
          {
            "Overview" => new SKColor(0x0a5591),
            "Spindle" => new SKColor(0x7002b0),
            "CutMotor" => new SKColor(0x3cc402),
            "TransferRack" => new SKColor(0xa14808),
            _ => new SKColor(0x000000)
          };
        }
      }
    }

    private const double kSafetyFactor = 2.0;// 安全係數


    // 從設定中讀取機型類型
    private MachineType GetMachineTypeFromSettings()
    {
      try
      {
        string machineType = Settings.Default.MachineType; // 假設設定中有 MachineType
        Enum.TryParse(machineType, out MachineType type);
        return type;
      }
      catch
      {
        throw new InvalidOperationException($"Unknown machine type: {Settings.Default.MachineType}");
      }
    }


    public enum MachineType
    {
      //在線機
      GAM320AT = 0, GAM330AT = 1, GAM360AT = 2, GAM310AT = 3, GAM300AT = 4, GAM380AT = 5,
      GAM336AT = 6, GAM330AD = 7, GAM316AT = 8, GAM336AD = 9, GAM386AT = 10,
      //離線機
      GAM330 = 100, GAM310 = 101, GAM320 = 102, GAM330D = 103, GAM386 = 104,
    }

    public Dictionary<int, PartBase> CreateComponents(Panel DetailChartPanel)
    {
      MachineType type = GetMachineTypeFromSettings();// 從設定中讀取機型類型
      Dictionary<int, PartBase> parts = new Dictionary<int, PartBase>();

      // 初始化基礎部件
      var parts_configs = new List<Config>
      {
        new Config { Type = "Overview", Name = Settings.Default.MachineType, Key = 0, MaxCurrentValue = 5 },
        new Config { Type = "Spindle", Name = Language.GetString("主軸"), Key = 17, MaxCurrentValue = 2 }
      };


      // 根據機型類型動態加入特定部件
      switch (type)
      {
        //離線機
        case MachineType.GAM310:
          parts_configs.AddRange(Create330());//320 一樣創建330
          break;
        case MachineType.GAM320:
          parts_configs.AddRange(Create330());//320 一樣創建330
          break;
        case MachineType.GAM330:
          parts_configs.AddRange(Create330());
          break;
        case MachineType.GAM330D:
          parts_configs.AddRange(Create330D());
          break;
        //case MachineType.GAM386:
        //  parts_configs.AddRange(Create330D());
        //  break;


        //在線機Series

        //case MachineType.GAM300AT:
        //  parts_configs.AddRange(Create300AT());
        //  break;
        //case MachineType.GAM310AT:
        //  parts_configs.AddRange(Create310AT());
        //  break;


        //   330AT series
        case MachineType.GAM320AT:
          parts_configs.AddRange(Create330AT());//320AT也是創建330AT
          break;
        case MachineType.GAM330AT:
          parts_configs.AddRange(Create330AT());
          break;
        case MachineType.GAM330AD:
          parts_configs.AddRange(Create330AD());
          break;
        case MachineType.GAM360AT:
          parts_configs.AddRange(Create330AT());//360AT也是創建330AT
          break;


        //case MachineType.GAM316AT:
        //  parts_configs.AddRange(Create316AT());
        //  break;

        //   336 series
        case MachineType.GAM336AT:
          parts_configs.AddRange(Create336AT());
          break;
        case MachineType.GAM336AD:
          parts_configs.AddRange(Create336AD());
          break;

        // 380 series
        case MachineType.GAM380AT:
          parts_configs.AddRange(Create380AT());
          break;
        //case MachineType.GAM386AT:
        //  parts_configs.AddRange(Create386AT());
        //  break;



        // 可加入更多機型...
        default:
          throw new InvalidOperationException($"Unknown machine type: {type}");
      }


      foreach (var config in parts_configs)
      {
        var component = PartBase.Create(config.Type, config.Name, DetailChartPanel,
          config.MaxCurrentValue * kSafetyFactor, config.Color);
        parts[config.Key] = component;
      }
      return parts;
    }

    private IEnumerable<Config> Create330()
    {
      return new Config[]
      {
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X1", Key = 5, MaxCurrentValue = 1.9},
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y1", Key = 3, MaxCurrentValue = 1.8 },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y2", Key = 4, MaxCurrentValue = 1.8 },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z1", Key = 6, MaxCurrentValue = 0.5 }
      };
    }


    private IEnumerable<Config> Create330D()
    {
      List<Config> gam330d = new List<Config>();
      gam330d.AddRange(Create330());
      gam330d.AddRange(new Config[]
      {
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z2", Key = 8, MaxCurrentValue = 0.5 }
      });
      return gam330d;
    }

    private IEnumerable<Config> Create330AT()
    {
      List<Config> gam330 = new List<Config>();
      gam330.AddRange(Create330());
      gam330.AddRange(new Config[]
      {
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}X", Key = 1, MaxCurrentValue = 1.5 },
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}Z", Key = 2, MaxCurrentValue = 0.4 }
      });
      return gam330;
    }

    private IEnumerable<Config> Create330AD()
    {
      List<Config> gam330ad = new List<Config>();
      gam330ad.AddRange(Create330AT());
      gam330ad.AddRange(new Config[]
      {
        new Config { Type = "TransferRack", Name = $"{Language.GetString("切割")}X2", Key = 7, MaxCurrentValue = 1.8 },
        new Config { Type = "TransferRack", Name = $"{Language.GetString("切割")}Z2", Key = 8, MaxCurrentValue = 1.8 }
      });
      return gam330ad;
    }


    private IEnumerable<Config> Create336AT()
    {
      List<Config> gam336at = new List<Config>();
      gam336at.AddRange(Create330AT());//基於330AT,再加一個移載Y軸
      gam336at.AddRange(new Config[]
      {
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}Y", Key = 7, MaxCurrentValue = 1.8 },
      });
      return gam336at;
    }

    //336AD沒有偵測 移載Z軸, 需要單獨創建
    private IEnumerable<Config> Create336AD()
    {
      List<Config> gam336ad = new List<Config>();
      gam336ad.AddRange(new Config[]
      {
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}X", Key = 1, MaxCurrentValue = 1.5 },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X2", Key = 2, MaxCurrentValue = 1.9},
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y1", Key = 3, MaxCurrentValue = 1.8 },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y2", Key = 4, MaxCurrentValue = 1.8 },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X1", Key = 5, MaxCurrentValue = 1.9},
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z1", Key = 6, MaxCurrentValue = 0.5 },
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}Y", Key = 7, MaxCurrentValue = 1.5 },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z2", Key = 8, MaxCurrentValue = 1.9},
      });
      return gam336ad;
    }


    private IEnumerable<Config> Create380AT()
    {
      List<Config> gam380at = new List<Config>();
      gam380at.AddRange(new Config[]
      {
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}X", Key = 1, MaxCurrentValue = 1.5 },
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}Z", Key = 2, MaxCurrentValue = 0.4 },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y1", Key = 3, MaxCurrentValue = 1.8 },
        //沒有 key 4
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X1", Key = 5, MaxCurrentValue = 1.9},
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z1", Key = 6, MaxCurrentValue = 0.5 },
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}Y", Key = 7, MaxCurrentValue = 1.5 },
      });
      return gam380at;
    }




  }
}
