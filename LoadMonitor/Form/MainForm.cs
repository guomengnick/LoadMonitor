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
using System.Diagnostics;

namespace LoadMonitor
{
  using Log = Serilog.Log;

  public partial class MainForm : Form
  {
    private Dictionary<int, PartBase> components_; // 用于保存组件数据
    private Thumbnail? current_selected_thumbnail_;



    public System.Timers.Timer update_timer_ = new System.Timers.Timer(500);
    public System.Timers.Timer restart_timer_ = new System.Timers.Timer(7200000/*2個小時*/)
    {
      AutoReset = false/*避免多次觸發*/
    };

    private Communication.Manager communication_manager_;

    private Overview? overview_;
    private readonly MachineType machine_type_;
    private readonly string spindle_ini_path_;
    public MainForm(MachineType machine_type, string spindle_ini_path)
    {
      machine_type_ = machine_type;
      spindle_ini_path_ = spindle_ini_path;

      InitializeComponent();
      this.MouseWheel += MouseWheelTrigger;
      communication_manager_ = new Communication.Manager(this.COMPortToolStripMenuItem1);

      LoadLanguage(machine_type);
      UpdateLanguageMenuState();
      this.FormClosed += MainFormClose;
      this.FormClosing += MainForm_FormClosing;
      InitializePart(machine_type, spindle_ini_path);


      update_timer_.Elapsed += Update;//更新畫面
      if (Read485Value())
      {
        Log.Information("開始讀取RS485");
        update_timer_.Start();//如果COM口沒有設置的話，就不要啟動
      }
      else
      {
        Log.Information("不讀取RS485");
        ShowErrorMessage(Language.GetString("請選擇電流"));
        update_timer_.Interval = 1000;//一樣啟動，但就是數值都給0
        update_timer_.Start();//如果COM口沒有設置的話，就不要啟動
      }

      InitializeRestartTimer();
    }

    public bool Read485Value()
    {
      // 檢查選單中是否有 Checked 的子項目
      var selected_item = COMPortToolStripMenuItem1.DropDownItems
          .OfType<ToolStripMenuItem>()
          .FirstOrDefault(item => item.Checked);

      // 如果沒有選中任何項目，則認為未選擇
      if (selected_item == null)
      {
        return false;
      }

      // 檢查選中的項目是否為 "未選擇"
      return selected_item.Text != Language.GetString("未選擇");

    }

    private void InitializePart(MachineType machine_type, string spindle_ini_path)
    {
      var factory = new Factory();
      components_ = factory.CreateComponents(DetailChartPanel, machine_type, spindle_ini_path, this/*傳入主控件*/);
      if (components_.ContainsKey(0) && components_[0] is Overview overview)
      {
        overview_ = overview;
        overview_.AddAllParts(components_);
      }


      // 將部件添加到界面
      bool add_first_detail_form = true;
      PartBase first = null;
      foreach (var component in components_.Values)
      {
        //component.Parent = flowLayoutPanel1;
        if (add_first_detail_form)
        {// 對第一個加入的部件先顯示出詳細數據
          first = component;
          add_first_detail_form = false;
          component.DetailChartPanel.Controls.Clear();
          component.DetailForm.TopLevel = false; // 設置為非頂層窗口
          component.DetailForm.Dock = DockStyle.Fill; // 填充父控件
          component.DetailForm.Show(); // 顯示詳細信息
          component.thumbnail_.Thumbnail_Click(null, new EventArgs());//顯示 'active' 

          DetailChartPanel.Controls.Add(component.DetailForm);
          UpdatePartImage(component.thumbnail_.image_);
        }
        component.thumbnail_.TopLevel = false;         // 設置為非頂層窗口
        //component.thumbnail_.FormBorderStyle = FormBorderStyle.None; // 移除邊框，讓它像一個控件
        component.thumbnail_.Dock = DockStyle.Fill;    // 填充容器
        component.thumbnail_.Show();                  // 顯示 Form
        // 将 PartInfoPanel 添加到 FlowLayoutPanel
        flowLayoutPanel1.Controls.Add(component.thumbnail_);


        component.thumbnail_?.Thumbnail_Click(null, new EventArgs());
        current_selected_thumbnail_ = component.thumbnail_;
      }

      first?.thumbnail_?.Thumbnail_Click(null, new EventArgs());
      current_selected_thumbnail_ = first.thumbnail_;


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
          //較耗時, 在子線程執行
          bool test_mode = !Read485Value();
          currents = communication_manager_.ReadCurrents(test_mode/*是否讀取485，若沒選擇，則不讀取*/);
        }
        catch (Exception ex)
        {

          Log.Error($"讀取rs485失敗，錯誤信息:{ex.Message}");
          string selected_port = Settings.Default.ComPort;

          string message = string.Format(
              Language.GetString("切換COM口彈窗內文"),
              selected_port
          );

          ShowErrorMessage(message);
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

          if (key == 1)
          {
            currents[key] += 1.5;
          }
          this.Invoke(new Action(() =>//在主線程執行
          {
            components_[key]?.Update(currents[key]);
          }));
        }
        //Log.Information(s);

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

