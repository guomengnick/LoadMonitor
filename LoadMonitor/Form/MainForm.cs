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


    private void AddThumbnailToPanel(Thumbnail thumbnail)
    {
      thumbnail.TopLevel = false;         // 設置為非頂層窗口
      thumbnail.FormBorderStyle = FormBorderStyle.None; // 移除邊框，讓它像一個控件
      thumbnail.Dock = DockStyle.Top;    // 填充容器
      thumbnail.Show();                  // 顯示 Form
      flowLayoutPanel1.Controls.Add(thumbnail); // 加入到 flowLayoutPanel1
    }


    public MainForm()
    {
      InitializeComponent();
      this.FormClosed += MainFormClose;
      //flowLayoutPanel1.Controls.Remove(PartInfoPanel);
      flowLayoutPanel1.PerformLayout(); // 刷新布局


      InitializePart();

      // 初始化 Modbus 通信
      modbusSerialPort_ = new ModbusSerialPort();
      modbusSerialPort_.Initialize("COM7"); // 替換為實際的串口名稱
      modbusSerialPort_.Open();

      read_current_timer_.Elapsed += Update;//更新畫面
      read_current_timer_.Start();
    }

    private void InitializePart()
    {
      var factory = new Factory();
      components_ = factory.CreateComponents(DetailChartPanel);
      // 將部件添加到界面
      bool add_first_detail_form = true;
      foreach (var component in components_.Values)
      {
        if (add_first_detail_form)
        {// 對第一個加入的部件先顯示出詳細數據
          add_first_detail_form = false;
          component.DetailChartPanel.Controls.Clear();
          component.DetailForm.TopLevel = false; // 設置為非頂層窗口
          component.DetailForm.Dock = DockStyle.Fill; // 填充父控件
          component.DetailForm.Show(); // 顯示詳細信息
          DetailChartPanel.Controls.Add(component.DetailForm);
        }
        component.thumbnail_.TopLevel = false;         // 設置為非頂層窗口
        component.thumbnail_.FormBorderStyle = FormBorderStyle.None; // 移除邊框，讓它像一個控件
        component.thumbnail_.Dock = DockStyle.Top;    // 填充容器
        component.thumbnail_.Show();                  // 顯示 Form
        // 将 PartInfoPanel 添加到 FlowLayoutPanel
        flowLayoutPanel1.Controls.Add(component.thumbnail_);
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



    //取消前一個被點擊的縮圖
    private void DeactivateCurrentComponent()
    {
      if (currentActiveComponent != null)
      {
        currentActiveComponent.thumbnail_.BackColor = Color.White; // 恢復背景顏色
        currentActiveComponent = null;
      }
    }

  }
}
