using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveCharts.Wpf;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Windows.Forms;
using LoadMonitor.TEST;
using LoadMonitor.Components;
using System.Timers;
using System.ComponentModel;
using Microsoft.VisualBasic.Logging;
using Serilog;

namespace LoadMonitor
{
  using Log = Serilog.Log;
  using ThreadTimer = System.Timers.Timer;
  using Timer = System.Windows.Forms.Timer;


  public enum MachineType
  {
    Unknown = 0,
    Machine330AT,
    Machine336AT,
    Machine380AT,
    Machine330,
    Machine320
  }
  public struct ComponentData
  {
    public PartBase Thumbnail; // 缩图图像
    public string MainTitle; // 主标题
    public string SubTitle; // 副标题
    public string DetailInfo;     // 详细信息
    public Form Detail; // 詳細图像 
    public bool IsActive; // 用來標記當前顯示是否為活動狀態

    public ComponentData(PartBase thumbnail, string mainTitle,
        string subTitle, string detailInfo, Form detail)
    {
      Thumbnail = thumbnail;
      MainTitle = mainTitle;
      SubTitle = subTitle;
      DetailInfo = detailInfo;
      Detail = detail;
      IsActive = false;
    }
  }




  public partial class MainForm : Form
  {
    private Dictionary<int, ComponentData> components_; // 用于保存组件数据
    private ComponentData currentActiveComponent; // 追踪當前顯示的部件

    private System.Timers.Timer read_current_timer_ = new System.Timers.Timer(1200);
    private ModbusSerialPort modbusSerialPort_; // Modbus 通信物件

    public MainForm()
    {
      InitializeComponent();
      this.FormClosed += MainFormClose;
      components_ = new Dictionary<int, ComponentData>(); // 初始化列表

      //AddSpindle();
      //AddCutMotor();
      //AddTransferRackMotor();

      CreateComponents();
      // 初始化 Modbus 通信
      modbusSerialPort_ = new ModbusSerialPort();
      modbusSerialPort_.Initialize("COM7"); // 替換為實際的串口名稱
      modbusSerialPort_.Open();

      read_current_timer_.Elapsed += Update;
      read_current_timer_.Start();
    }

    private void MainFormClose(object? sender, FormClosedEventArgs e)
    {
      read_current_timer_.Stop();
      if (modbusSerialPort_ != null && modbusSerialPort_.IsConnected)
      {
        modbusSerialPort_.Close();
      }
    }

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



    private void CreateComponents()
    {
      // 從設定中讀取機型類型
      AddOverviewChart();

      MachineType type = GetMachineTypeFromSettings();

      // 根據機型創建對應的 components_
      switch (type)
      {
        case MachineType.Machine330AT:
          Create330AT();
          break;
        //case MachineType.Machine336AT:
        //  Create336AT();
        //  break;
        //case MachineType.Machine380AT:
        //  Create380AT();
        //  break;
        //case MachineType.Machine330:
        //  Create330();
        //  break;
        //case MachineType.Machine320:
        //  Create320();
        //  break;
        default:
          Console.WriteLine("Unknown machine type, cannot create components.");
          break;
      }

    }


    private void Create330AT()
    { 
      PartBase component;
      string detail_string = "";
      ComponentData componentdemo;

      // 主軸
      component = new Components.Spindle("主軸");
      detail_string = $@"
電流 : 0.135 A
附載 : 13%";
      componentdemo = new ComponentData(
          component, "主軸", "2%", detail_string, component.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);
      components_[17/*主軸key*/] = componentdemo;


      //移載
      component = new Components.TransferRack("TransferRack X");
      detail_string = $@"
電流 : 0.135 A
附載 : 13%";
      componentdemo = new ComponentData(
         component, "TransferRack X", "14%", detail_string, component.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);
      components_[1] = componentdemo;


      component = new Components.TransferRack("TransferRack Z");
      detail_string = $@"
電流 : 0.175 A
附載 : 16%";
      componentdemo = new ComponentData(
          component, "TransferRack Z", "34%", detail_string, component.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);
      components_[2] = componentdemo;



      //切割軸
      component = new Components.CutMotor("Cutting Y1");
      detail_string = $@"
電流 : 0.175 A
附載 : 16%";
      componentdemo = new ComponentData(
          component, "Cutting Y1", "14%", detail_string, component.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);
      components_[3] = componentdemo;


      component = new Components.CutMotor("Cutting Y2");
      detail_string = $@"
電流 : 0.175 A
附載 : 16%";
      componentdemo = new ComponentData(
          component, "Cutting Y2", "14%", detail_string, component.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);
      components_[4] = componentdemo;


      component = new Components.CutMotor("Cutting X1");
      detail_string = $@"
電流 : 0.135 A
附載 : 13%";
      componentdemo = new ComponentData(
          component, "Cutting X1", "2%", detail_string, component.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);
      components_[5] = componentdemo;

      component = new Components.CutMotor("Cutting Z1");
      detail_string = $@"
電流 : 0.135 A
附載 : 13%";
      componentdemo = new ComponentData(
          component, "Cutting Z1", "2%", detail_string, component.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);
      components_[6] = componentdemo;




    }



