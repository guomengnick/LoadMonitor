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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace LoadMonitor
{
  using Log = Serilog.Log;

  public partial class MainForm : Form
  {
    private Dictionary<int, PartBase> components_; // 用于保存组件数据
    private Thumbnail? current_selected_thumbnail_;



    private System.Timers.Timer update_timer_ = new System.Timers.Timer(200);
    private Communication.Manager communication_manager_;

    private Overview? overview_;
    public MainForm(MachineType machine_type, int is_spindle_show)
    {
      InitializeComponent();
      LoadLanguage(machine_type);
      UpdateLanguageMenuState();
      this.FormClosed += MainFormClose;
      this.FormClosing += MainForm_FormClosing;
      InitializePart(machine_type, is_spindle_show);

      communication_manager_ = new Communication.Manager(this.COMPortToolStripMenuItem1);

      update_timer_.Elapsed += Update;//更新畫面
      update_timer_.Interval = 2000;
      update_timer_.Start();
    }

    private void InitializePart(MachineType machine_type, int is_spindle_show)
    {
      var factory = new Factory();
      components_ = factory.CreateComponents(DetailChartPanel, machine_type, is_spindle_show, this/*傳入主控件*/);
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
      // 判斷關閉來源
      if (e.CloseReason == CloseReason.UserClosing)
      {
        // 用戶點擊視窗 X 按鈕，隱藏視窗而不是關閉程式
        e.Cancel = true;
        this.Hide();
      }
      else
      {
        // 其他來源（如通知區域右鍵選單），允許正常關閉
        e.Cancel = false;
      }
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
        Dictionary<int, double> currents = new Dictionary<int, double>();
        try
        {
          currents = communication_manager_.ReadCurrents(true/*TEST*/);//較耗時, 在子線程執行
        }
        catch (Exception ex)
        {
          Log.Error($"讀取rs485失敗，錯誤信息:{ex.Message}");
          ShowErrorMessage($"讀取{Settings.Default.ComPort} 口失敗.");
        }


        if (!is_test_mode && communication_manager_.IsConnected())
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

          // 電機電壓為 220V
          // 主軸電壓為 48V，為了統一單位，需要將主軸電流按照電壓比例進行換算
          // 將主軸電流調整為等效 220V 電壓下的電流後，再進行加總
          // 等效電流 = 主軸電流 × 電機電壓 / 主軸電壓
          currents[0] = motors_current + spindle_current * (48.0 / 220.0);
        }
        var s = "";
        // 更新组件数据
        foreach (var key in components_.Keys.ToList()) // 使用 ToList() 遍历，避免修改集合时出错
        {
          s += $"{key}  電流: {currents[key]}";
          if (!currents.ContainsKey(key)) continue;

          this.Invoke(new Action(() =>//在主線程執行
          {
            components_[key]?.Update(currents[key]);
          }));
        }
        Log.Information(s);

      }
    }

    public void ShowErrorMessage(string msg)
    {
      if (LabelMainMessage.InvokeRequired)
      {
        LabelMainMessage.Invoke(new Action(() =>
        {
          LabelMainMessage.Text = msg;
        }));
      }
      else
      {
        LabelMainMessage.Text = msg;
      }

    }


    // 动态加载语言资源的方法
    private void LoadLanguage(MachineType machine_type)
    {
      // 更新控件的文本内容
      this.Text = MachineTypeHelper.ToString(machine_type) + "  " + Language.GetString("MainForm.Text");
      ToolStripMenuItemLanguege.Text = Language.GetString("ToolStripMenuItemLanguege.Text");

      COMPortToolStripMenuItem1.Text = Language.GetString("COM口設置");
      設置負載警示值ToolStripMenuItem.Text = Language.GetString("設置負載值");

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
      update_timer_.Stop();
      Application.Exit();
      // 清理通知區域圖示
      notifyIcon1.Visible = false;

      // 正常退出程式
      //Application.Exit();
    }

    private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
    {

      // 顯示視窗
      this.Show();

      // 確保視窗從最小化恢復
      if (this.WindowState == FormWindowState.Minimized)
      {
        this.WindowState = FormWindowState.Normal;
      }

      // 將視窗設置為前台
      this.Activate();
    }


    public void OnThumbnailClicked(Thumbnail clickedThumbnail)
    {
      // 還原上一個選中的縮圖底色
      if (current_selected_thumbnail_ != null)
      {
        current_selected_thumbnail_.SetSelected(false); // 設置為未選中
      }
      // 設置當前縮圖的選中狀態
      clickedThumbnail.SetSelected(true);
      current_selected_thumbnail_ = clickedThumbnail;
    }



    // 方法：複製 Thumbnail 控件
    private Thumbnail CloneThumbnail(Thumbnail original)
    {
      Image image = null;
      var newThumbnail = new Thumbnail(image, original.PartBase)
      {
        TopLevel = false,
        // 假設 Thumbnail 是一個自定義控件，複製其需要的屬性
        Name = original.Name,
        Size = original.Size,
        BackColor = original.BackColor,
        Text = original.Text // 如果有 Text 屬性
      };

      // 根據需要複製其他屬性
      return newThumbnail;
    }

    private void 設置負載警示值ToolStripMenuItem_Click(object sender, EventArgs e)
    {

      // 創建背景遮罩
      //var overlay = new Panel
      //{
      //  Dock = DockStyle.Fill,
      //  BackColor = Color.FromArgb(128, 0, 0, 0), // 半透明黑色
      //  Parent = this,
      //  Visible = true
      //};
      //this.Controls.Add(overlay);
      //overlay.BringToFront();

      // 創建設定窗體並傳入母窗體的 FlowLayoutPanel
      var setting_form = new SettingForm(this.flowLayoutPanel1)
      {
        Owner = this, // 設置母窗體為擁有者
        StartPosition = FormStartPosition.CenterParent
      };

      // 顯示設定窗體
      setting_form.ShowDialog();

      // 關閉設定窗體後移除遮罩
      //overlay.Visible = false;
      //this.Controls.Remove(overlay);
    }
  }
}
