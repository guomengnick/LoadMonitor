using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;

namespace LoadMonitor.Components
{

  public enum MachineType
  {
    //在線機
    GAM320AT = 0, GAM330AT = 1, GAM360AT = 2, GAM310AT = 3, GAM300AT = 4, GAM380AT = 5,
    GAM336AT = 6, GAM330AD = 7, GAM316AT = 8, GAM336AD = 9, GAM386AT = 10,
    //離線機
    GAM330 = 100, GAM310 = 101, GAM320 = 102, GAM330D = 103, GAM386 = 104,
  }

  public static class MachineTypeHelper
  {

    private static readonly Dictionary<MachineType, string> machineTypeMap = new Dictionary<MachineType, string>
    {
        { MachineType.GAM320AT, "GAM320AT" }, { MachineType.GAM330AT, "GAM330AT" },
        { MachineType.GAM360AT, "GAM360AT" }, { MachineType.GAM310AT, "GAM310AT" },
        { MachineType.GAM300AT, "GAM300AT" }, { MachineType.GAM380AT, "GAM385AT" },
        { MachineType.GAM336AT, "GAM336AT" }, { MachineType.GAM330AD, "GAM330AD" },
        { MachineType.GAM316AT, "GAM316AT" }, { MachineType.GAM336AD, "GAM336AD" },
        { MachineType.GAM386AT, "GAM386AT" },

        { MachineType.GAM330, "GAM330" }, { MachineType.GAM310, "GAM310" },
        { MachineType.GAM320, "GAM320" }, { MachineType.GAM330D, "GAM330D" },
        { MachineType.GAM386, "GAM386" }
    };

    public static string ToString(MachineType type)
    {
      if (machineTypeMap.TryGetValue(type, out var name))
      {
        return name;
      }
      return "Unknown MachineType";
    }
  }

  internal class Factory
  {

    public struct Config
    {
      public string Type;
      public string Name;
      public int Key;
      public double MaxCurrentValue;
      public string ImagePath;
      public string SettingName; // 對應 Settings.Default 的屬性名稱

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
    public Dictionary<int, PartBase> CreateComponents(Panel DetailChartPanel, MachineType type, string spindle_ini_path, MainForm owner)
    {
      Dictionary<int, PartBase> parts = new Dictionary<int, PartBase>();

      // 初始化基礎部件
      var parts_configs = new List<Config>
      {
        new Config { Type = "Overview", Name = $"{MachineTypeHelper.ToString(type)} {Language.GetString("整機")}",
            Key = 0, MaxCurrentValue = 5 ,
          ImagePath = $@".\Doc\{MachineTypeHelper.ToString(type)}\Overview.png", },
      };

      if (spindle_ini_path != ""/*為空的話就不顯示主軸部件*/)
      {
        Settings.Default.主軸參數檔案位置 = spindle_ini_path;
        Settings.Default.Save();

        parts_configs.Add(new Config
        {
          Type = "Spindle",
          Name = Language.GetString("主軸"),
          Key = 17,
          MaxCurrentValue = 2,
          ImagePath = $@".\Doc\{MachineTypeHelper.ToString(type)}\Spindle.png"
        });
      }

      var directory_path = $@".\Doc\{MachineTypeHelper.ToString(type)}\";

      // 根據機型類型動態加入特定部件
      switch (type)
      {
        //離線機
        case MachineType.GAM310:
          parts_configs.AddRange(Create310(directory_path));//320 一樣創建330
          break;
        case MachineType.GAM320:
          parts_configs.AddRange(Create310(directory_path));//320 一樣創建330
          break;
        case MachineType.GAM330:
          parts_configs.AddRange(Create330(directory_path));
          break;
        case MachineType.GAM330D:
          parts_configs.AddRange(Create330D(directory_path));
          break;
        case MachineType.GAM386:
          parts_configs.AddRange(Create386(directory_path));
          break;


        //在線機Series

        case MachineType.GAM300AT:
          parts_configs.AddRange(Create300AT(directory_path));
          break;
        case MachineType.GAM310AT://沒有移載旋轉軸的336AT，因為沒有監測移載 sita,所以沿用336AT
          parts_configs.AddRange(Create336AT(directory_path));
          break;


        //   330AT series
        case MachineType.GAM320AT:
          parts_configs.AddRange(Create330AT(directory_path));//320AT也是創建330AT
          break;
        case MachineType.GAM330AT:
          parts_configs.AddRange(Create330AT(directory_path));
          break;
        case MachineType.GAM330AD:
          parts_configs.AddRange(Create330AD(directory_path));
          break;
        case MachineType.GAM360AT:
          parts_configs.AddRange(Create330AT(directory_path));//360AT也是創建330AT
          break;


        //316AT沒有量產
        //case MachineType.GAM316AT:
        //  parts_configs.AddRange(Create316AT());
        //  break;

        //   336 series
        case MachineType.GAM336AT:
          parts_configs.AddRange(Create336AT(directory_path));
          break;
        case MachineType.GAM336AD:
          parts_configs.AddRange(Create336AD(directory_path));
          break;

        // 380 series
        case MachineType.GAM380AT:
          parts_configs.AddRange(Create385AT(directory_path));
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
          config.MaxCurrentValue * kSafetyFactor, config.Color, owner, config.ImagePath, config.SettingName);
        parts[config.Key] = component;
      }
      return parts;
    }