    private void Update(object? sender, ElapsedEventArgs e)
    {
      if (modbusSerialPort_ == null || !modbusSerialPort_.IsConnected)
      {
        Console.WriteLine("Modbus serial port is not connected.");
        return;
      }

      // 發送請求並解析數據
      Dictionary<int, double> currents = modbusSerialPort_.ReadCurrents();
      if (currents.Count == 0)
      {
        return;
      }
      if (currents.Count == 16)
      {
        string s = "";
        foreach (var kvp in currents)
        {
          int registerIndex = kvp.Key; // 寄存器索引
          var currentValue = kvp.Value; // 寄存器值
          s += $"Register {registerIndex}: {currentValue}{Environment.NewLine}";
        }
      }
      // 更新组件数据
      foreach (var key in components_.Keys.ToList()) // 使用 ToList() 遍历，避免修改集合时出错
      {
        // 判斷 `currents` 是否包含該鍵
        if (currents.ContainsKey(key))
        {
          // 获取当前组件数据
          var componentData = components_[key];
          componentData.Thumbnail.Update(currents[key]);
          (string Summary, string DetailInfo) texts = componentData.Thumbnail.GetText();
          Log.Information($"key{key} 遍歷 PartBase 名稱{components_[key].Thumbnail.ToString()}\tIsActive:{componentData.IsActive}");
          Log.Information($"摘要{texts.Summary}\n詳細:{texts.DetailInfo}");

          componentData.SubTitle = texts.Summary;
          componentData.DetailInfo = texts.DetailInfo;

          if (!componentData.IsActive)
          {
            continue;
          }

          // 确保在 UI 线程中更新主画面内容
          if (InvokeRequired)
          {
            Invoke(new Action(() => UpdateMainFormUI(componentData)));
          }
          else
          {
            UpdateMainFormUI(componentData);
          }
        }
      }
    }


    private void UpdateMainFormUI(ComponentData info)
    {
      // 更新摘要信息
      foreach (Control control in flowLayoutPanel1.Controls)
      {
        if (control is Panel partInfoPanel && partInfoPanel.Controls.Count > 0)
        {
          var infoPanel = partInfoPanel.Controls
              .OfType<Panel>()
              .FirstOrDefault(p => p.Controls.OfType<Label>().Any(l => l.Text == info.MainTitle));

          if (infoPanel != null)
          {
            var summaryLabel = infoPanel.Controls.OfType<Label>().FirstOrDefault();
            if (summaryLabel != null)
            {
              summaryLabel.Text = info.SubTitle; // 更新副标题
            }
          }
        }
      }

      // 更新縮圖的顏色
      //info.Thumbnail.Highlight(); // 假設 Highlight 方法將縮圖變色

      // 清空并更新 DetailTextPanel 的详细信息
      DetailTextPanel.Controls.Clear();
      var detailTextBox = new TextBox
      {
        Text = info.DetailInfo,
        Font = new Font("Arial", 12),
        Dock = DockStyle.Fill,
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Vertical, // 添加滾動條（若內容過多）
        BorderStyle = BorderStyle.None
      };
      DetailTextPanel.Controls.Add(detailTextBox);
      DetailTextPanel.Show();
    }

