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
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace LoadMonitor
{
  using Log = Serilog.Log;

  public partial class MainForm : Form
  {
    private Dictionary<int, PartBase> components_; // 用于保存组件数据


    private System.Timers.Timer update_timer_ = new System.Timers.Timer(1000);
    private Communication.Manager communication_manager_;

    private Overview? overview_;
    public MainForm(MachineType machine_type)
    {
      InitializeComponent();
      LoadLanguage(machine_type);
      UpdateLanguageMenuState();
      //this.FormClosed += MainFormClose;
      //this.FormClosing += MainForm_FormClosing;
      InitializePart(machine_type);

      communication_manager_ = new Communication.Manager(this.COMPortToolStripMenuItem1);

      update_timer_.Elapsed += Update;//更新畫面
      update_timer_.Interval = 1000;
      update_timer_.Start();
    }

    private void InitializePart(MachineType machine_type)
    {
      var factory = new Factory();
      components_ = factory.CreateComponents(DetailChartPanel, machine_type);
      if (components_.ContainsKey(0) && components_[0] is Overview overview)
      {
        overview_ = overview;
        overview_.AddAllParts(components_);
      }


      // 將部件添加到界面
      bool add_first_detail_form = true;
      foreach (var component in components_.Values)
      {
        //component.Parent = flowLayoutPanel1;
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
        //component.thumbnail_.FormBorderStyle = FormBorderStyle.None; // 移除邊框，讓它像一個控件
        component.thumbnail_.Dock = DockStyle.Fill;    // 填充容器
        component.thumbnail_.Show();                  // 顯示 Form
        // 将 PartInfoPanel 添加到 FlowLayoutPanel
        flowLayoutPanel1.Controls.Add(component.thumbnail_);

      }
    }


    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      // 攔截關閉事件，隱藏表單而不是關閉程式
      e.Cancel = true; // 阻止表單關閉
      this.Hide(); // 隱藏表單
    }

    private void MainFormClose(object? sender, FormClosedEventArgs e)
    {
      update_timer_.Stop();
    }


    //因為有讀取rs485, 以下除了更新UI外，都在子線程執行
    private readonly object update_lock_ = new object(); // 增加鎖來確保執行不交錯
    private void Update(object? sender, ElapsedEventArgs e)
    {

      lock (update_lock_)
      {
        bool is_test_mode = true;
        Dictionary<int, double> currents = communication_manager_.ReadCurrents(true/*TEST*/);//較耗時, 在子線程執行
        if (!is_test_mode && (communication_manager_.IsConnected()))
        {
          Log.Warning("Modbus serial port is not connected.");
          return;
        }

        if (currents.Count == 0)
        {
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
    private void LoadLanguage(MachineType machine_type)
    {
      // 更新控件的文本内容
      this.Text = MachineTypeHelper.ToString(machine_type) + "  " + Language.GetString("MainForm.Text");
      ToolStripMenuItemLanguege.Text = Language.GetString("ToolStripMenuItemLanguege.Text");

      // 定義共享的語言切換邏輯
      EventHandler action = (object? sender, EventArgs e) =>
      {
        if (sender is ToolStripMenuItem selected_item)
        {
          string selected_language = selected_item.Tag as string ?? string.Empty;
          if (Language.CurrentLanguage != selected_language) // 判斷當前語言是否改變
          {
            // 更新語言設定並顯示重啟提示
            Language.SetLanguage(selected_language);
            UpdateLanguageMenuState();
            Helper.ShowRestartDialog(
                $"{Language.GetString("是否重啟")}",
                $"{Language.GetString("切換語言彈窗內文")}"
            );
          }
        }
      };

      // 為每個語言選單項目分配 Tag 和 Click 事件
      簡體中文ToolStripMenuItem.Tag = "zh-CN";
      簡體中文ToolStripMenuItem.Click += action;

      繁體中文ToolStripMenuItem.Tag = "zh-TW";
      繁體中文ToolStripMenuItem.Click += action;

      englishToolStripMenuItem.Tag = "en-US";
      englishToolStripMenuItem.Click += action;
      UpdateLanguageMenuState();
    }


    private void UpdateLanguageMenuState()
    {
      // 遍歷所有語言選單項目
      foreach (ToolStripMenuItem item in ToolStripMenuItemLanguege.DropDownItems)
      {
        // 判斷選單項目的 Tag 是否與當前語言匹配
        if (item.Tag != null && item.Tag.ToString() == Language.CurrentLanguage)
        {
          item.Checked = true;  // 設置為選中
          item.Enabled = false; // 禁用選中項
        }
        else
        {
          item.Checked = false; // 取消選中
          item.Enabled = true;  // 啟用其他選項
        }
      }
    }

    private void 關閉監控軟體ToolStripMenuItem_Click(object sender, EventArgs e)
    {
      // 清理通知區域圖示
      //notifyIcon1.Visible = false;

      // 正常退出程式
      //Application.Exit();
    }
  }
}
