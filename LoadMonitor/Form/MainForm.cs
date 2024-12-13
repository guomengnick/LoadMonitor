using System;
using System.Windows.Forms;
using LoadMonitor.Components;
using System.Timers;
using System.ComponentModel;
using Microsoft.VisualBasic.Logging;
using Serilog;
using System.Resources;
using System.Globalization;
using HarfBuzzSharp;

namespace LoadMonitor
{
  using Log = Serilog.Log;

  public partial class MainForm : Form
  {
    private Dictionary<int, PartBase> components_; // 用于保存组件数据
    private System.Timers.Timer read_current_timer_ = new System.Timers.Timer(500);
    private ModbusSerialPort modbusSerialPort_; // Modbus 通信物件
    private Overview overview_;
    private System.Resources.ResourceManager resource_manager_;
    public MainForm()
    {
      InitializeComponent();
      LoadLanguage();
      UpdateLanguageMenuState();
      this.FormClosed += MainFormClose;

      InitializePart();
      modbusSerialPort_ = new ModbusSerialPort(Settings.Default.ComPort);// 初始化 Modbus 通信

      read_current_timer_.Elapsed += Update;//更新畫面
      read_current_timer_.Start();
    }

    private void InitializePart()
    {
      var factory = new Factory();
      components_ = factory.CreateComponents(DetailChartPanel);
      if (components_.ContainsKey(0) && components_[0] is Overview overview)
      {
        overview_ = overview;
        overview_.AddAllParts(components_);
      }


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


    //因為有讀取rs485, 以下除了更新UI外，都在子線程執行
    private readonly object update_lock_ = new object(); // 增加鎖來確保執行不交錯
    private void Update(object? sender, ElapsedEventArgs e)
    {

      lock (update_lock_)
      {
        if (modbusSerialPort_ == null || !modbusSerialPort_.IsConnected)
        {
          Log.Warning("Modbus serial port is not connected.");
          return;
        }

        // 發送請求並解析數據
        Dictionary<int, double> currents = modbusSerialPort_.ReadCurrents();//較耗時, 在子線程執行
        if (currents.Count == 0)
        {
          //Log.Warning("Modbus serial port 收到的值為空");
          return;
        }

        //因為主軸的電流是另外計算,不來自rs485，這邊另外計算
        if (components_.ContainsKey(0/*總覽的key*/) && components_.ContainsKey(17/*主軸的key*/))
        {
          var motors_current = currents[0];//除主軸外, 量測模組量到的電流加總
          var spindle_current = components_[17].GetCurrentLoad();
          currents[17/*主軸*/] = spindle_current;
          //currents[0] = motors_current + spindle_current;//TODO:目前主軸為測試值
        }

        // 更新组件数据
        foreach (var key in components_.Keys.ToList()) // 使用 ToList() 遍历，避免修改集合时出错
        {
          if (!currents.ContainsKey(key)) continue;

          this.Invoke(new Action(() =>//在主線程執行
          {
            components_[key]?.Update(currents[key]);
          }));
        }

      }
    }





    // 动态加载语言资源的方法
    private void LoadLanguage()
    {
      // 更新控件的文本内容
      this.Text = Settings.Default.MachineType + "  " + Language.GetString("MainForm.Text");
      ToolStripMenuItemSetting.Text = Language.GetString("ToolStripMenuItemSetting.Text");
      ToolStripMenuItemLanguege.Text = Language.GetString("ToolStripMenuItemLanguege.Text");
    }

    private void 簡體中文ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Language.SetLanguage("zh-CN");
      UpdateLanguageMenuState();
      MessageBox.Show("切换语言须重启系统!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void 繁體中文ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Language.SetLanguage("zh-TW");
      UpdateLanguageMenuState();
      MessageBox.Show("切換語言須重啟系統!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void 英文ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Language.SetLanguage("en-US");
      UpdateLanguageMenuState();
      MessageBox.Show("Language switch requires restart!!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // 更新语言菜单的选中状态
    private void UpdateLanguageMenuState()
    {
      簡體中文ToolStripMenuItem.Checked = Language.current_culture_code_ == "zh-CN";
      繁體中文ToolStripMenuItem.Checked = Language.current_culture_code_ == "zh-TW";
      englishToolStripMenuItem.Checked = Language.current_culture_code_ == "en-US";
    }


  }
}
