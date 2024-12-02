using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveCharts.Wpf;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace LoadMonitor
{
  public enum DetailLayout
  {
    Single,        // 单图布局
    LeftRight,     // 左右布局
    LeftOneRightTwo // 左一右二布局
  }

  public struct ComponentData
  {
    public UserControl Thumbnail; // 缩图图像
    public string MainTitle; // 主标题
    public string SubTitle; // 副标题
    public string DetailInfo;     // 详细信息
    public Form Detail; // 詳細图像 

    //public List<UserControl> Details; // 详细图像列表

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
    private AutoUpdatePanel autoupdatepanel_; // 子窗體
    private AngularGauge a;
    private View view;

    private List<ComponentData> components_; // 用于保存组件数据
    public MainForm()
    {
      InitializeComponent();

      components_ = new List<ComponentData>(); // 初始化列表
      //TESTAddCharts();
      TESTAddPanel();

    }

    private void TESTAddPanel()
    {
      // 测试添加 10 个 PartInfoPanel
      for (int i = 0; i < 3; i++)
      {
        Random _random = new();
        var newValue = _random.Next(15, 25);
        // 创建一个新的 AutoUpdatePanel
        var autoUpdatePanel = new AutoUpdatePanel();

        // 创建一个新的 ComponentData
        var component = new ComponentData(
            autoUpdatePanel, // 缩略图使用 AutoUpdatePanel
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
        components_.Add(component);
        // 显示到界面
        AddComponentChart(component);
      }

      AddDemoChart();
      AddDemoQuadChart();
      AddDemo3Chart();
    }


    private void AddDemo3Chart()
    {
      var demo_ui = new DemoComponentLeftOneRightTwo();
      var newValuei = 123;
      var ii = 3;
      var componentdemo = new ComponentData(
          demo_ui, // 缩略图使用 AutoUpdatePanel
          $"主标题 {ii + 1}",    // 示例主标题
          $"副标题 {ii + 1}",     // 示例副标题
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
      components_.Add(componentdemo);
      // 显示到界面
      AddComponentChart(componentdemo);
    }

    private void AddDemoQuadChart()
    {
      var demo_ui = new DemoComponentQuadGrid();
      var newValuei = 123;
      var ii = 3;
      var componentdemo = new ComponentData(
          demo_ui, // 缩略图使用 AutoUpdatePanel
          $"主标题 {ii + 1}",    // 示例主标题
          $"副标题 {ii + 1}",     // 示例副标题
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
      components_.Add(componentdemo);
      // 显示到界面
      AddComponentChart(componentdemo);
    }

    private void AddDemoChart()
    {
      var demo_ui = new DemoComponent();
      var newValuei = 123;
      var ii = 3;
      var componentdemo = new ComponentData(
          demo_ui, // 缩略图使用 AutoUpdatePanel
          $"主标题 {ii + 1}",    // 示例主标题
          $"副标题 {ii + 1}",     // 示例副标题
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
      components_.Add(componentdemo);
      // 显示到界面
      AddComponentChart(componentdemo);
    }



    private Button AddHoverAnimation(ComponentData info)
    {
      // 创建一个透明的 Button，覆盖整个 PartInfoPanel
      var hoverButton = new Button
      {
        Dock = DockStyle.Fill, // 填满整个 PartInfoPanel
        BackColor = Color.Transparent, // 背景透明
        FlatStyle = FlatStyle.Flat, // 去掉边框
        Text = string.Empty, // 不显示文字
        Cursor = Cursors.Hand, // 鼠标变为手型
      };

      // 去掉按钮的焦点和视觉效果
      hoverButton.FlatAppearance.BorderSize = 0;
      hoverButton.FlatAppearance.MouseDownBackColor = Color.DodgerBlue; // 点击时的背景颜色
      hoverButton.FlatAppearance.MouseOverBackColor = Color.LightBlue; // Hover 时的背景颜色

      // Button 的点击事件
      hoverButton.Click += (object? sender, EventArgs e) =>
      {

        // 遍历 Detail 列表，将所有图像添加到 DetailChartPanel
        if (info.Detail != null) // 确保 Detail 列表不为空
        {
          info.Detail.TopLevel = false;
          info.Detail.Dock = DockStyle.Top; // 每个图像按照顶部对齐方式堆叠
          //info.Detail.Height = 150; // 设置每个图像的高度（可根据需求调整）
          DetailChartPanel.Controls.Add(info.Detail);
          // 显示 Form 内容
          info.Detail.Show();
        }

        // 将详细文本信息填入 DetailTextPanel
        DetailTextPanel.Controls.Clear(); // 清空 DetailTextPanel 之前的内容
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


      };


      return hoverButton;
    }


    public void AddComponentChart(ComponentData info)
    {
      // 创建一个新的 PartInfoPanel 容器
      var partInfoPanel = new Panel
      {
        Width = PartInfoPanel.Width, // 使用设计器中 PartInfoPanel 的宽度
        Height = PartInfoPanel.Height, // 使用设计器中 PartInfoPanel 的高度
        Margin = new Padding(5),
        BackColor = Color.Transparent // 背景透明
      };

      // *** 确保先添加 hoverButton ***
      partInfoPanel.Controls.Add(AddHoverAnimation(info)); // 将 hoverButton 添加到 Panel 的最底层

      // 创建一个新的 ThumbnailPanel
      var thumbnailPanel = new Panel
      {
        Width = ThumbnailPanel.Width, // 使用设计器中 ThumbnailPanel 的宽度
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
        Dock = DockStyle.Fill,
        BackColor = Color.Transparent // 避免覆盖 Panel 的透明背景
      };

      // 添加主标题 Label
      var titleLabel = new Label
      {
        Text = info.MainTitle, // 主标题内容
        Font = TitleLabel.Font, // 使用设计器中 TitleLabel 的字体
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
        Dock = DockStyle.Top,
        TextAlign = SummaryLabel.TextAlign, // 使用设计器中 SummaryLabel 的对齐方式
        AutoSize = true, // 设置自动调整大小
        MaximumSize = new Size(infoPanel.Width, 0), // 限制最大宽度，允许换行
      };
      summaryLabel.Visible = true;

      // 将主标题和副标题添加到 InfoPanel
      infoPanel.Controls.Add(summaryLabel); // 添加副标题
      infoPanel.Controls.Add(titleLabel);   // 添加主标题
      infoPanel.BringToFront();
      // 将 ThumbnailPanel 和 InfoPanel 添加到 PartInfoPanel
      partInfoPanel.Controls.Add(thumbnailPanel);
      partInfoPanel.Controls.Add(infoPanel);
      // 将 PartInfoPanel 添加到 FlowLayoutPanel
      flowLayoutPanel1.Controls.Add(partInfoPanel);
    }



    private void TESTAddCharts()
    {
      // 創建並嵌入 AutoUpdateForm
      autoupdatepanel_ = new AutoUpdatePanel
      {
        Dock = DockStyle.Fill // 填滿容器
      };
      panel1.Controls.Add(autoupdatepanel_); // 將子窗體添加到 panel1
      autoupdatepanel_.Show(); // 顯示子窗體

      view = new View()
      {
        Dock = DockStyle.Fill,
      };

      panel3.Controls.Add(view);
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
  }








}