    /// <summary>
    /// 以下是離線機
    /// </summary>
    /// <returns></returns>



    //300、310、320A 都是只有XYZ 電機
    private IEnumerable<Config> Create310(string directory_path)//只有切割軸在移動，XYZ的移動
    {
      return new Config[]
      {
    new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X", Key = 5, MaxCurrentValue = 1.9,
      ImagePath = $@"{directory_path}CutMotorX1.png", SettingName = nameof(Settings.Default.切割X1負載率警示) },
    new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y", Key = 3, MaxCurrentValue = 1.8,
      ImagePath = $@"{directory_path}CutMotorY1.png", SettingName = nameof(Settings.Default.切割Y1負載率警示) },
    new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z", Key = 6, MaxCurrentValue = 0.5,
      ImagePath = $@"{directory_path}CutMotorZ1.png", SettingName = nameof(Settings.Default.切割Z1負載率警示) }
      };
    }



    private IEnumerable<Config> Create330(string directory_path)//總共4個電機軸
    {
      return new Config[]
      {
    new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X1", Key = 5, MaxCurrentValue = 1.9,
      ImagePath = $@"{directory_path}CutMotorX1.png", SettingName = nameof(Settings.Default.切割X1負載率警示) },
    new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y1", Key = 3, MaxCurrentValue = 1.8,
      ImagePath = $@"{directory_path}CutMotorY1.png", SettingName = nameof(Settings.Default.切割Y1負載率警示) },
    new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y2", Key = 4, MaxCurrentValue = 1.8,
      ImagePath = $@"{directory_path}CutMotorY2.png", SettingName = nameof(Settings.Default.切割Y2負載率警示) },
    new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z1", Key = 6, MaxCurrentValue = 0.5,
      ImagePath = $@"{directory_path}CutMotorZ1.png", SettingName = nameof(Settings.Default.切割Z1負載率警示) }
      };
    }


    private IEnumerable<Config> Create330D(string directory_path)
    {
      List<Config> gam330d = new List<Config>();
      gam330d.AddRange(Create330($@".\Doc\{MachineTypeHelper.ToString(MachineType.GAM330)}\"));

      gam330d.AddRange(new Config[]
      {
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X2", Key = 7, MaxCurrentValue = 1.9,
          ImagePath = $@"{directory_path}CutMotorX2.png", SettingName = nameof(Settings.Default.切割X2負載率警示) },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z2", Key = 8, MaxCurrentValue = 0.5,
          ImagePath = $@"{directory_path}CutMotorZ2.png", SettingName = nameof(Settings.Default.切割Z2負載率警示) }
      });
      return gam330d;
    }

    private IEnumerable<Config> Create386(string directory_path)
    {
      List<Config> gam380at = new List<Config>();

      gam380at.AddRange(new Config[]
      {
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X", Key = 5, MaxCurrentValue = 1.5,
         ImagePath = $@"{directory_path}CutMotorX1.png", SettingName = nameof(Settings.Default.切割X1負載率警示) },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y", Key = 3, MaxCurrentValue = 1.8,
         ImagePath = $@"{directory_path}CutMotorY1.png", SettingName = nameof(Settings.Default.切割Y1負載率警示) },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z", Key = 6, MaxCurrentValue = 0.4,
         ImagePath = $@"{directory_path}CutMotorZ1.png", SettingName = nameof(Settings.Default.切割Z1負載率警示) },
      });
      return gam380at;
    }



    /// <summary>
    /// 以下是在線機
    /// </summary>
    /// <returns></returns>




    private IEnumerable<Config> Create300AT(string directory_path)//只有切割軸在移動，XYZ的移動
    {
      return new Config[]
      {
    new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X", Key = 5, MaxCurrentValue = 1.9,
      ImagePath = $@"{directory_path}CutMotorX1.png", SettingName = nameof(Settings.Default.切割X1負載率警示) },
    new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y", Key = 3, MaxCurrentValue = 1.8,
      ImagePath = $@"{directory_path}CutMotorY1.png", SettingName = nameof(Settings.Default.切割Y1負載率警示) },
    new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z", Key = 6, MaxCurrentValue = 0.5,
      ImagePath = $@"{directory_path}CutMotorZ1.png", SettingName = nameof(Settings.Default.切割Z1負載率警示) }
      };
    }

    private IEnumerable<Config> Create330AT(string directory_path)// 330:4個電機軸 + 2 = 總共6個電機軸
    {
      List<Config> gam330 = new List<Config>();
      gam330.AddRange(Create330($@".\Doc\{MachineTypeHelper.ToString(MachineType.GAM330)}\"));

      gam330.AddRange(new Config[]
      {
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}X", Key = 1, MaxCurrentValue = 1.5,
      ImagePath = $@"{directory_path}TransferRackX1.png", SettingName = nameof(Settings.Default.移載X負載率警示)  },
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}Z", Key = 2, MaxCurrentValue = 0.45,
      ImagePath = $@"{directory_path}TransferRackZ1.png", SettingName = nameof(Settings.Default.移載Z負載率警示)  }
      });
      return gam330;
    }

    private IEnumerable<Config> Create330AD(string directory_path)
    {
      List<Config> gam330ad = new List<Config>();
      gam330ad.AddRange(Create330AT($@".\Doc\{MachineTypeHelper.ToString(MachineType.GAM330AT)}\"));

      gam330ad.AddRange(new Config[]
      {
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X2", Key = 7, MaxCurrentValue = 1.8 ,
      ImagePath = $@"{directory_path}CutMotorX2.png", SettingName = nameof(Settings.Default.切割X2負載率警示)  },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z2", Key = 8, MaxCurrentValue = 1.8,
      ImagePath = $@"{directory_path}CutMotorZ2.png", SettingName = nameof(Settings.Default.切割Z2負載率警示)   }
      });
      return gam330ad;
    }


    private IEnumerable<Config> Create336AT(string directory_path)// 330AT:6個電機軸 + 移載Y軸(前進後退) = 7個
    {
      List<Config> gam336at = new List<Config>();
      gam336at.AddRange(Create330AT($@".\Doc\{MachineTypeHelper.ToString(MachineType.GAM330AT)}\"));//基於330AT,再加一個移載Y軸


      gam336at.AddRange(new Config[]
      {
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}Y", Key = 7, MaxCurrentValue = 1.8 ,
      ImagePath = $@"{directory_path}TransferRackY1.png", SettingName = nameof(Settings.Default.移載Y負載率警示)  },
      });
      return gam336at;
    }

    //336AD沒有偵測 移載Z軸, 需要單獨創建，以下因為key值與336AT不一樣，獨立創建
    private IEnumerable<Config> Create336AD(string directory_path)
    {

      List<Config> gam336ad = new List<Config>();
      gam336ad.AddRange(new Config[]
      {
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X1", Key = 5, MaxCurrentValue = 1.9,
          ImagePath = $@"{directory_path}CutMotorX1.png", SettingName = nameof(Settings.Default.切割X1負載率警示) },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X2", Key = 2, MaxCurrentValue = 1.9,
          ImagePath = $@"{directory_path}CutMotorX2.png", SettingName = nameof(Settings.Default.切割X2負載率警示) },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y1", Key = 3, MaxCurrentValue = 1.8,
          ImagePath = $@"{directory_path}CutMotorY1.png", SettingName = nameof(Settings.Default.切割Y1負載率警示) },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y2", Key = 4, MaxCurrentValue = 1.8,
          ImagePath = $@"{directory_path}CutMotorY2.png", SettingName = nameof(Settings.Default.切割Y2負載率警示) },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z1", Key = 6, MaxCurrentValue = 0.5,
          ImagePath = $@"{directory_path}CutMotorZ1.png", SettingName = nameof(Settings.Default.切割Z1負載率警示) },
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z2", Key = 8, MaxCurrentValue = 1.9,
          ImagePath = $@"{directory_path}CutMotorZ2.png", SettingName = nameof(Settings.Default.切割Z2負載率警示) },
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}X", Key = 1, MaxCurrentValue = 1.5,
          ImagePath = $@"{directory_path}TransferRackX1.png", SettingName = nameof(Settings.Default.移載X負載率警示)},
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}Y", Key = 7, MaxCurrentValue = 0.4,
          ImagePath = $@"{directory_path}TransferRackY1.png", SettingName = nameof(Settings.Default.移載Y負載率警示) },
  });
      return gam336ad;
    }


    private IEnumerable<Config> Create385AT(string directory_path)// 沒有380AT了，一律改385AT
    {
      List<Config> gam380at = new List<Config>();
      gam380at.AddRange(new Config[]
      {
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}X", Key = 5, MaxCurrentValue = 1.9,
          ImagePath = $@"{directory_path}CutMotorX1.png" , SettingName = nameof(Settings.Default.切割X1負載率警示)},
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Y", Key = 3, MaxCurrentValue = 1.8,
          ImagePath = $@"{directory_path}CutMotorY1.png" , SettingName = nameof(Settings.Default.切割Y1負載率警示)},
        new Config { Type = "CutMotor", Name = $"{Language.GetString("切割")}Z", Key = 6, MaxCurrentValue = 0.5,
          ImagePath = $@"{directory_path}CutMotorZ1.png" , SettingName = nameof(Settings.Default.切割Z1負載率警示)},
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}X", Key = 1, MaxCurrentValue = 1.5,
          ImagePath = $@"{directory_path}TransferRackX1.png", SettingName = nameof(Settings.Default.移載X負載率警示)},
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}Y", Key = 7, MaxCurrentValue = 1.5,
          ImagePath = $@"{directory_path}TransferRackY1.png", SettingName = nameof(Settings.Default.移載Y負載率警示) },
        new Config { Type = "TransferRack", Name = $"{Language.GetString("移載")}Z", Key = 2, MaxCurrentValue = 0.4,
          ImagePath = $@"{directory_path}TransferRackZ1.png", SettingName = nameof(Settings.Default.移載Z負載率警示) },
      });
      return gam380at;
    }





  }
}
