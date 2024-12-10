using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LoadMonitor.Components
{

    public enum MachineType
    {
        Unknown = 0,
        Machine330AT,
        Machine336AT,
        Machine380AT,
        Machine330,
        Machine320
    }



    internal class Factory
    {
        private const double kSafetyFactor = 2.0;
        // 提供給外部使用的創建方法，根據機型返回部件字典
        public Dictionary<int, PartBase> CreateComponents(Panel DetailChartPanel)
        {
            // 從設定中讀取機型類型
            MachineType type = GetMachineTypeFromSettings();

            // 根據機型創建對應的部件字典
            return type switch
            {
                MachineType.Machine330AT => Create330AT(DetailChartPanel),
                // MachineType.Machine336AT => Create336AT(),
                // MachineType.Machine380AT => Create380AT(),
                // MachineType.Machine330 => Create330(),
                // MachineType.Machine320 => Create320(),
                _ => throw new InvalidOperationException($"Unknown machine type: {type}")
            };
        }

        // 從設定中讀取機型類型
        private MachineType GetMachineTypeFromSettings()
        {
            try
            {
                string machineType = Settings.Default.MachineType; // 假設設定中有 MachineType
                return Enum.TryParse(machineType, out MachineType type) ? type : MachineType.Unknown;
            }
            catch
            {
                return MachineType.Unknown;
            }
        }

        // 創建 Machine330AT 的部件
        private Dictionary<int, PartBase> Create330AT(Panel DetailChartPanel)
        {
            var componentConfigs = new (string Type, string Name, 
          string SubTitle, string DetailInfo, int Key, double max_current_value, SKColor color)[]
            {
        ("Overview", "總覽", "4%", "電流 : 0.235 A\n附載 : 33%", 0, 5, new SKColor(0x6eddff)), // 加總電流
        ("Spindle", "主軸", "2%", "電流 : 0.135 A\n附載 : 13%", 17, 2, new SKColor(0x7002b0)),
        ("CutMotor", "切割X1", "2%", "電流 : 0.135 A\n附載 : 13%", 5, 1.9, new SKColor(0x3cc402)),
        ("CutMotor", "切割Y1", "14%", "電流 : 0.175 A\n附載 : 16%", 3, 1.8, new SKColor(0x3cc402)),
        ("CutMotor", "切割Y2", "14%", "電流 : 0.175 A\n附載 : 16%", 4, 1.8, new SKColor(0x3cc402)),
        ("CutMotor", "切割Z1", "2%", "電流 : 0.135 A\n附載 : 13%", 6, 0.5, new SKColor(0x3cc402)),
        ("TransferRack", "移載X", "14%", "電流 : 0.135 A\n附載 : 13%", 1, 1.5, new SKColor(0xa14808)),
        ("TransferRack", "移載Z", "34%", "電流 : 0.175 A\n附載 : 16%", 2, 0.4, new SKColor(0xa14808)),
            };

            // 使用工廠類創建部件
            var components = new Dictionary<int, PartBase>();
            foreach (var config in componentConfigs)
            {
                var component = PartBase.Create(config.Type, config.Name, config.SubTitle, 
                  config.DetailInfo, DetailChartPanel, 
                  config.max_current_value * kSafetyFactor, config.color);
                components[config.Key] = component;
            }

            return components;
        }
    }
}
