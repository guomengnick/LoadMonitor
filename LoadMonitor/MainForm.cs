using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveCharts.Wpf;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Windows.Forms;
using LoadMonitor.TEST;
using LoadMonitor.Components;

namespace LoadMonitor
{

  public struct ComponentData
  {
    public UserControl Thumbnail; // 缩图图像
    public string MainTitle; // 主标题
    public string SubTitle; // 副标题
    public string DetailInfo;     // 详细信息
    public Form Detail; // 詳細图像 

    public ComponentData(UserControl thumbnail, string mainTitle, 
        string subTitle, string detailInfo, Form detail)
    {
      Thumbnail = thumbnail;
      MainTitle = mainTitle;
      SubTitle = subTitle;
      DetailInfo = detailInfo;
      Detail = detail;
    }
  }



  public partial class MainForm : Form
  {
    private List<ComponentData> components_; // 用于保存组件数据
    public MainForm()
    {
      InitializeComponent();
      components_ = new List<ComponentData>(); // 初始化列表

      AddOverviewChart();
      AddSpindle();
      AddCutMotor();
      AddTransferRackMotor();

      //TESTAddCharts();
      //TESTAddPanel();
      //AddDemoQuadChart();
      //AddDemo3Chart();
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

      cut_motor = new Components.TransferRack("TransferRack Y1");
      detail_string = $@"
電流 : 0.175 A
附載 : 16%";
      componentdemo = new ComponentData(
          cut_motor, "TransferRack Y1", "34%", detail_string, cut_motor.GetDetailForm());
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

    private void TESTAddPanel()
    {
      // 测试添加 10 个 PartInfoPanel
      for (int i = 0; i < 2; i++)
      {
        Random _random = new();
        var newValue = _random.Next(15, 25);
        // 创建一个新的 PartBase
        var autoUpdatePanel = new PartBase();

        // 创建一个新的 ComponentData
        var component = new ComponentData(
            autoUpdatePanel, // 缩略图使用 PartBase
            $"主标题 {i + 1}",    // 示例主标题
            $"副标题 {i + 1}",     // 示例副标题
            $@"
Query Speed: {newValue + 1} RPM
Query Status: Normal
Query Internal Status: OK
Query Power: {newValue * 1.2:F1} kW
Query Bus Voltage: {newValue * 2.3:F1} V
Query Current: {newValue * 0.8:F1} A
Query Motor Temperature: {20 + newValue / 10} °C
Query Inverter Temperature: {25 + newValue / 15} °C
", // 详细文本
           autoUpdatePanel.GetDetailForm()  // 详细图像列表
        );
        // 将数据添加到保存列表
        // 显示到界面
        AddComponentChart(component);
      }

    }


    private void AddDemo3Chart()
    {
      var demo_ui = new DemoComponentLeftOneRightTwo();
      var newValuei = 123;
      var ii = 3;
      var componentdemo = new ComponentData(
          demo_ui, // 缩略图使用 PartBase
          $"主軸",    // 示例主标题
          $"14% 37500rpm",     // 示例副标题
          $@"
Query Speed: {newValuei + 1} RPM
Query Status: Normal
Query Internal Status: OK
Query Power: {newValuei * 1.2:F1} kW
Query Bus Voltage: {newValuei * 2.3:F1} V
Query Current: {newValuei * 0.8:F1} A
Query Motor Temperature: {20 + newValuei / 10} °C
Query Inverter Temperature: {25 + newValuei / 15} °C
", // 详细文本
         demo_ui.GetDetailForm()  // 详细图像列表
      );
      // 将数据添加到保存列表
      // 显示到界面
      AddComponentChart(componentdemo);
    }

    private void AddDemoQuadChart()
    {
      var demo_ui = new DemoComponentQuadGrid();
      var newValuei = 123;
      var componentdemo = new ComponentData(
          demo_ui, // 缩略图使用 PartBase
          $"Y1",    // 示例主标题
          $"16% ",     // 示例副标题
          $@"
Query Speed: {newValuei + 1} RPM
Query Status: Normal
Query Internal Status: OK
Query Power: {newValuei * 1.2:F1} kW
Query Bus Voltage: {newValuei * 2.3:F1} V
Query Current: {newValuei * 0.8:F1} A
Query Motor Temperature: {20 + newValuei / 10} °C
Query Inverter Temperature: {25 + newValuei / 15} °C
", // 详细文本
         demo_ui.GetDetailForm()  // 详细图像列表
      );
      // 将数据添加到保存列表
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
      // 显示到界面
      AddComponentChart(componentdemo);
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

        // 将详细文本信息填入 DetailTextPanel
        //DetailTextPanel.Controls.Clear(); // 清空 DetailTextPanel 之前的内容
        var detailLabel = new Label
        {
          Text = info.DetailInfo, // 显示详细信息
          Font = new Font("Arial", 12), // 设置字体
          Dock = DockStyle.Fill,
          TextAlign = ContentAlignment.MiddleLeft, // 左对齐
          AutoSize = false,
          Padding = new Padding(10) // 添加一些内边距
        };
        DetailTextPanel.Controls.Add(detailLabel); // 添加 Label 到 DetailTextPanel
        DetailTextPanel.Show();
      };
      partInfoPanel.Click += callback;

      // 初始化時執行顯示邏輯（判斷 DetailTextPanel 是否为空）
      if (DetailTextPanel.Controls.Count == 0)
      {
        callback(null, EventArgs.Empty); // 调用回调函数直接显示内容
      }
    }

    public void AddComponentChart(ComponentData info)
    {
      components_.Add(info);

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


    private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
    {

    }

    private void PartInfoPanel_MouseEnter(object sender, EventArgs e)
    {

    }

    private void button1_Click(object sender, EventArgs e)
    {
    }

    private void DetailTextPanel_Paint(object sender, PaintEventArgs e)
    {

    }

    private void DetailTextPanel_Click(object sender, EventArgs e)
    {

      MessageBox.Show("CLICK");
    }

    private void DetailTextPanel_MouseHover(object sender, EventArgs e)
    {
      //this.BackColor = Color.LightBlue;
    }

    private void DetailTextPanel_MouseLeave(object sender, EventArgs e)
    {

      //this.BackColor = Color.LightGray;
    }

    private void SummaryLabel_Click(object sender, EventArgs e)
    {

    }
  }








}