    private void AddSpindle()
    {
      var cut_motor = new Components.Spindle("4033 Spindle");
      var detail_string = $@"
電流 : 0.135 A
溫度 : 13%";
      var componentdemo = new ComponentData(
          cut_motor, "4033 Spindle", "14%", detail_string, cut_motor.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);
    }

    private void AddTransferRackMotor()
    {
      var cut_motor = new Components.TransferRack("TransferRack X1");
      var detail_string = $@"
電流 : 0.135 A
附載 : 13%";
      var componentdemo = new ComponentData(
          cut_motor, "TransferRack X1", "14%", detail_string, cut_motor.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);

      cut_motor = new Components.TransferRack("TransferRack Z");
      detail_string = $@"
電流 : 0.175 A
附載 : 16%";
      componentdemo = new ComponentData(
          cut_motor, "TransferRack Z", "34%", detail_string, cut_motor.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);
    }

    private void AddCutMotor()
    {
      var cut_motor = new Components.CutMotor("Cutting X1");
      var detail_string = $@"
電流 : 0.135 A
附載 : 13%";
      var componentdemo = new ComponentData(
          cut_motor, "Cutting X1", "2%", detail_string, cut_motor.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);

      cut_motor = new Components.CutMotor("Cutting Y1");
      detail_string = $@"
電流 : 0.175 A
附載 : 16%";
      componentdemo = new ComponentData(
          cut_motor, "Cutting Y1", "14%", detail_string, cut_motor.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);


      cut_motor = new Components.CutMotor("Cutting Y2");
      detail_string = $@"
電流 : 0.235 A
附載 : 33%";
      componentdemo = new ComponentData(
          cut_motor, "Cutting Y2", $"4%", detail_string, cut_motor.GetDetailForm());
      // 显示到界面
      AddComponentChart(componentdemo);
    }


    private void AddOverviewChart()
    {
      var overview_chart = new Components.Overview();
      var detail_string = $@"
電流 : 0.235 A
附載 : 33%";
      var componentdemo = new ComponentData(
          overview_chart, "Overview", $"4%", detail_string, overview_chart.GetDetailForm());
      AddComponentChart(componentdemo);

      components_[0/*0 是加總電流*/] = componentdemo;
    }

    private void AddOnClickEvent(ComponentData info, Panel partInfoPanel)
    {
      // 按钮的点击事件
      EventHandler callback = (object? sender, EventArgs e) =>
      {
        if (info.Detail != null) // 确保 Detail 列表不为空
        {
          info.Detail.TopLevel = false;
          info.Detail.Dock = DockStyle.Top; // 每个图像按照顶部对齐方式堆叠
          DetailChartPanel.Controls.Add(info.Detail);
          info.Detail.Show();
        }

        var detailLabel = new Label
        {
          Text = info.DetailInfo, // 显示详细信息
          Font = new Font("Arial", 12), // 设置字体
          Dock = DockStyle.Fill,
          TextAlign = ContentAlignment.MiddleLeft, // 左对齐
          AutoSize = false,
          Padding = new Padding(10) // 添加一些内边距
        };
        DetailTextPanel.Controls.Add(detailLabel); // 添加 Label 
        DetailTextPanel.Show();
        if(currentActiveComponent.IsActive)
        {
          currentActiveComponent.IsActive = false;
        }
        currentActiveComponent = info;// 更新當前最新被點擊的
        info.IsActive = true;//標記為顯示

      };
      partInfoPanel.Click += callback;

      // 初始化時執行顯示邏輯
      if (DetailTextPanel.Controls.Count == 0)
      {
        callback(null, EventArgs.Empty); // 调用回调函数直接显示内容
        Log.Information($"顯示的Class :{info.Thumbnail.ToString()}Isactive  true");
        info.IsActive = true;
        currentActiveComponent = info;
      }
    }

