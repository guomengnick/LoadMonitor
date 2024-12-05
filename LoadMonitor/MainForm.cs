using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveCharts.Wpf;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Windows.Forms;
using LoadMonitor.Components;
using System.Timers;
using System.ComponentModel;
using Microsoft.VisualBasic.Logging;
using Serilog;

namespace LoadMonitor
{
  using Log = Serilog.Log;

  public partial class MainForm : Form
  {
    private Dictionary<int, PartBase> components_; // 用于保存组件数据
    private PartBase currentActiveComponent; // 追踪當前顯示的部件

    private System.Timers.Timer read_current_timer_ = new System.Timers.Timer(1200);
    private ModbusSerialPort modbusSerialPort_; // Modbus 通信物件

    public MainForm()
    {
      InitializeComponent();
      this.FormClosed += MainFormClose;

      InitializePart();
      // 初始化 Modbus 通信
      modbusSerialPort_ = new ModbusSerialPort();
      modbusSerialPort_.Initialize("COM7"); // 替換為實際的串口名稱
      modbusSerialPort_.Open();

      read_current_timer_.Elapsed += Update;
      read_current_timer_.Start();
    }

    private void InitializePart()
    {
      var factory = new Factory();
      components_ = factory.CreateComponents();

      // 將部件添加到界面
      foreach (var component in components_.Values)
      {
        AddComponentChart(component);
      }
    }

    private void MainFormClose(object? sender, FormClosedEventArgs e)
    {
      read_current_timer_.Stop();
      if (modbusSerialPort_ != null && modbusSerialPort_.IsConnected)
      {
        modbusSerialPort_.Close();
      }
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
          componentData.Update(currents[key]);
          (string Summary, string DetailInfo) texts = componentData.GetText();
          Log.Information($"key{key} 遍歷 PartBase 名稱{components_[key].ToString()}\tIsActive:{componentData.IsSelected}");
          Log.Information($"摘要{texts.Summary}\n詳細:{texts.DetailInfo}");

          componentData.SubTitle = texts.Summary;
          componentData.DetailInfo = texts.DetailInfo;

          if (!componentData.IsSelected)
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


    private void UpdateMainFormUI(PartBase info)
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

    private void AddOnClickEvent(PartBase info, Panel partInfoPanel)
    {
      // 按钮的点击事件
      EventHandler callback = (object? sender, EventArgs e) =>
      {
        if (info.DetailForm == null) // 确保 DetailForm 不为空
        {
          Log.Error("詳細頁面不可為空");
          throw new InvalidOperationException("詳細頁面不可為空");
        }

        info.DetailForm.TopLevel = false;
        info.DetailForm.Dock = DockStyle.Top; // 每个图像按照顶部对齐方式堆叠
        DetailChartPanel.Controls.Add(info.DetailForm);
        info.DetailForm.Show();

        var detailLabel = new Label
        {
          Text = info.DetailInfo, // 显示详细信息
          Font = new Font("Arial", 18), // 设置字体
          Dock = DockStyle.Fill,
          TextAlign = ContentAlignment.MiddleLeft, // 左对齐
          AutoSize = true,
          Padding = new Padding(10), // 添加一些内边距
          
          MaximumSize = new Size(DetailTextPanel.Width - 20, 0), // 限制宽度，允许多行显示
        };
        DetailTextPanel.Controls.Add(detailLabel); // 添加 Label 
        DetailTextPanel.Show();

        if (currentActiveComponent != null)
        {
          currentActiveComponent.IsSelected = false; // 上一个组件取消选中状态
        }

        currentActiveComponent = info; // 更新当前最新被点击的组件
        info.IsSelected = true;        // 标记为选中状态
      };
      partInfoPanel.Click += callback;

      // 初始化时执行显示逻辑
      if (DetailTextPanel.Controls.Count == 0)
      {
        callback(null, EventArgs.Empty); // 调用回调函数直接显示内容
        Log.Information($"顯示的Class :{info.MainTitle} IsSelected: true");
        info.IsSelected = true;
        currentActiveComponent = info;
        callback(null, new EventArgs());
      }
    }

    public void AddComponentChart(PartBase info)
    {
      // 创建一个新的 PartInfoPanel 容器
      var partInfoPanel = new Panel
      {
        Width = PartInfoPanel.Width, // 使用设计器中 PartInfoPanel 的宽度
        Height = PartInfoPanel.Height, // 使用设计器中 PartInfoPanel 的高度
        Margin = new Padding(0, 0, 0, 0), // 控制每个 Panel 的上下左右间距
        Padding = new Padding(0, 0, 0, 0), // 增加内部顶部间距，使内容整体向下移动
        BackColor = Color.Transparent // 背景透明
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
        Padding = new Padding(0, 0, 0, 0), // 增加内部顶部间距，使内容整体向下移动
        Dock = DockStyle.Left,
        BackColor = Color.Transparent // 避免覆盖 Panel 的透明背景
      };

      // 将 UserControl (Thumbnail) 添加到 ThumbnailPanel
      info.Dock = DockStyle.Fill; // 填满 ThumbnailPanel
      thumbnailPanel.Controls.Add(info);

      // 创建一个新的 InfoPanel
      var infoPanel = new Panel
      {
        Width = InfoPanel.Width, // 使用设计器中 InfoPanel 的宽度
        Padding = new Padding(0, 0, 0, 0), // 增加内部顶部间距，使内容整体向下移动
        Dock = DockStyle.Right,
        BackColor = Color.Transparent // 避免覆盖 Panel 的透明背景
      };

      // 添加主标题 Label
      var titleLabel = new Label
      {
        Text = info.MainTitle, // 主标题内容
        Font = TitleLabel.Font, // 使用设计器中 TitleLabel 的字体
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
      partInfoPanel.Controls.Add(infoPanel);

      // 添加透明的按钮到 PartInfoPanel 顶层
      AddOnClickEvent(info, partInfoPanel);
      flowLayoutPanel1.Padding = new Padding(0, 0, 0, 0);

      // 将 PartInfoPanel 添加到 FlowLayoutPanel
      flowLayoutPanel1.Controls.Add(partInfoPanel);
    }



  }
}
