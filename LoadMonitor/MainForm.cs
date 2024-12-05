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
      //因為主軸的電流是另外計算,不來自rs485，這邊另外計算
      if (components_.ContainsKey(0/*總覽的key*/) && components_.ContainsKey(17/*主軸的key*/))
      {
        var motors_current = currents[0];//除主軸外, 量測模組量到的電流加總
        var spindle_current = components_[17].GetCurrentLoad();
        currents[0] = motors_current + spindle_current;
      }

      // 更新组件数据
      foreach (var key in components_.Keys.ToList()) // 使用 ToList() 遍历，避免修改集合时出错
      {
        // 判斷 `currents` 是否包含該鍵
        if (!currents.ContainsKey(key))
        {
          continue;
        }
        // 获取当前组件数据
        var componentData = components_[key];
        componentData.Update(currents[key]);//更新圖表
        (string Summary, string DetailInfo) texts = componentData.GetText();
        Log.Information($"key{key} 遍歷 PartBase 名稱{components_[key].ToString()}\tIsActive:{componentData.IsSelected}");
        Log.Information($"摘要{texts.Summary}\n詳細:{texts.DetailInfo}");

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


    private void UpdateMainFormUI(PartBase info)
    {
      // 更新摘要信息
      foreach (Control control in flowLayoutPanel1.Controls)
      {
        // 如果不是 Panel 或者 Panel 没有子控件，直接跳过
        if (!(control is Panel partInfoPanel) || partInfoPanel.Controls.Count == 0)
        {
          continue;
        }

        // 查找 infoPanel
        var infoPanel = partInfoPanel.Controls
            .OfType<Panel>()
            .FirstOrDefault(p => p.Controls.OfType<Label>().Any(l => l.Text == info.MainTitle));

        // 如果没有找到 infoPanel，跳过
        if (infoPanel == null)
        {
          continue;
        }

        // 查找 summaryLabel
        var summaryLabel = infoPanel.Controls.OfType<Label>().FirstOrDefault();

        // 如果没有找到 summaryLabel，跳过
        if (summaryLabel == null)
        {
          continue;
        }

        // 更新副标题
        summaryLabel.Text = info.SubTitle;
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
    private bool isFirstComponentAdded = true; // 用於判斷是否為第一個加入的部件


    private void AddOnClickEvent(PartBase info, Panel partInfoPanel)
    {
      // 按钮的点击事件
      EventHandler callback = (object? sender, EventArgs e) =>
      {
        if (info.DetailForm == null) // 确保 DetailForm 不为空
        {
          throw new InvalidOperationException("詳細頁面不可為空");
        }

        info.DetailForm.TopLevel = false;
        info.DetailForm.Dock = DockStyle.Top; // 每个图像按照顶部对齐方式堆叠
        DetailChartPanel.Controls.Add(info.DetailForm);
        info.DetailForm.Show();
        (string Summary, string DetailInfo) texts = info.GetText();
        TextBoxDetailInfo.Text = texts.DetailInfo;

        if (currentActiveComponent != null)
        {
          currentActiveComponent.IsSelected = false; // 上一个组件取消选中状态
        }

        currentActiveComponent = info; // 更新当前最新被点击的组件
        info.IsSelected = true;        // 标记为选中状态
      };
      partInfoPanel.Click += callback;

      // 如果是第一個加入的部件，直接執行回調
      if (isFirstComponentAdded)
      {
        isFirstComponentAdded = false; // 更新標誌，確保只執行一次
        callback(null, EventArgs.Empty); // 直接觸發回調函數顯示詳細頁面
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