      關閉監控軟體ToolStripMenuItem.Text = Language.GetString("關閉監控軟體");

      未選擇ToolStripMenuItem.Text = Language.GetString("未選擇");

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
      // 清理通知區域圖示
      notifyIcon1.Visible = false;

      // 显式调用 PartBase 的 Dispose 方法
      DisposePartBaseComponents();

      // 正常退出程式
      Application.Exit();
    }

    // 手动释放 PartBase 资源
    private void DisposePartBaseComponents()
    {
      if (components_ != null)
      {
        foreach (var partBase in components_.Values)
        {
          partBase.Dispose();
        }
        components_.Clear(); // 清空 Dictionary
      }

      Serilog.Log.Information("所有 PartBase 组件已释放。");
    }

    private void InitializeRestartTimer()
    {
      restart_timer_.Interval = 600000;
      restart_timer_.Elapsed += RestartApplication;
      restart_timer_.Start();

      Serilog.Log.Information("2 小时自动重启计时器已启动。");
    }

    private void RestartApplication(object? sender, ElapsedEventArgs e)
    {
      restart_timer_.Stop();// 停止计时器，避免多次触发


      PerformApplicationRestart();
    }


    private void PerformApplicationRestart()
    {
      Serilog.Log.Information("程序即将重启...");

      // 保存所有数据并释放资源
      DisposePartBaseComponents();

      //用初始化的參數，把自己啟動起來，這是 "GAM320AT" 啟動時傳入的參數
      var args = $"{(int)this.machine_type_}%{this.spindle_ini_path_}";
      string executablePath = Process.GetCurrentProcess().MainModule.FileName;

      // 启动新的进程，传入参数
      Process.Start(executablePath,  args);
      Serilog.Log.Information($"Application restarted with arguments: {this.machine_type_}%{this.spindle_ini_path_}");
      // 退出当前进程
      Application.Exit();
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

      UpdatePartImage(clickedThumbnail.image_);
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



    public void UpdatePartImage(System.Drawing.Image image)
    {
      //PanelPartView.Dock = DockStyle.Fill;
      PanelPartView.BackgroundImage = image;
      PanelPartView.BackgroundImageLayout = ImageLayout.Stretch; // 確保圖片適應面板大小
    }


    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      const int ScrollAmount = 20; // 每次滾動的像素量

      if (keyData == Keys.Up)
      {
        MoveSelection(true); // 向上移動
        return true; // 表示已處理
      }
      else if (keyData == Keys.Down)
      {
        MoveSelection(false); // 向下移動
        return true; // 表示已處理
      }

      return base.ProcessCmdKey(ref msg, keyData);
    }



    private void MoveSelection(bool up)
    {
      if (components_ == null || components_.Count == 0)
      {
        return; // 如果沒有元件，直接返回
      }

      // 取得目前的 keys 列表
      var keys = components_.Keys.ToList();

      // 找到目前選中的索引
      int currentIndex = keys.IndexOf(components_.FirstOrDefault(
          x => x.Value.thumbnail_ == current_selected_thumbnail_).Key);

      // 如果目前選中不存在，則預設為第一個索引
      if (currentIndex == -1)
      {
        currentIndex = 0;
      }

      // 計算新的索引
      int newIndex;
      if (up)
      {
        // 上一個（如果是第 0 個，則循環到最後一個）
        newIndex = currentIndex == 0 ? keys.Count - 1 : currentIndex - 1;
      }
      else
      {
        // 下一個（如果是最後一個，則循環到第 0 個）
        newIndex = currentIndex == keys.Count - 1 ? 0 : currentIndex + 1;
      }

      // 更新 current_selected_thumbnail_
      var newKey = keys[newIndex];
      var component = components_[newKey];

      // 觸發 Thumbnail_Click 事件
      component.thumbnail_?.Thumbnail_Click(null, new EventArgs());
      current_selected_thumbnail_ = component.thumbnail_;

      // 滾動到對應的控件
      if (flowLayoutPanel1.Controls.Contains(component.thumbnail_))
      {
        flowLayoutPanel1.ScrollControlIntoView(component.thumbnail_);
      }
    }



    private void MouseWheelTrigger(object sender, MouseEventArgs e)
    {
      if (e.Delta > 0) // 滾輪向上
      {
        MoveSelection(true); // 上一個
      }
      else if (e.Delta < 0) // 滾輪向下
      {
        MoveSelection(false); // 下一個
      }
    }



  }


}