    public void AddComponentChart(ComponentData info)
    {
      // 创建一个新的 PartInfoPanel 容器
      var partInfoPanel = new Panel
      {
        Width = PartInfoPanel.Width, // 使用设计器中 PartInfoPanel 的宽度
        Height = PartInfoPanel.Height, // 使用设计器中 PartInfoPanel 的高度
        Margin = new Padding(0, 0, 0, 0), // 控制每个 Panel 的上下左右间距
        Padding = new Padding(0, 0, 0, 0), // 增加内部顶部间距，使内容整体向下移动

        BackColor = Color.Transparent, // 背景透明
                                       //BorderStyle = BorderStyle.FixedSingle,
      };
      // 模拟 Button 的悬停效果
      partInfoPanel.MouseEnter += (s, e) =>
      {
        partInfoPanel.BackColor = Color.LightBlue; // 鼠标进入时，设置背景颜色为浅蓝色
      };
      partInfoPanel.MouseLeave += (s, e) =>
      {
        partInfoPanel.BackColor = Color.Transparent; // 鼠标移开时，恢复背景透明
      };

      // 创建一个新的 ThumbnailPanel
      var thumbnailPanel = new Panel
      {
        Width = ThumbnailPanel.Width, // 使用设计器中 ThumbnailPanel 的宽度
                                      //Margin = new Padding(0,0,0,0), // 控制每个 Panel 的上下左右间距
        Padding = new Padding(0, 0, 0, 0), // 增加内部顶部间距，使内容整体向下移动

        Dock = DockStyle.Left,
        BackColor = Color.Transparent // 避免覆盖 Panel 的透明背景
      };

      // 将 UserControl (Thumbnail) 添加到 ThumbnailPanel
      info.Thumbnail.Dock = DockStyle.Fill; // 填满 ThumbnailPanel
      thumbnailPanel.Controls.Add(info.Thumbnail);

      // 创建一个新的 InfoPanel
      var infoPanel = new Panel
      {
        Width = InfoPanel.Width, // 使用设计器中 InfoPanel 的宽度
                                 //Margin = new Padding(0,0,0,0), // 控制每个 Panel 的上下左右间距
                                 //Padding = new Padding(0, 20, 0, 0), // 增加内部顶部间距，使内容整体向下移动
        Padding = new Padding(0, 0, 0, 0), // 增加内部顶部间距，使内容整体向下移动
        Dock = DockStyle.Right,
        BackColor = Color.Transparent // 避免覆盖 Panel 的透明背景
      };

      // 添加主标题 Label
      var titleLabel = new Label
      {
        Text = info.MainTitle, // 主标题内容
        Font = TitleLabel.Font, // 使用设计器中 TitleLabel 的字体
                                //Margin = new Padding(10, 20, 10, 0), // 增加顶部间距（第二个参数控制顶部间距）
        Padding = new Padding(0, 0, 0, 0), // 增加内部顶部间距，使内容整体向下移动

        Dock = DockStyle.Top,
        TextAlign = TitleLabel.TextAlign, // 使用设计器中 TitleLabel 的对齐方式
        Height = TitleLabel.Height, // 使用设计器中 TitleLabel 的高度
        AutoSize = false,
      };

      // 添加副标题 Label
      var summaryLabel = new Label
      {
        Text = info.SubTitle, // 副标题内容
        Font = SummaryLabel.Font, // 使用设计器中 SummaryLabel 的字体
                                  //Margin = new Padding(10,10,0,0), // 控制每个 Panel 的上下左右间距
        Padding = new Padding(0, 8, 0, 0), // 增加内部顶部间距，使内容整体向下移动
        Dock = DockStyle.Top,
        TextAlign = SummaryLabel.TextAlign, // 使用设计器中 SummaryLabel 的对齐方式
        AutoSize = true, // 设置自动调整大小
        MaximumSize = new Size(infoPanel.Width, 0), // 限制最大宽度，允许换行
      };
      // 将主标题和副标题添加到 InfoPanel
      infoPanel.Controls.Add(summaryLabel); // 添加副标题
      infoPanel.Controls.Add(titleLabel);   // 添加主标题
                                            // 将 ThumbnailPanel 和 InfoPanel 添加到 PartInfoPanel
      partInfoPanel.Controls.Add(thumbnailPanel);
      // *** 确保先添加 hoverButton ***
      partInfoPanel.Controls.Add(infoPanel);

      // 添加透明的按钮到 PartInfoPanel 顶层
      AddOnClickEvent(info, partInfoPanel);
      flowLayoutPanel1.Padding = new Padding(0, 0, 0, 0);
      // 将 PartInfoPanel 添加到 FlowLayoutPanel
      flowLayoutPanel1.Controls.Add(partInfoPanel);
    }



  }
}
