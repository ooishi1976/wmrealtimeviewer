using RealtimeViewer.Model;
using RealtimeViewer.Controls;

namespace RealtimeViewer
{
    partial class MainForm
	{
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナで生成されたコード

		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.timerGPSDraw = new System.Windows.Forms.Timer(this.components);
            this.labelUpdateDateCaption = new System.Windows.Forms.Label();
            this.labelUpdateDate = new System.Windows.Forms.Label();
            this.labelCarListCaption = new System.Windows.Forms.Label();
            this.gridCarList = new System.Windows.Forms.DataGridView();
            this.carListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panelBase = new System.Windows.Forms.Panel();
            this.panelMap = new System.Windows.Forms.Panel();
            this.mpgMap = new MpgMap.MpgMap();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.tabControlRtSelect = new System.Windows.Forms.TabControl();
            this.tabPageRT = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelStreamingOnCar = new System.Windows.Forms.TableLayoutPanel();
            this.labelStreamingOnCarCaption = new System.Windows.Forms.Label();
            this.labelCarStreamElapsedCaption = new System.Windows.Forms.Label();
            this.labelCarStreamFramesCaption = new System.Windows.Forms.Label();
            this.labelCarStreamByteCaption = new System.Windows.Forms.Label();
            this.labelCarStreamFpsCaption = new System.Windows.Forms.Label();
            this.labelCarStreamKbpsCaption = new System.Windows.Forms.Label();
            this.labelCarStreamDropsCaption = new System.Windows.Forms.Label();
            this.labelCarStreamSpeedCaption = new System.Windows.Forms.Label();
            this.labelCarStreamElapsed = new System.Windows.Forms.Label();
            this.labelCarStreamFrames = new System.Windows.Forms.Label();
            this.labelCarStreamBytes = new System.Windows.Forms.Label();
            this.labelCarStreamFps = new System.Windows.Forms.Label();
            this.labelCarStreamKbps = new System.Windows.Forms.Label();
            this.labelCarStreamDrops = new System.Windows.Forms.Label();
            this.labelCarStreamSpeed = new System.Windows.Forms.Label();
            this.tableLayoutPanelRealTimePlayer = new System.Windows.Forms.TableLayoutPanel();
            this.buttonRtStart = new System.Windows.Forms.Button();
            this.buttonRtStop = new System.Windows.Forms.Button();
            this.labelRtStatus = new System.Windows.Forms.Label();
            this.labelRtElapsedCaption = new System.Windows.Forms.Label();
            this.labelRtElapsed = new System.Windows.Forms.Label();
            this.progressBarRtStart = new System.Windows.Forms.ProgressBar();
            this.labelRtRetryStatus = new System.Windows.Forms.Label();
            this.labelStreamingSessionRetry = new System.Windows.Forms.Label();
            this.radioButtonRtCh1 = new System.Windows.Forms.RadioButton();
            this.radioButtonRtCh2 = new System.Windows.Forms.RadioButton();
            this.radioButtonRtCh3 = new System.Windows.Forms.RadioButton();
            this.radioButtonRtCh4 = new System.Windows.Forms.RadioButton();
            this.radioButtonRtCh5 = new System.Windows.Forms.RadioButton();
            this.radioButtonRtCh6 = new System.Windows.Forms.RadioButton();
            this.radioButtonRtCh7 = new System.Windows.Forms.RadioButton();
            this.radioButtonRtCh8 = new System.Windows.Forms.RadioButton();
            this.tabPageEvent = new System.Windows.Forms.TabPage();
            this.buttonShowDrivingMovie = new System.Windows.Forms.Button();
            this.tabPageRemoteConfig = new System.Windows.Forms.TabPage();
            this.buttonShowRemoteSetting = new System.Windows.Forms.Button();
            this.tableLayoutPanelCarInfo = new System.Windows.Forms.TableLayoutPanel();
            this.buttonLeftPanelClose = new System.Windows.Forms.Button();
            this.labelCarIdCaption = new System.Windows.Forms.Label();
            this.labelCarStatusCaption = new System.Windows.Forms.Label();
            this.labelRtCarId = new System.Windows.Forms.Label();
            this.labelRtCarStatus = new System.Windows.Forms.Label();
            this.zoomBar = new System.Windows.Forms.TrackBar();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.tableLayoutPanelHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panelCarDisplayMode = new System.Windows.Forms.Panel();
            this.radioButtonSelect = new System.Windows.Forms.RadioButton();
            this.radioButtonALL = new System.Windows.Forms.RadioButton();
            this.comboBoxOffice = new System.Windows.Forms.ComboBox();
            this.labelUserName = new System.Windows.Forms.Label();
            this.labelUserNameCaption = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelEventList = new System.Windows.Forms.TableLayoutPanel();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.buttonEventListUpdate = new System.Windows.Forms.Button();
            this.progressBarEventListUpdate = new System.Windows.Forms.ProgressBar();
            this.buttonGetG = new System.Windows.Forms.Button();
            this.buttonCancelG = new System.Windows.Forms.Button();
            this.progressBarG = new System.Windows.Forms.ProgressBar();
            this.labelGstatus = new System.Windows.Forms.Label();
            this.progressBarDownload = new System.Windows.Forms.ProgressBar();
            this.labelDownloadStatus = new System.Windows.Forms.Label();
            this.panelPlay = new System.Windows.Forms.Panel();
            this.tableLayoutPanelPlay = new System.Windows.Forms.TableLayoutPanel();
            this.labelEventProc = new System.Windows.Forms.Label();
            this.labelPanelPlayCarIdCaption = new System.Windows.Forms.Label();
            this.buttonPlay7 = new System.Windows.Forms.Button();
            this.buttonPlay6 = new System.Windows.Forms.Button();
            this.buttonPlay5 = new System.Windows.Forms.Button();
            this.buttonPlay1 = new System.Windows.Forms.Button();
            this.buttonPlay2 = new System.Windows.Forms.Button();
            this.buttonPlay3 = new System.Windows.Forms.Button();
            this.labelPanelPlayCarId = new System.Windows.Forms.Label();
            this.buttonPlay4 = new System.Windows.Forms.Button();
            this.buttonPlay8 = new System.Windows.Forms.Button();
            this.labelPanelPlayDateCaption = new System.Windows.Forms.Label();
            this.labelPanelPlayDate = new System.Windows.Forms.Label();
            this.gridEventList = new System.Windows.Forms.DataGridView();
            this.ColumnCheck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.timestampDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.carIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.movieTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MovieId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eventInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.labelEventListCaption = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonUpdateDeviceListUseDebugOnly = new System.Windows.Forms.Button();
            this.timerStartMQTT = new System.Windows.Forms.Timer(this.components);
            this.sqliteCommand1 = new Microsoft.Data.Sqlite.SqliteCommand();
            this.comboBoxServerEnv = new System.Windows.Forms.ComboBox();
            this.buttonEventDubug = new System.Windows.Forms.Button();
            this.timerStreamingPreparation = new System.Windows.Forms.Timer(this.components);
            this.comboBoxDebugUser = new System.Windows.Forms.ComboBox();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.pictureBoxWarn = new System.Windows.Forms.PictureBox();
            this.panelErrorIcon = new System.Windows.Forms.Panel();
            this.labelServerEnv = new System.Windows.Forms.Label();
            this.bindingSourceErrorInfomation = new System.Windows.Forms.BindingSource(this.components);
            this.officeInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.deviceIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridCarList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.carListBindingSource)).BeginInit();
            this.tabControlMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panelBase.SuspendLayout();
            this.panelMap.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.tabControlRtSelect.SuspendLayout();
            this.tabPageRT.SuspendLayout();
            this.tableLayoutPanelStreamingOnCar.SuspendLayout();
            this.tableLayoutPanelRealTimePlayer.SuspendLayout();
            this.tabPageEvent.SuspendLayout();
            this.tabPageRemoteConfig.SuspendLayout();
            this.tableLayoutPanelCarInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomBar)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.tableLayoutPanelHeader.SuspendLayout();
            this.panelCarDisplayMode.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanelEventList.SuspendLayout();
            this.panelPlay.SuspendLayout();
            this.tableLayoutPanelPlay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEventList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventInfoBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarn)).BeginInit();
            this.panelErrorIcon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceErrorInfomation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.officeInfoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // menuNew
            // 
            this.menuNew.Name = "menuNew";
            this.menuNew.Size = new System.Drawing.Size(32, 19);
            // 
            // menuOpen
            // 
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.Size = new System.Drawing.Size(32, 19);
            // 
            // menuSave
            // 
            this.menuSave.Name = "menuSave";
            this.menuSave.Size = new System.Drawing.Size(32, 19);
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Name = "menuSaveAs";
            this.menuSaveAs.Size = new System.Drawing.Size(32, 19);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 6);
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(32, 19);
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(32, 19);
            // 
            // timerGPSDraw
            // 
            this.timerGPSDraw.Interval = 15000;
            this.timerGPSDraw.Tick += new System.EventHandler(this.timerGPSDraw_Tick);
            // 
            // labelUpdateDateCaption
            // 
            this.labelUpdateDateCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUpdateDateCaption.AutoSize = true;
            this.labelUpdateDateCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelUpdateDateCaption.Location = new System.Drawing.Point(543, 0);
            this.labelUpdateDateCaption.Name = "labelUpdateDateCaption";
            this.labelUpdateDateCaption.Size = new System.Drawing.Size(96, 50);
            this.labelUpdateDateCaption.TabIndex = 2;
            this.labelUpdateDateCaption.Text = "更新日時：";
            this.labelUpdateDateCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelUpdateDate
            // 
            this.labelUpdateDate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUpdateDate.AutoSize = true;
            this.labelUpdateDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelUpdateDate.Location = new System.Drawing.Point(645, 0);
            this.labelUpdateDate.Name = "labelUpdateDate";
            this.labelUpdateDate.Size = new System.Drawing.Size(188, 50);
            this.labelUpdateDate.TabIndex = 3;
            this.labelUpdateDate.Text = "初期車両データ取得中";
            this.labelUpdateDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCarListCaption
            // 
            this.labelCarListCaption.AutoSize = true;
            this.labelCarListCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarListCaption.Location = new System.Drawing.Point(12, 18);
            this.labelCarListCaption.Name = "labelCarListCaption";
            this.labelCarListCaption.Size = new System.Drawing.Size(152, 24);
            this.labelCarListCaption.TabIndex = 4;
            this.labelCarListCaption.Text = "運行中車両リスト";
            // 
            // gridCarList
            // 
            this.gridCarList.AllowUserToAddRows = false;
            this.gridCarList.AllowUserToDeleteRows = false;
            this.gridCarList.AllowUserToResizeColumns = false;
            this.gridCarList.AllowUserToResizeRows = false;
            this.gridCarList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridCarList.AutoGenerateColumns = false;
            this.gridCarList.ColumnHeadersHeight = 46;
            this.gridCarList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridCarList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.deviceIdDataGridViewTextBoxColumn,
            this.addressDataGridViewTextBoxColumn});
            this.gridCarList.DataSource = this.carListBindingSource;
            this.gridCarList.Enabled = false;
            this.gridCarList.Location = new System.Drawing.Point(12, 47);
            this.gridCarList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridCarList.MultiSelect = false;
            this.gridCarList.Name = "gridCarList";
            this.gridCarList.ReadOnly = true;
            this.gridCarList.RowHeadersVisible = false;
            this.gridCarList.RowHeadersWidth = 20;
            this.gridCarList.RowTemplate.Height = 21;
            this.gridCarList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridCarList.Size = new System.Drawing.Size(296, 901);
            this.gridCarList.TabIndex = 9;
            this.gridCarList.TabStop = false;
            this.gridCarList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridCarList_CellDoubleClick);
            this.gridCarList.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.GridCarList_ColumnHeaderMouseClick);
            // 
            // carListBindingSource
            // 
            this.carListBindingSource.DataSource = typeof(RealtimeViewer.Model.MapEntryInfo);
            this.carListBindingSource.Sort = "";
            this.carListBindingSource.CurrentChanged += new System.EventHandler(this.CarListBindingSource_CurrentChanged);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.Controls.Add(this.tabPage1);
            this.tabControlMain.Controls.Add(this.tabPage2);
            this.tabControlMain.Enabled = false;
            this.tabControlMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabControlMain.Location = new System.Drawing.Point(327, 18);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1445, 930);
            this.tabControlMain.TabIndex = 12;
            this.tabControlMain.TabStop = false;
            this.tabControlMain.Selected += new System.Windows.Forms.TabControlEventHandler(this.TabControlMain_Selected);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panelBase);
            this.tabPage1.Controls.Add(this.zoomBar);
            this.tabPage1.Controls.Add(this.panelHeader);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1437, 892);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "現在地情報";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panelBase
            // 
            this.panelBase.AutoSize = true;
            this.panelBase.Controls.Add(this.panelMap);
            this.panelBase.Controls.Add(this.panelLeft);
            this.panelBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBase.Location = new System.Drawing.Point(3, 53);
            this.panelBase.MinimumSize = new System.Drawing.Size(100, 100);
            this.panelBase.Name = "panelBase";
            this.panelBase.Size = new System.Drawing.Size(1386, 836);
            this.panelBase.TabIndex = 15;
            // 
            // panelMap
            // 
            this.panelMap.AutoSize = true;
            this.panelMap.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelMap.Controls.Add(this.mpgMap);
            this.panelMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMap.Location = new System.Drawing.Point(240, 0);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(1146, 836);
            this.panelMap.TabIndex = 1;
            // 
            // mpgMap
            // 
            this.mpgMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mpgMap.CenterMarker = false;
            this.mpgMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpgMap.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mpgMap.Location = new System.Drawing.Point(0, 0);
            this.mpgMap.MapAngle = 0F;
            this.mpgMap.MapCenter = new System.Drawing.Point(503209660, 128494390);
            this.mpgMap.MapMode = MpgMap.MapMode.Move;
            this.mpgMap.MapPath = "latest";
            this.mpgMap.MapScale = 100000;
            this.mpgMap.Margin = new System.Windows.Forms.Padding(5);
            this.mpgMap.Name = "mpgMap";
            this.mpgMap.Size = new System.Drawing.Size(1146, 836);
            this.mpgMap.StyleName = "標準";
            this.mpgMap.StylePath = "latest";
            this.mpgMap.TabIndex = 3;
            this.mpgMap.UserMarker = false;
            this.mpgMap.UserMarkerPosition = new System.Drawing.Point(430000000, 72000000);
            this.mpgMap.CustomObjectHit += new MpgMap.MpgMap.CustomObjectHitEventHandler(this.MpgMap_CustomObjectHit);
            this.mpgMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MpgMap_MouseDown);
            this.mpgMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MpgMap_MouseMove);
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.CornflowerBlue;
            this.panelLeft.Controls.Add(this.tabControlRtSelect);
            this.panelLeft.Controls.Add(this.tableLayoutPanelCarInfo);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(240, 836);
            this.panelLeft.TabIndex = 0;
            // 
            // tabControlRtSelect
            // 
            this.tabControlRtSelect.Alignment = System.Windows.Forms.TabAlignment.Right;
            this.tabControlRtSelect.Controls.Add(this.tabPageRT);
            this.tabControlRtSelect.Controls.Add(this.tabPageEvent);
            this.tabControlRtSelect.Controls.Add(this.tabPageRemoteConfig);
            this.tabControlRtSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlRtSelect.Location = new System.Drawing.Point(0, 100);
            this.tabControlRtSelect.Multiline = true;
            this.tabControlRtSelect.Name = "tabControlRtSelect";
            this.tabControlRtSelect.SelectedIndex = 0;
            this.tabControlRtSelect.Size = new System.Drawing.Size(240, 736);
            this.tabControlRtSelect.TabIndex = 1;
            // 
            // tabPageRT
            // 
            this.tabPageRT.BackColor = System.Drawing.Color.CornflowerBlue;
            this.tabPageRT.Controls.Add(this.tableLayoutPanelStreamingOnCar);
            this.tabPageRT.Controls.Add(this.tableLayoutPanelRealTimePlayer);
            this.tabPageRT.Location = new System.Drawing.Point(4, 4);
            this.tabPageRT.Name = "tabPageRT";
            this.tabPageRT.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRT.Size = new System.Drawing.Size(208, 728);
            this.tabPageRT.TabIndex = 0;
            this.tabPageRT.Text = "リアルタイム再生";
            // 
            // tableLayoutPanelStreamingOnCar
            // 
            this.tableLayoutPanelStreamingOnCar.ColumnCount = 2;
            this.tableLayoutPanelStreamingOnCar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelStreamingOnCar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelStreamingOnCarCaption, 0, 0);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamElapsedCaption, 0, 1);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamFramesCaption, 0, 2);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamByteCaption, 0, 3);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamFpsCaption, 0, 4);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamKbpsCaption, 0, 5);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamDropsCaption, 0, 6);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamSpeedCaption, 0, 7);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamElapsed, 1, 1);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamFrames, 1, 2);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamBytes, 1, 3);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamFps, 1, 4);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamKbps, 1, 5);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamDrops, 1, 6);
            this.tableLayoutPanelStreamingOnCar.Controls.Add(this.labelCarStreamSpeed, 1, 7);
            this.tableLayoutPanelStreamingOnCar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanelStreamingOnCar.Location = new System.Drawing.Point(3, 529);
            this.tableLayoutPanelStreamingOnCar.Name = "tableLayoutPanelStreamingOnCar";
            this.tableLayoutPanelStreamingOnCar.RowCount = 9;
            this.tableLayoutPanelStreamingOnCar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelStreamingOnCar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelStreamingOnCar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelStreamingOnCar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelStreamingOnCar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelStreamingOnCar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelStreamingOnCar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelStreamingOnCar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelStreamingOnCar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelStreamingOnCar.Size = new System.Drawing.Size(202, 196);
            this.tableLayoutPanelStreamingOnCar.TabIndex = 1;
            this.tableLayoutPanelStreamingOnCar.Visible = false;
            // 
            // labelStreamingOnCarCaption
            // 
            this.labelStreamingOnCarCaption.AutoSize = true;
            this.tableLayoutPanelStreamingOnCar.SetColumnSpan(this.labelStreamingOnCarCaption, 2);
            this.labelStreamingOnCarCaption.Location = new System.Drawing.Point(3, 0);
            this.labelStreamingOnCarCaption.Name = "labelStreamingOnCarCaption";
            this.labelStreamingOnCarCaption.Size = new System.Drawing.Size(147, 20);
            this.labelStreamingOnCarCaption.TabIndex = 0;
            this.labelStreamingOnCarCaption.Text = "車載器からサーバーへ";
            // 
            // labelCarStreamElapsedCaption
            // 
            this.labelCarStreamElapsedCaption.AutoSize = true;
            this.labelCarStreamElapsedCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamElapsedCaption.Location = new System.Drawing.Point(3, 20);
            this.labelCarStreamElapsedCaption.Name = "labelCarStreamElapsedCaption";
            this.labelCarStreamElapsedCaption.Size = new System.Drawing.Size(64, 17);
            this.labelCarStreamElapsedCaption.TabIndex = 1;
            this.labelCarStreamElapsedCaption.Text = "送信経過";
            // 
            // labelCarStreamFramesCaption
            // 
            this.labelCarStreamFramesCaption.AutoSize = true;
            this.labelCarStreamFramesCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamFramesCaption.Location = new System.Drawing.Point(3, 40);
            this.labelCarStreamFramesCaption.Name = "labelCarStreamFramesCaption";
            this.labelCarStreamFramesCaption.Size = new System.Drawing.Size(78, 17);
            this.labelCarStreamFramesCaption.TabIndex = 2;
            this.labelCarStreamFramesCaption.Text = "送信フレーム";
            // 
            // labelCarStreamByteCaption
            // 
            this.labelCarStreamByteCaption.AutoSize = true;
            this.labelCarStreamByteCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamByteCaption.Location = new System.Drawing.Point(3, 60);
            this.labelCarStreamByteCaption.Name = "labelCarStreamByteCaption";
            this.labelCarStreamByteCaption.Size = new System.Drawing.Size(77, 17);
            this.labelCarStreamByteCaption.TabIndex = 3;
            this.labelCarStreamByteCaption.Text = "送信Kバイト";
            // 
            // labelCarStreamFpsCaption
            // 
            this.labelCarStreamFpsCaption.AutoSize = true;
            this.labelCarStreamFpsCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamFpsCaption.Location = new System.Drawing.Point(3, 80);
            this.labelCarStreamFpsCaption.Name = "labelCarStreamFpsCaption";
            this.labelCarStreamFpsCaption.Size = new System.Drawing.Size(27, 17);
            this.labelCarStreamFpsCaption.TabIndex = 4;
            this.labelCarStreamFpsCaption.Text = "fps";
            // 
            // labelCarStreamKbpsCaption
            // 
            this.labelCarStreamKbpsCaption.AutoSize = true;
            this.labelCarStreamKbpsCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamKbpsCaption.Location = new System.Drawing.Point(3, 100);
            this.labelCarStreamKbpsCaption.Name = "labelCarStreamKbpsCaption";
            this.labelCarStreamKbpsCaption.Size = new System.Drawing.Size(38, 17);
            this.labelCarStreamKbpsCaption.TabIndex = 5;
            this.labelCarStreamKbpsCaption.Text = "kbps";
            // 
            // labelCarStreamDropsCaption
            // 
            this.labelCarStreamDropsCaption.AutoSize = true;
            this.labelCarStreamDropsCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamDropsCaption.Location = new System.Drawing.Point(3, 120);
            this.labelCarStreamDropsCaption.Name = "labelCarStreamDropsCaption";
            this.labelCarStreamDropsCaption.Size = new System.Drawing.Size(47, 17);
            this.labelCarStreamDropsCaption.TabIndex = 6;
            this.labelCarStreamDropsCaption.Text = "ドロップ";
            // 
            // labelCarStreamSpeedCaption
            // 
            this.labelCarStreamSpeedCaption.AutoSize = true;
            this.labelCarStreamSpeedCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamSpeedCaption.Location = new System.Drawing.Point(3, 140);
            this.labelCarStreamSpeedCaption.Name = "labelCarStreamSpeedCaption";
            this.labelCarStreamSpeedCaption.Size = new System.Drawing.Size(64, 17);
            this.labelCarStreamSpeedCaption.TabIndex = 7;
            this.labelCarStreamSpeedCaption.Text = "送信速度";
            // 
            // labelCarStreamElapsed
            // 
            this.labelCarStreamElapsed.AutoEllipsis = true;
            this.labelCarStreamElapsed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCarStreamElapsed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamElapsed.Location = new System.Drawing.Point(104, 20);
            this.labelCarStreamElapsed.Name = "labelCarStreamElapsed";
            this.labelCarStreamElapsed.Size = new System.Drawing.Size(95, 20);
            this.labelCarStreamElapsed.TabIndex = 8;
            this.labelCarStreamElapsed.Text = "label20";
            this.labelCarStreamElapsed.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelCarStreamFrames
            // 
            this.labelCarStreamFrames.AutoEllipsis = true;
            this.labelCarStreamFrames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCarStreamFrames.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamFrames.Location = new System.Drawing.Point(104, 40);
            this.labelCarStreamFrames.Name = "labelCarStreamFrames";
            this.labelCarStreamFrames.Size = new System.Drawing.Size(95, 20);
            this.labelCarStreamFrames.TabIndex = 9;
            this.labelCarStreamFrames.Text = "label20";
            this.labelCarStreamFrames.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelCarStreamBytes
            // 
            this.labelCarStreamBytes.AutoEllipsis = true;
            this.labelCarStreamBytes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCarStreamBytes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamBytes.Location = new System.Drawing.Point(104, 60);
            this.labelCarStreamBytes.Name = "labelCarStreamBytes";
            this.labelCarStreamBytes.Size = new System.Drawing.Size(95, 20);
            this.labelCarStreamBytes.TabIndex = 10;
            this.labelCarStreamBytes.Text = "label20";
            this.labelCarStreamBytes.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelCarStreamFps
            // 
            this.labelCarStreamFps.AutoEllipsis = true;
            this.labelCarStreamFps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCarStreamFps.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamFps.Location = new System.Drawing.Point(104, 80);
            this.labelCarStreamFps.Name = "labelCarStreamFps";
            this.labelCarStreamFps.Size = new System.Drawing.Size(95, 20);
            this.labelCarStreamFps.TabIndex = 11;
            this.labelCarStreamFps.Text = "label20";
            this.labelCarStreamFps.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelCarStreamKbps
            // 
            this.labelCarStreamKbps.AutoEllipsis = true;
            this.labelCarStreamKbps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCarStreamKbps.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamKbps.Location = new System.Drawing.Point(104, 100);
            this.labelCarStreamKbps.Name = "labelCarStreamKbps";
            this.labelCarStreamKbps.Size = new System.Drawing.Size(95, 20);
            this.labelCarStreamKbps.TabIndex = 12;
            this.labelCarStreamKbps.Text = "label20";
            this.labelCarStreamKbps.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelCarStreamDrops
            // 
            this.labelCarStreamDrops.AutoEllipsis = true;
            this.labelCarStreamDrops.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCarStreamDrops.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamDrops.Location = new System.Drawing.Point(104, 120);
            this.labelCarStreamDrops.Name = "labelCarStreamDrops";
            this.labelCarStreamDrops.Size = new System.Drawing.Size(95, 20);
            this.labelCarStreamDrops.TabIndex = 13;
            this.labelCarStreamDrops.Text = "label20";
            this.labelCarStreamDrops.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelCarStreamSpeed
            // 
            this.labelCarStreamSpeed.AutoEllipsis = true;
            this.labelCarStreamSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCarStreamSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelCarStreamSpeed.Location = new System.Drawing.Point(104, 140);
            this.labelCarStreamSpeed.Name = "labelCarStreamSpeed";
            this.labelCarStreamSpeed.Size = new System.Drawing.Size(95, 20);
            this.labelCarStreamSpeed.TabIndex = 14;
            this.labelCarStreamSpeed.Text = "label20";
            this.labelCarStreamSpeed.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tableLayoutPanelRealTimePlayer
            // 
            this.tableLayoutPanelRealTimePlayer.AutoSize = true;
            this.tableLayoutPanelRealTimePlayer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelRealTimePlayer.ColumnCount = 2;
            this.tableLayoutPanelRealTimePlayer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelRealTimePlayer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.buttonRtStart, 0, 0);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.buttonRtStop, 0, 1);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.labelRtStatus, 0, 2);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.labelRtElapsedCaption, 0, 14);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.labelRtElapsed, 1, 14);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.progressBarRtStart, 1, 0);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.labelRtRetryStatus, 0, 3);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.labelStreamingSessionRetry, 0, 4);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.radioButtonRtCh1, 0, 5);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.radioButtonRtCh2, 0, 6);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.radioButtonRtCh3, 0, 7);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.radioButtonRtCh4, 0, 8);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.radioButtonRtCh5, 0, 9);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.radioButtonRtCh6, 0, 10);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.radioButtonRtCh7, 0, 11);
            this.tableLayoutPanelRealTimePlayer.Controls.Add(this.radioButtonRtCh8, 0, 12);
            this.tableLayoutPanelRealTimePlayer.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelRealTimePlayer.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelRealTimePlayer.Name = "tableLayoutPanelRealTimePlayer";
            this.tableLayoutPanelRealTimePlayer.RowCount = 15;
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelRealTimePlayer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelRealTimePlayer.Size = new System.Drawing.Size(202, 430);
            this.tableLayoutPanelRealTimePlayer.TabIndex = 0;
            // 
            // buttonRtStart
            // 
            this.buttonRtStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRtStart.Location = new System.Drawing.Point(3, 3);
            this.buttonRtStart.Name = "buttonRtStart";
            this.buttonRtStart.Size = new System.Drawing.Size(95, 27);
            this.buttonRtStart.TabIndex = 0;
            this.buttonRtStart.Text = "開始";
            this.buttonRtStart.UseVisualStyleBackColor = true;
            this.buttonRtStart.Click += new System.EventHandler(this.buttonStreamStart_Click);
            // 
            // buttonRtStop
            // 
            this.buttonRtStop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRtStop.Location = new System.Drawing.Point(3, 36);
            this.buttonRtStop.Name = "buttonRtStop";
            this.buttonRtStop.Size = new System.Drawing.Size(95, 27);
            this.buttonRtStop.TabIndex = 1;
            this.buttonRtStop.Text = "終了";
            this.buttonRtStop.UseVisualStyleBackColor = true;
            this.buttonRtStop.Click += new System.EventHandler(this.buttonStreamStop_Click);
            // 
            // labelRtStatus
            // 
            this.labelRtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRtStatus.AutoSize = true;
            this.tableLayoutPanelRealTimePlayer.SetColumnSpan(this.labelRtStatus, 2);
            this.labelRtStatus.Location = new System.Drawing.Point(3, 66);
            this.labelRtStatus.Name = "labelRtStatus";
            this.labelRtStatus.Size = new System.Drawing.Size(196, 20);
            this.labelRtStatus.TabIndex = 10;
            // 
            // labelRtElapsedCaption
            // 
            this.labelRtElapsedCaption.AutoSize = true;
            this.labelRtElapsedCaption.Location = new System.Drawing.Point(3, 410);
            this.labelRtElapsedCaption.Name = "labelRtElapsedCaption";
            this.labelRtElapsedCaption.Size = new System.Drawing.Size(73, 20);
            this.labelRtElapsedCaption.TabIndex = 11;
            this.labelRtElapsedCaption.Text = "経過時間";
            // 
            // labelRtElapsed
            // 
            this.labelRtElapsed.AutoSize = true;
            this.labelRtElapsed.Location = new System.Drawing.Point(104, 410);
            this.labelRtElapsed.Name = "labelRtElapsed";
            this.labelRtElapsed.Size = new System.Drawing.Size(71, 20);
            this.labelRtElapsed.TabIndex = 12;
            this.labelRtElapsed.Text = "00:00:00";
            // 
            // progressBarRtStart
            // 
            this.progressBarRtStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBarRtStart.Location = new System.Drawing.Point(104, 3);
            this.progressBarRtStart.Name = "progressBarRtStart";
            this.progressBarRtStart.Size = new System.Drawing.Size(95, 27);
            this.progressBarRtStart.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarRtStart.TabIndex = 13;
            this.progressBarRtStart.Visible = false;
            // 
            // labelRtRetryStatus
            // 
            this.labelRtRetryStatus.AutoSize = true;
            this.tableLayoutPanelRealTimePlayer.SetColumnSpan(this.labelRtRetryStatus, 2);
            this.labelRtRetryStatus.Location = new System.Drawing.Point(3, 86);
            this.labelRtRetryStatus.Name = "labelRtRetryStatus";
            this.labelRtRetryStatus.Size = new System.Drawing.Size(0, 20);
            this.labelRtRetryStatus.TabIndex = 14;
            // 
            // labelStreamingSessionRetry
            // 
            this.labelStreamingSessionRetry.AutoSize = true;
            this.tableLayoutPanelRealTimePlayer.SetColumnSpan(this.labelStreamingSessionRetry, 2);
            this.labelStreamingSessionRetry.Location = new System.Drawing.Point(3, 106);
            this.labelStreamingSessionRetry.Name = "labelStreamingSessionRetry";
            this.labelStreamingSessionRetry.Size = new System.Drawing.Size(0, 20);
            this.labelStreamingSessionRetry.TabIndex = 15;
            // 
            // radioButtonRtCh1
            // 
            this.radioButtonRtCh1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonRtCh1.Enabled = false;
            this.radioButtonRtCh1.Location = new System.Drawing.Point(3, 129);
            this.radioButtonRtCh1.Name = "radioButtonRtCh1";
            this.radioButtonRtCh1.Size = new System.Drawing.Size(94, 27);
            this.radioButtonRtCh1.TabIndex = 16;
            this.radioButtonRtCh1.Tag = "0";
            this.radioButtonRtCh1.Text = "Ch1";
            this.radioButtonRtCh1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonRtCh1.UseVisualStyleBackColor = true;
            this.radioButtonRtCh1.CheckedChanged += new System.EventHandler(this.RadioButtonRtCh1_CheckedChanged);
            // 
            // radioButtonRtCh2
            // 
            this.radioButtonRtCh2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonRtCh2.Enabled = false;
            this.radioButtonRtCh2.Location = new System.Drawing.Point(3, 162);
            this.radioButtonRtCh2.Name = "radioButtonRtCh2";
            this.radioButtonRtCh2.Size = new System.Drawing.Size(94, 27);
            this.radioButtonRtCh2.TabIndex = 17;
            this.radioButtonRtCh2.Tag = "1";
            this.radioButtonRtCh2.Text = "Ch2";
            this.radioButtonRtCh2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonRtCh2.UseVisualStyleBackColor = true;
            this.radioButtonRtCh2.CheckedChanged += new System.EventHandler(this.RadioButtonRtCh1_CheckedChanged);
            // 
            // radioButtonRtCh3
            // 
            this.radioButtonRtCh3.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonRtCh3.Enabled = false;
            this.radioButtonRtCh3.Location = new System.Drawing.Point(3, 195);
            this.radioButtonRtCh3.Name = "radioButtonRtCh3";
            this.radioButtonRtCh3.Size = new System.Drawing.Size(94, 27);
            this.radioButtonRtCh3.TabIndex = 18;
            this.radioButtonRtCh3.Tag = "2";
            this.radioButtonRtCh3.Text = "Ch3";
            this.radioButtonRtCh3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonRtCh3.UseVisualStyleBackColor = true;
            this.radioButtonRtCh3.CheckedChanged += new System.EventHandler(this.RadioButtonRtCh1_CheckedChanged);
            // 
            // radioButtonRtCh4
            // 
            this.radioButtonRtCh4.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonRtCh4.Enabled = false;
            this.radioButtonRtCh4.Location = new System.Drawing.Point(3, 228);
            this.radioButtonRtCh4.Name = "radioButtonRtCh4";
            this.radioButtonRtCh4.Size = new System.Drawing.Size(94, 27);
            this.radioButtonRtCh4.TabIndex = 19;
            this.radioButtonRtCh4.Tag = "3";
            this.radioButtonRtCh4.Text = "Ch4";
            this.radioButtonRtCh4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonRtCh4.UseVisualStyleBackColor = true;
            this.radioButtonRtCh4.CheckedChanged += new System.EventHandler(this.RadioButtonRtCh1_CheckedChanged);
            // 
            // radioButtonRtCh5
            // 
            this.radioButtonRtCh5.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonRtCh5.Enabled = false;
            this.radioButtonRtCh5.Location = new System.Drawing.Point(3, 261);
            this.radioButtonRtCh5.Name = "radioButtonRtCh5";
            this.radioButtonRtCh5.Size = new System.Drawing.Size(94, 27);
            this.radioButtonRtCh5.TabIndex = 20;
            this.radioButtonRtCh5.Tag = "4";
            this.radioButtonRtCh5.Text = "Ch5";
            this.radioButtonRtCh5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonRtCh5.UseVisualStyleBackColor = true;
            this.radioButtonRtCh5.CheckedChanged += new System.EventHandler(this.RadioButtonRtCh1_CheckedChanged);
            // 
            // radioButtonRtCh6
            // 
            this.radioButtonRtCh6.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonRtCh6.Enabled = false;
            this.radioButtonRtCh6.Location = new System.Drawing.Point(3, 294);
            this.radioButtonRtCh6.Name = "radioButtonRtCh6";
            this.radioButtonRtCh6.Size = new System.Drawing.Size(94, 27);
            this.radioButtonRtCh6.TabIndex = 21;
            this.radioButtonRtCh6.Tag = "5";
            this.radioButtonRtCh6.Text = "Ch6";
            this.radioButtonRtCh6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonRtCh6.UseVisualStyleBackColor = true;
            this.radioButtonRtCh6.CheckedChanged += new System.EventHandler(this.RadioButtonRtCh1_CheckedChanged);
            // 
            // radioButtonRtCh7
            // 
            this.radioButtonRtCh7.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonRtCh7.Enabled = false;
            this.radioButtonRtCh7.Location = new System.Drawing.Point(3, 327);
            this.radioButtonRtCh7.Name = "radioButtonRtCh7";
            this.radioButtonRtCh7.Size = new System.Drawing.Size(94, 27);
            this.radioButtonRtCh7.TabIndex = 22;
            this.radioButtonRtCh7.Tag = "6";
            this.radioButtonRtCh7.Text = "Ch7";
            this.radioButtonRtCh7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonRtCh7.UseVisualStyleBackColor = true;
            this.radioButtonRtCh7.CheckedChanged += new System.EventHandler(this.RadioButtonRtCh1_CheckedChanged);
            // 
            // radioButtonRtCh8
            // 
            this.radioButtonRtCh8.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonRtCh8.Enabled = false;
            this.radioButtonRtCh8.Location = new System.Drawing.Point(3, 360);
            this.radioButtonRtCh8.Name = "radioButtonRtCh8";
            this.radioButtonRtCh8.Size = new System.Drawing.Size(94, 27);
            this.radioButtonRtCh8.TabIndex = 23;
            this.radioButtonRtCh8.Tag = "7";
            this.radioButtonRtCh8.Text = "Ch8";
            this.radioButtonRtCh8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButtonRtCh8.UseVisualStyleBackColor = true;
            this.radioButtonRtCh8.CheckedChanged += new System.EventHandler(this.RadioButtonRtCh1_CheckedChanged);
            // 
            // tabPageEvent
            // 
            this.tabPageEvent.BackColor = System.Drawing.Color.CornflowerBlue;
            this.tabPageEvent.Controls.Add(this.buttonShowDrivingMovie);
            this.tabPageEvent.Location = new System.Drawing.Point(4, 4);
            this.tabPageEvent.Name = "tabPageEvent";
            this.tabPageEvent.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEvent.Size = new System.Drawing.Size(208, 728);
            this.tabPageEvent.TabIndex = 1;
            this.tabPageEvent.Text = "ドラレコ映像";
            // 
            // buttonShowDrivingMovie
            // 
            this.buttonShowDrivingMovie.Location = new System.Drawing.Point(7, 7);
            this.buttonShowDrivingMovie.Name = "buttonShowDrivingMovie";
            this.buttonShowDrivingMovie.Size = new System.Drawing.Size(193, 40);
            this.buttonShowDrivingMovie.TabIndex = 0;
            this.buttonShowDrivingMovie.Text = "ドラレコ映像";
            this.buttonShowDrivingMovie.UseVisualStyleBackColor = true;
            this.buttonShowDrivingMovie.Click += new System.EventHandler(this.buttonShowDrivingMovie_Click);
            // 
            // tabPageRemoteConfig
            // 
            this.tabPageRemoteConfig.BackColor = System.Drawing.Color.CornflowerBlue;
            this.tabPageRemoteConfig.Controls.Add(this.buttonShowRemoteSetting);
            this.tabPageRemoteConfig.Location = new System.Drawing.Point(4, 4);
            this.tabPageRemoteConfig.Name = "tabPageRemoteConfig";
            this.tabPageRemoteConfig.Size = new System.Drawing.Size(208, 728);
            this.tabPageRemoteConfig.TabIndex = 2;
            this.tabPageRemoteConfig.Text = "遠隔設定";
            // 
            // buttonShowRemoteSetting
            // 
            this.buttonShowRemoteSetting.Location = new System.Drawing.Point(7, 7);
            this.buttonShowRemoteSetting.Name = "buttonShowRemoteSetting";
            this.buttonShowRemoteSetting.Size = new System.Drawing.Size(193, 40);
            this.buttonShowRemoteSetting.TabIndex = 0;
            this.buttonShowRemoteSetting.Text = "遠隔設定";
            this.buttonShowRemoteSetting.UseVisualStyleBackColor = true;
            this.buttonShowRemoteSetting.Click += new System.EventHandler(this.buttonShowRemoteSetting_Click);
            // 
            // tableLayoutPanelCarInfo
            // 
            this.tableLayoutPanelCarInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelCarInfo.ColumnCount = 2;
            this.tableLayoutPanelCarInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanelCarInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanelCarInfo.Controls.Add(this.buttonLeftPanelClose, 1, 0);
            this.tableLayoutPanelCarInfo.Controls.Add(this.labelCarIdCaption, 0, 1);
            this.tableLayoutPanelCarInfo.Controls.Add(this.labelCarStatusCaption, 0, 2);
            this.tableLayoutPanelCarInfo.Controls.Add(this.labelRtCarId, 1, 1);
            this.tableLayoutPanelCarInfo.Controls.Add(this.labelRtCarStatus, 1, 2);
            this.tableLayoutPanelCarInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelCarInfo.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelCarInfo.Name = "tableLayoutPanelCarInfo";
            this.tableLayoutPanelCarInfo.RowCount = 3;
            this.tableLayoutPanelCarInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelCarInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelCarInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelCarInfo.Size = new System.Drawing.Size(240, 100);
            this.tableLayoutPanelCarInfo.TabIndex = 2;
            // 
            // buttonLeftPanelClose
            // 
            this.buttonLeftPanelClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLeftPanelClose.Location = new System.Drawing.Point(75, 3);
            this.buttonLeftPanelClose.Name = "buttonLeftPanelClose";
            this.buttonLeftPanelClose.Size = new System.Drawing.Size(162, 27);
            this.buttonLeftPanelClose.TabIndex = 0;
            this.buttonLeftPanelClose.Text = "<< 閉じる";
            this.buttonLeftPanelClose.UseVisualStyleBackColor = true;
            this.buttonLeftPanelClose.Click += new System.EventHandler(this.buttonLeftPanelClose_Click);
            // 
            // labelCarIdCaption
            // 
            this.labelCarIdCaption.AutoSize = true;
            this.labelCarIdCaption.Location = new System.Drawing.Point(3, 33);
            this.labelCarIdCaption.Name = "labelCarIdCaption";
            this.labelCarIdCaption.Size = new System.Drawing.Size(41, 20);
            this.labelCarIdCaption.TabIndex = 1;
            this.labelCarIdCaption.Text = "社番";
            // 
            // labelCarStatusCaption
            // 
            this.labelCarStatusCaption.AutoSize = true;
            this.labelCarStatusCaption.Location = new System.Drawing.Point(3, 53);
            this.labelCarStatusCaption.Name = "labelCarStatusCaption";
            this.labelCarStatusCaption.Size = new System.Drawing.Size(41, 20);
            this.labelCarStatusCaption.TabIndex = 2;
            this.labelCarStatusCaption.Text = "状態";
            // 
            // labelRtCarId
            // 
            this.labelRtCarId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRtCarId.AutoSize = true;
            this.labelRtCarId.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.carListBindingSource, "CarId", true));
            this.labelRtCarId.Location = new System.Drawing.Point(75, 33);
            this.labelRtCarId.Name = "labelRtCarId";
            this.labelRtCarId.Size = new System.Drawing.Size(162, 20);
            this.labelRtCarId.TabIndex = 3;
            // 
            // labelRtCarStatus
            // 
            this.labelRtCarStatus.AutoEllipsis = true;
            this.labelRtCarStatus.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.carListBindingSource, "ErrorMessage", true));
            this.labelRtCarStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRtCarStatus.Location = new System.Drawing.Point(75, 53);
            this.labelRtCarStatus.Name = "labelRtCarStatus";
            this.labelRtCarStatus.Size = new System.Drawing.Size(162, 790);
            this.labelRtCarStatus.TabIndex = 4;
            // 
            // zoomBar
            // 
            this.zoomBar.BackColor = System.Drawing.SystemColors.Control;
            this.zoomBar.Cursor = System.Windows.Forms.Cursors.NoMoveVert;
            this.zoomBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.zoomBar.LargeChange = 0;
            this.zoomBar.Location = new System.Drawing.Point(1389, 53);
            this.zoomBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.zoomBar.Maximum = 330;
            this.zoomBar.Name = "zoomBar";
            this.zoomBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.zoomBar.Size = new System.Drawing.Size(45, 836);
            this.zoomBar.TabIndex = 12;
            this.zoomBar.TabStop = false;
            this.zoomBar.TickFrequency = 20;
            this.zoomBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.zoomBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.zoomBar_MouseUp);
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.tableLayoutPanelHeader);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(3, 3);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1431, 50);
            this.panelHeader.TabIndex = 4;
            // 
            // tableLayoutPanelHeader
            // 
            this.tableLayoutPanelHeader.ColumnCount = 6;
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelHeader.Controls.Add(this.panelCarDisplayMode, 0, 0);
            this.tableLayoutPanelHeader.Controls.Add(this.comboBoxOffice, 1, 0);
            this.tableLayoutPanelHeader.Controls.Add(this.labelUserName, 5, 0);
            this.tableLayoutPanelHeader.Controls.Add(this.labelUpdateDate, 3, 0);
            this.tableLayoutPanelHeader.Controls.Add(this.labelUserNameCaption, 4, 0);
            this.tableLayoutPanelHeader.Controls.Add(this.labelUpdateDateCaption, 2, 0);
            this.tableLayoutPanelHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelHeader.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            this.tableLayoutPanelHeader.RowCount = 1;
            this.tableLayoutPanelHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelHeader.Size = new System.Drawing.Size(1431, 50);
            this.tableLayoutPanelHeader.TabIndex = 0;
            // 
            // panelCarDisplayMode
            // 
            this.panelCarDisplayMode.Controls.Add(this.radioButtonSelect);
            this.panelCarDisplayMode.Controls.Add(this.radioButtonALL);
            this.panelCarDisplayMode.Location = new System.Drawing.Point(3, 3);
            this.panelCarDisplayMode.Name = "panelCarDisplayMode";
            this.panelCarDisplayMode.Size = new System.Drawing.Size(298, 40);
            this.panelCarDisplayMode.TabIndex = 16;
            // 
            // radioButtonSelect
            // 
            this.radioButtonSelect.AutoSize = true;
            this.radioButtonSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButtonSelect.Location = new System.Drawing.Point(132, 11);
            this.radioButtonSelect.Name = "radioButtonSelect";
            this.radioButtonSelect.Size = new System.Drawing.Size(142, 28);
            this.radioButtonSelect.TabIndex = 13;
            this.radioButtonSelect.TabStop = true;
            this.radioButtonSelect.Text = "選択車両表示";
            this.radioButtonSelect.UseVisualStyleBackColor = true;
            this.radioButtonSelect.CheckedChanged += new System.EventHandler(this.RadioButtonSelect_CheckedChanged);
            // 
            // radioButtonALL
            // 
            this.radioButtonALL.AutoSize = true;
            this.radioButtonALL.Checked = true;
            this.radioButtonALL.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButtonALL.Location = new System.Drawing.Point(3, 11);
            this.radioButtonALL.Name = "radioButtonALL";
            this.radioButtonALL.Size = new System.Drawing.Size(123, 28);
            this.radioButtonALL.TabIndex = 12;
            this.radioButtonALL.TabStop = true;
            this.radioButtonALL.Text = "全車両表示";
            this.radioButtonALL.UseVisualStyleBackColor = true;
            // 
            // comboBoxOffice
            // 
            this.comboBoxOffice.DisplayMember = "Id";
            this.comboBoxOffice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOffice.Enabled = false;
            this.comboBoxOffice.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.comboBoxOffice.FormattingEnabled = true;
            this.comboBoxOffice.Location = new System.Drawing.Point(307, 5);
            this.comboBoxOffice.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.comboBoxOffice.Name = "comboBoxOffice";
            this.comboBoxOffice.Size = new System.Drawing.Size(230, 39);
            this.comboBoxOffice.TabIndex = 14;
            this.comboBoxOffice.ValueMember = "Id";
            // 
            // labelUserName
            // 
            this.labelUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUserName.AutoSize = true;
            this.labelUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelUserName.Location = new System.Drawing.Point(976, 0);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(452, 50);
            this.labelUserName.TabIndex = 2;
            this.labelUserName.Text = "(指紋認証してください)";
            this.labelUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelUserNameCaption
            // 
            this.labelUserNameCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUserNameCaption.AutoSize = true;
            this.labelUserNameCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelUserNameCaption.Location = new System.Drawing.Point(839, 0);
            this.labelUserNameCaption.Name = "labelUserNameCaption";
            this.labelUserNameCaption.Size = new System.Drawing.Size(131, 50);
            this.labelUserNameCaption.TabIndex = 2;
            this.labelUserNameCaption.Text = "ユーザー:";
            this.labelUserNameCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage2
            // 
            this.tabPage2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage2.Controls.Add(this.tableLayoutPanelEventList);
            this.tabPage2.Controls.Add(this.gridEventList);
            this.tabPage2.Controls.Add(this.labelEventListCaption);
            this.tabPage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabPage2.Location = new System.Drawing.Point(4, 34);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1437, 892);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "イベント再生";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelEventList
            // 
            this.tableLayoutPanelEventList.ColumnCount = 4;
            this.tableLayoutPanelEventList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelEventList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelEventList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelEventList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelEventList.Controls.Add(this.buttonDownload, 0, 0);
            this.tableLayoutPanelEventList.Controls.Add(this.buttonEventListUpdate, 0, 4);
            this.tableLayoutPanelEventList.Controls.Add(this.progressBarEventListUpdate, 0, 5);
            this.tableLayoutPanelEventList.Controls.Add(this.buttonGetG, 0, 7);
            this.tableLayoutPanelEventList.Controls.Add(this.buttonCancelG, 1, 7);
            this.tableLayoutPanelEventList.Controls.Add(this.progressBarG, 0, 8);
            this.tableLayoutPanelEventList.Controls.Add(this.labelGstatus, 1, 8);
            this.tableLayoutPanelEventList.Controls.Add(this.progressBarDownload, 0, 1);
            this.tableLayoutPanelEventList.Controls.Add(this.labelDownloadStatus, 1, 1);
            this.tableLayoutPanelEventList.Controls.Add(this.panelPlay, 0, 2);
            this.tableLayoutPanelEventList.Location = new System.Drawing.Point(741, 45);
            this.tableLayoutPanelEventList.Name = "tableLayoutPanelEventList";
            this.tableLayoutPanelEventList.RowCount = 9;
            this.tableLayoutPanelEventList.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelEventList.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelEventList.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelEventList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelEventList.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelEventList.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelEventList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelEventList.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelEventList.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelEventList.Size = new System.Drawing.Size(429, 403);
            this.tableLayoutPanelEventList.TabIndex = 33;
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(3, 3);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(140, 40);
            this.buttonDownload.TabIndex = 2;
            this.buttonDownload.Text = "ダウンロード開始";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.ButtonDownload_Click);
            // 
            // buttonEventListUpdate
            // 
            this.buttonEventListUpdate.Location = new System.Drawing.Point(3, 228);
            this.buttonEventListUpdate.Name = "buttonEventListUpdate";
            this.buttonEventListUpdate.Size = new System.Drawing.Size(140, 40);
            this.buttonEventListUpdate.TabIndex = 3;
            this.buttonEventListUpdate.Text = "リストの更新";
            this.buttonEventListUpdate.UseVisualStyleBackColor = true;
            this.buttonEventListUpdate.Click += new System.EventHandler(this.buttonEventListUpdate_Click);
            // 
            // progressBarEventListUpdate
            // 
            this.progressBarEventListUpdate.Location = new System.Drawing.Point(3, 274);
            this.progressBarEventListUpdate.Name = "progressBarEventListUpdate";
            this.progressBarEventListUpdate.Size = new System.Drawing.Size(140, 23);
            this.progressBarEventListUpdate.TabIndex = 28;
            this.progressBarEventListUpdate.Visible = false;
            // 
            // buttonGetG
            // 
            this.buttonGetG.Location = new System.Drawing.Point(3, 323);
            this.buttonGetG.Name = "buttonGetG";
            this.buttonGetG.Size = new System.Drawing.Size(140, 40);
            this.buttonGetG.TabIndex = 4;
            this.buttonGetG.Text = "G取得";
            this.buttonGetG.UseVisualStyleBackColor = true;
            this.buttonGetG.Click += new System.EventHandler(this.buttonGetG_Click);
            // 
            // buttonCancelG
            // 
            this.buttonCancelG.Enabled = false;
            this.buttonCancelG.Location = new System.Drawing.Point(149, 323);
            this.buttonCancelG.Name = "buttonCancelG";
            this.buttonCancelG.Size = new System.Drawing.Size(140, 40);
            this.buttonCancelG.TabIndex = 5;
            this.buttonCancelG.Text = "G取得キャンセル";
            this.buttonCancelG.UseVisualStyleBackColor = true;
            this.buttonCancelG.Click += new System.EventHandler(this.buttonCancelG_Click);
            // 
            // progressBarG
            // 
            this.progressBarG.Location = new System.Drawing.Point(3, 369);
            this.progressBarG.Name = "progressBarG";
            this.progressBarG.Size = new System.Drawing.Size(140, 23);
            this.progressBarG.TabIndex = 30;
            this.progressBarG.Visible = false;
            // 
            // labelGstatus
            // 
            this.labelGstatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGstatus.AutoSize = true;
            this.labelGstatus.Location = new System.Drawing.Point(149, 369);
            this.labelGstatus.Margin = new System.Windows.Forms.Padding(3);
            this.labelGstatus.Name = "labelGstatus";
            this.labelGstatus.Size = new System.Drawing.Size(140, 20);
            this.labelGstatus.TabIndex = 32;
            this.labelGstatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBarDownload
            // 
            this.progressBarDownload.Location = new System.Drawing.Point(3, 49);
            this.progressBarDownload.MarqueeAnimationSpeed = 50;
            this.progressBarDownload.Name = "progressBarDownload";
            this.progressBarDownload.Size = new System.Drawing.Size(140, 23);
            this.progressBarDownload.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarDownload.TabIndex = 4;
            this.progressBarDownload.Visible = false;
            // 
            // labelDownloadStatus
            // 
            this.labelDownloadStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDownloadStatus.AutoSize = true;
            this.labelDownloadStatus.Location = new System.Drawing.Point(149, 49);
            this.labelDownloadStatus.Margin = new System.Windows.Forms.Padding(3);
            this.labelDownloadStatus.Name = "labelDownloadStatus";
            this.labelDownloadStatus.Size = new System.Drawing.Size(140, 23);
            this.labelDownloadStatus.TabIndex = 5;
            this.labelDownloadStatus.Text = "ダウンロード中";
            this.labelDownloadStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelDownloadStatus.Visible = false;
            // 
            // panelPlay
            // 
            this.panelPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPlay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanelEventList.SetColumnSpan(this.panelPlay, 4);
            this.panelPlay.Controls.Add(this.tableLayoutPanelPlay);
            this.panelPlay.Location = new System.Drawing.Point(3, 78);
            this.panelPlay.Name = "panelPlay";
            this.panelPlay.Size = new System.Drawing.Size(426, 124);
            this.panelPlay.TabIndex = 6;
            this.panelPlay.Visible = false;
            // 
            // tableLayoutPanelPlay
            // 
            this.tableLayoutPanelPlay.ColumnCount = 7;
            this.tableLayoutPanelPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelPlay.Controls.Add(this.labelEventProc, 0, 3);
            this.tableLayoutPanelPlay.Controls.Add(this.labelPanelPlayCarIdCaption, 0, 0);
            this.tableLayoutPanelPlay.Controls.Add(this.buttonPlay7, 5, 2);
            this.tableLayoutPanelPlay.Controls.Add(this.buttonPlay6, 2, 2);
            this.tableLayoutPanelPlay.Controls.Add(this.buttonPlay5, 0, 2);
            this.tableLayoutPanelPlay.Controls.Add(this.buttonPlay1, 0, 1);
            this.tableLayoutPanelPlay.Controls.Add(this.buttonPlay2, 2, 1);
            this.tableLayoutPanelPlay.Controls.Add(this.buttonPlay3, 5, 1);
            this.tableLayoutPanelPlay.Controls.Add(this.labelPanelPlayCarId, 1, 0);
            this.tableLayoutPanelPlay.Controls.Add(this.buttonPlay4, 6, 1);
            this.tableLayoutPanelPlay.Controls.Add(this.buttonPlay8, 6, 2);
            this.tableLayoutPanelPlay.Controls.Add(this.labelPanelPlayDateCaption, 3, 0);
            this.tableLayoutPanelPlay.Controls.Add(this.labelPanelPlayDate, 4, 0);
            this.tableLayoutPanelPlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelPlay.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelPlay.Name = "tableLayoutPanelPlay";
            this.tableLayoutPanelPlay.RowCount = 4;
            this.tableLayoutPanelPlay.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelPlay.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelPlay.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelPlay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanelPlay.Size = new System.Drawing.Size(424, 122);
            this.tableLayoutPanelPlay.TabIndex = 33;
            // 
            // labelEventProc
            // 
            this.labelEventProc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelEventProc.AutoSize = true;
            this.tableLayoutPanelPlay.SetColumnSpan(this.labelEventProc, 7);
            this.labelEventProc.Location = new System.Drawing.Point(3, 95);
            this.labelEventProc.Margin = new System.Windows.Forms.Padding(3);
            this.labelEventProc.Name = "labelEventProc";
            this.labelEventProc.Size = new System.Drawing.Size(418, 29);
            this.labelEventProc.TabIndex = 29;
            this.labelEventProc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelPanelPlayCarIdCaption
            // 
            this.labelPanelPlayCarIdCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPanelPlayCarIdCaption.AutoSize = true;
            this.labelPanelPlayCarIdCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelPanelPlayCarIdCaption.Location = new System.Drawing.Point(3, 3);
            this.labelPanelPlayCarIdCaption.Margin = new System.Windows.Forms.Padding(3);
            this.labelPanelPlayCarIdCaption.Name = "labelPanelPlayCarIdCaption";
            this.labelPanelPlayCarIdCaption.Size = new System.Drawing.Size(49, 20);
            this.labelPanelPlayCarIdCaption.TabIndex = 0;
            this.labelPanelPlayCarIdCaption.Text = "社番：";
            // 
            // buttonPlay7
            // 
            this.buttonPlay7.Enabled = false;
            this.buttonPlay7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay7.Location = new System.Drawing.Point(215, 62);
            this.buttonPlay7.Name = "buttonPlay7";
            this.buttonPlay7.Size = new System.Drawing.Size(100, 27);
            this.buttonPlay7.TabIndex = 10;
            this.buttonPlay7.Text = "Ch7 再生";
            this.buttonPlay7.UseVisualStyleBackColor = true;
            this.buttonPlay7.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay6
            // 
            this.tableLayoutPanelPlay.SetColumnSpan(this.buttonPlay6, 3);
            this.buttonPlay6.Enabled = false;
            this.buttonPlay6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay6.Location = new System.Drawing.Point(109, 62);
            this.buttonPlay6.Name = "buttonPlay6";
            this.buttonPlay6.Size = new System.Drawing.Size(100, 27);
            this.buttonPlay6.TabIndex = 9;
            this.buttonPlay6.Text = "Ch6 再生";
            this.buttonPlay6.UseVisualStyleBackColor = true;
            this.buttonPlay6.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay5
            // 
            this.tableLayoutPanelPlay.SetColumnSpan(this.buttonPlay5, 2);
            this.buttonPlay5.Enabled = false;
            this.buttonPlay5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay5.Location = new System.Drawing.Point(3, 62);
            this.buttonPlay5.Name = "buttonPlay5";
            this.buttonPlay5.Size = new System.Drawing.Size(100, 27);
            this.buttonPlay5.TabIndex = 8;
            this.buttonPlay5.Text = "Ch5 再生";
            this.buttonPlay5.UseVisualStyleBackColor = true;
            this.buttonPlay5.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay1
            // 
            this.tableLayoutPanelPlay.SetColumnSpan(this.buttonPlay1, 2);
            this.buttonPlay1.Enabled = false;
            this.buttonPlay1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay1.Location = new System.Drawing.Point(3, 29);
            this.buttonPlay1.Name = "buttonPlay1";
            this.buttonPlay1.Size = new System.Drawing.Size(100, 27);
            this.buttonPlay1.TabIndex = 4;
            this.buttonPlay1.Text = "Ch1 再生";
            this.buttonPlay1.UseVisualStyleBackColor = true;
            this.buttonPlay1.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay2
            // 
            this.tableLayoutPanelPlay.SetColumnSpan(this.buttonPlay2, 3);
            this.buttonPlay2.Enabled = false;
            this.buttonPlay2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay2.Location = new System.Drawing.Point(109, 29);
            this.buttonPlay2.Name = "buttonPlay2";
            this.buttonPlay2.Size = new System.Drawing.Size(100, 27);
            this.buttonPlay2.TabIndex = 5;
            this.buttonPlay2.Text = "Ch2 再生";
            this.buttonPlay2.UseVisualStyleBackColor = true;
            this.buttonPlay2.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay3
            // 
            this.buttonPlay3.Enabled = false;
            this.buttonPlay3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay3.Location = new System.Drawing.Point(215, 29);
            this.buttonPlay3.Name = "buttonPlay3";
            this.buttonPlay3.Size = new System.Drawing.Size(100, 27);
            this.buttonPlay3.TabIndex = 6;
            this.buttonPlay3.Text = "Ch3 再生";
            this.buttonPlay3.UseVisualStyleBackColor = true;
            this.buttonPlay3.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // labelPanelPlayCarId
            // 
            this.labelPanelPlayCarId.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPanelPlayCarId.AutoSize = true;
            this.tableLayoutPanelPlay.SetColumnSpan(this.labelPanelPlayCarId, 2);
            this.labelPanelPlayCarId.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelPanelPlayCarId.Location = new System.Drawing.Point(58, 3);
            this.labelPanelPlayCarId.Margin = new System.Windows.Forms.Padding(3);
            this.labelPanelPlayCarId.Name = "labelPanelPlayCarId";
            this.labelPanelPlayCarId.Size = new System.Drawing.Size(71, 20);
            this.labelPanelPlayCarId.TabIndex = 1;
            this.labelPanelPlayCarId.Text = "0000";
            // 
            // buttonPlay4
            // 
            this.buttonPlay4.Enabled = false;
            this.buttonPlay4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay4.Location = new System.Drawing.Point(321, 29);
            this.buttonPlay4.Name = "buttonPlay4";
            this.buttonPlay4.Size = new System.Drawing.Size(100, 27);
            this.buttonPlay4.TabIndex = 7;
            this.buttonPlay4.Text = "Ch4 再生";
            this.buttonPlay4.UseVisualStyleBackColor = true;
            this.buttonPlay4.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay8
            // 
            this.buttonPlay8.Enabled = false;
            this.buttonPlay8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay8.Location = new System.Drawing.Point(321, 62);
            this.buttonPlay8.Name = "buttonPlay8";
            this.buttonPlay8.Size = new System.Drawing.Size(100, 27);
            this.buttonPlay8.TabIndex = 11;
            this.buttonPlay8.Text = "Ch8 再生";
            this.buttonPlay8.UseVisualStyleBackColor = true;
            this.buttonPlay8.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // labelPanelPlayDateCaption
            // 
            this.labelPanelPlayDateCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPanelPlayDateCaption.AutoSize = true;
            this.labelPanelPlayDateCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelPanelPlayDateCaption.Location = new System.Drawing.Point(135, 3);
            this.labelPanelPlayDateCaption.Margin = new System.Windows.Forms.Padding(3);
            this.labelPanelPlayDateCaption.Name = "labelPanelPlayDateCaption";
            this.labelPanelPlayDateCaption.Size = new System.Drawing.Size(49, 20);
            this.labelPanelPlayDateCaption.TabIndex = 2;
            this.labelPanelPlayDateCaption.Text = "日時：";
            // 
            // labelPanelPlayDate
            // 
            this.labelPanelPlayDate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPanelPlayDate.AutoSize = true;
            this.tableLayoutPanelPlay.SetColumnSpan(this.labelPanelPlayDate, 3);
            this.labelPanelPlayDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelPanelPlayDate.Location = new System.Drawing.Point(190, 3);
            this.labelPanelPlayDate.Margin = new System.Windows.Forms.Padding(3);
            this.labelPanelPlayDate.Name = "labelPanelPlayDate";
            this.labelPanelPlayDate.Size = new System.Drawing.Size(231, 20);
            this.labelPanelPlayDate.TabIndex = 3;
            this.labelPanelPlayDate.Text = "2021/01/01";
            // 
            // gridEventList
            // 
            this.gridEventList.AllowUserToAddRows = false;
            this.gridEventList.AllowUserToDeleteRows = false;
            this.gridEventList.AllowUserToResizeRows = false;
            this.gridEventList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridEventList.AutoGenerateColumns = false;
            this.gridEventList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridEventList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCheck,
            this.timestampDataGridViewTextBoxColumn,
            this.carIdDataGridViewTextBoxColumn,
            this.movieTypeDataGridViewTextBoxColumn,
            this.Remarks,
            this.MovieId});
            this.gridEventList.DataSource = this.eventInfoBindingSource;
            this.gridEventList.Location = new System.Drawing.Point(25, 45);
            this.gridEventList.Margin = new System.Windows.Forms.Padding(2);
            this.gridEventList.Name = "gridEventList";
            this.gridEventList.RowHeadersVisible = false;
            this.gridEventList.RowHeadersWidth = 4;
            this.gridEventList.Size = new System.Drawing.Size(700, 825);
            this.gridEventList.TabIndex = 1;
            this.gridEventList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridEventList_CellContentClick);
            this.gridEventList.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.GridEventList_CellFormatting);
            this.gridEventList.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.GridEventList_ColumnHeaderMouseClick);
            // 
            // ColumnCheck
            // 
            this.ColumnCheck.DataPropertyName = "Selected";
            this.ColumnCheck.FalseValue = "0";
            this.ColumnCheck.HeaderText = "";
            this.ColumnCheck.MinimumWidth = 25;
            this.ColumnCheck.Name = "ColumnCheck";
            this.ColumnCheck.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnCheck.TrueValue = "1";
            this.ColumnCheck.Width = 25;
            // 
            // timestampDataGridViewTextBoxColumn
            // 
            this.timestampDataGridViewTextBoxColumn.DataPropertyName = "Timestamp";
            this.timestampDataGridViewTextBoxColumn.HeaderText = "日付";
            this.timestampDataGridViewTextBoxColumn.MinimumWidth = 200;
            this.timestampDataGridViewTextBoxColumn.Name = "timestampDataGridViewTextBoxColumn";
            this.timestampDataGridViewTextBoxColumn.ReadOnly = true;
            this.timestampDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.timestampDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.timestampDataGridViewTextBoxColumn.Width = 200;
            // 
            // carIdDataGridViewTextBoxColumn
            // 
            this.carIdDataGridViewTextBoxColumn.DataPropertyName = "CarId";
            this.carIdDataGridViewTextBoxColumn.HeaderText = "社番";
            this.carIdDataGridViewTextBoxColumn.MinimumWidth = 65;
            this.carIdDataGridViewTextBoxColumn.Name = "carIdDataGridViewTextBoxColumn";
            this.carIdDataGridViewTextBoxColumn.ReadOnly = true;
            this.carIdDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.carIdDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.carIdDataGridViewTextBoxColumn.Width = 65;
            // 
            // movieTypeDataGridViewTextBoxColumn
            // 
            this.movieTypeDataGridViewTextBoxColumn.DataPropertyName = "MovieType";
            this.movieTypeDataGridViewTextBoxColumn.HeaderText = "種別";
            this.movieTypeDataGridViewTextBoxColumn.MinimumWidth = 79;
            this.movieTypeDataGridViewTextBoxColumn.Name = "movieTypeDataGridViewTextBoxColumn";
            this.movieTypeDataGridViewTextBoxColumn.ReadOnly = true;
            this.movieTypeDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.movieTypeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.movieTypeDataGridViewTextBoxColumn.Width = 79;
            // 
            // Remarks
            // 
            this.Remarks.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Remarks.DataPropertyName = "Remarks";
            this.Remarks.HeaderText = "備考";
            this.Remarks.MinimumWidth = 10;
            this.Remarks.Name = "Remarks";
            this.Remarks.ReadOnly = true;
            this.Remarks.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Remarks.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // MovieId
            // 
            this.MovieId.DataPropertyName = "MovieId";
            this.MovieId.HeaderText = "MovieId";
            this.MovieId.MinimumWidth = 10;
            this.MovieId.Name = "MovieId";
            this.MovieId.Visible = false;
            this.MovieId.Width = 200;
            // 
            // eventInfoBindingSource
            // 
            this.eventInfoBindingSource.DataSource = typeof(RealtimeViewer.Model.EventInfo);
            // 
            // labelEventListCaption
            // 
            this.labelEventListCaption.AutoSize = true;
            this.labelEventListCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelEventListCaption.Location = new System.Drawing.Point(20, 20);
            this.labelEventListCaption.Name = "labelEventListCaption";
            this.labelEventListCaption.Size = new System.Drawing.Size(89, 20);
            this.labelEventListCaption.TabIndex = 0;
            this.labelEventListCaption.Text = "イベントリスト";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "MovieType";
            this.dataGridViewTextBoxColumn1.HeaderText = "種別";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 10;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 200;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "MovieType";
            this.dataGridViewTextBoxColumn2.HeaderText = "種別";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 10;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 200;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "MovieType";
            this.dataGridViewTextBoxColumn3.HeaderText = "種別";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 10;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 200;
            // 
            // buttonUpdateDeviceListUseDebugOnly
            // 
            this.buttonUpdateDeviceListUseDebugOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpdateDeviceListUseDebugOnly.Enabled = false;
            this.buttonUpdateDeviceListUseDebugOnly.Location = new System.Drawing.Point(1197, 11);
            this.buttonUpdateDeviceListUseDebugOnly.Name = "buttonUpdateDeviceListUseDebugOnly";
            this.buttonUpdateDeviceListUseDebugOnly.Size = new System.Drawing.Size(75, 28);
            this.buttonUpdateDeviceListUseDebugOnly.TabIndex = 13;
            this.buttonUpdateDeviceListUseDebugOnly.Text = "button1";
            this.buttonUpdateDeviceListUseDebugOnly.UseVisualStyleBackColor = true;
            this.buttonUpdateDeviceListUseDebugOnly.Visible = false;
            this.buttonUpdateDeviceListUseDebugOnly.Click += new System.EventHandler(this.Button1_Click);
            // 
            // timerStartMQTT
            // 
            this.timerStartMQTT.Interval = 25000;
            this.timerStartMQTT.Tick += new System.EventHandler(this.TimerStartMQTT_Tick);
            // 
            // sqliteCommand1
            // 
            this.sqliteCommand1.CommandTimeout = 30;
            this.sqliteCommand1.Connection = null;
            this.sqliteCommand1.Transaction = null;
            this.sqliteCommand1.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // comboBoxServerEnv
            // 
            this.comboBoxServerEnv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxServerEnv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxServerEnv.FormattingEnabled = true;
            this.comboBoxServerEnv.Items.AddRange(new object[] {
            "Production",
            "Staging"});
            this.comboBoxServerEnv.Location = new System.Drawing.Point(1505, 11);
            this.comboBoxServerEnv.Name = "comboBoxServerEnv";
            this.comboBoxServerEnv.Size = new System.Drawing.Size(174, 28);
            this.comboBoxServerEnv.TabIndex = 14;
            this.comboBoxServerEnv.TabStop = false;
            this.comboBoxServerEnv.Visible = false;
            this.comboBoxServerEnv.SelectedIndexChanged += new System.EventHandler(this.comboBoxServerEnv_SelectedIndexChanged);
            // 
            // buttonEventDubug
            // 
            this.buttonEventDubug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEventDubug.Location = new System.Drawing.Point(1278, 14);
            this.buttonEventDubug.Name = "buttonEventDubug";
            this.buttonEventDubug.Size = new System.Drawing.Size(136, 23);
            this.buttonEventDubug.TabIndex = 15;
            this.buttonEventDubug.Text = "イベントデバッグ";
            this.buttonEventDubug.UseVisualStyleBackColor = true;
            this.buttonEventDubug.Visible = false;
            this.buttonEventDubug.Click += new System.EventHandler(this.button2_Click);
            // 
            // timerStreamingPreparation
            // 
            this.timerStreamingPreparation.Interval = 1000;
            this.timerStreamingPreparation.Tick += new System.EventHandler(this.TimerStreamingPreparation_Tick);
            // 
            // comboBoxDebugUser
            // 
            this.comboBoxDebugUser.Enabled = false;
            this.comboBoxDebugUser.FormattingEnabled = true;
            this.comboBoxDebugUser.Items.AddRange(new object[] {
            "一般太郎",
            "管理次郎",
            "技術三郎"});
            this.comboBoxDebugUser.Location = new System.Drawing.Point(197, 18);
            this.comboBoxDebugUser.Name = "comboBoxDebugUser";
            this.comboBoxDebugUser.Size = new System.Drawing.Size(121, 28);
            this.comboBoxDebugUser.TabIndex = 16;
            this.comboBoxDebugUser.Visible = false;
            this.comboBoxDebugUser.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // pictureBoxError
            // 
            this.pictureBoxError.Image = global::RealtimeViewer.Properties.Resources.ic_error_red_24dp;
            this.pictureBoxError.Location = new System.Drawing.Point(45, 3);
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxError.TabIndex = 17;
            this.pictureBoxError.TabStop = false;
            // 
            // pictureBoxWarn
            // 
            this.pictureBoxWarn.Image = global::RealtimeViewer.Properties.Resources.ic_warning_amber_24dp;
            this.pictureBoxWarn.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxWarn.Name = "pictureBoxWarn";
            this.pictureBoxWarn.Size = new System.Drawing.Size(32, 32);
            this.pictureBoxWarn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxWarn.TabIndex = 18;
            this.pictureBoxWarn.TabStop = false;
            // 
            // panelErrorIcon
            // 
            this.panelErrorIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelErrorIcon.Controls.Add(this.pictureBoxWarn);
            this.panelErrorIcon.Controls.Add(this.pictureBoxError);
            this.panelErrorIcon.Location = new System.Drawing.Point(1685, 7);
            this.panelErrorIcon.Name = "panelErrorIcon";
            this.panelErrorIcon.Size = new System.Drawing.Size(80, 38);
            this.panelErrorIcon.TabIndex = 21;
            this.panelErrorIcon.Visible = false;
            // 
            // labelServerEnv
            // 
            this.labelServerEnv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelServerEnv.AutoSize = true;
            this.labelServerEnv.Location = new System.Drawing.Point(1446, 15);
            this.labelServerEnv.Name = "labelServerEnv";
            this.labelServerEnv.Size = new System.Drawing.Size(57, 20);
            this.labelServerEnv.TabIndex = 22;
            this.labelServerEnv.Text = "接続先";
            this.labelServerEnv.Visible = false;
            // 
            // bindingSourceErrorInfomation
            // 
            this.bindingSourceErrorInfomation.DataSource = typeof(RealtimeViewer.Logger.ErrorInformationManager);
            // 
            // officeInfoBindingSource
            // 
            this.officeInfoBindingSource.DataSource = typeof(RealtimeViewer.Model.OfficeInfo);
            this.officeInfoBindingSource.Filter = "";
            // 
            // deviceIdDataGridViewTextBoxColumn
            // 
            this.deviceIdDataGridViewTextBoxColumn.DataPropertyName = "CarId";
            this.deviceIdDataGridViewTextBoxColumn.HeaderText = "社番";
            this.deviceIdDataGridViewTextBoxColumn.MinimumWidth = 70;
            this.deviceIdDataGridViewTextBoxColumn.Name = "deviceIdDataGridViewTextBoxColumn";
            this.deviceIdDataGridViewTextBoxColumn.ReadOnly = true;
            this.deviceIdDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.deviceIdDataGridViewTextBoxColumn.Width = 70;
            // 
            // addressDataGridViewTextBoxColumn
            // 
            this.addressDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.addressDataGridViewTextBoxColumn.DataPropertyName = "Address";
            this.addressDataGridViewTextBoxColumn.HeaderText = "現在地";
            this.addressDataGridViewTextBoxColumn.Name = "addressDataGridViewTextBoxColumn";
            this.addressDataGridViewTextBoxColumn.ReadOnly = true;
            this.addressDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1784, 961);
            this.Controls.Add(this.labelServerEnv);
            this.Controls.Add(this.panelErrorIcon);
            this.Controls.Add(this.comboBoxDebugUser);
            this.Controls.Add(this.buttonEventDubug);
            this.Controls.Add(this.comboBoxServerEnv);
            this.Controls.Add(this.buttonUpdateDeviceListUseDebugOnly);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.gridCarList);
            this.Controls.Add(this.labelCarListCaption);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "リアルタイム配信システム Ver.X.Y.Z";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.gridCarList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.carListBindingSource)).EndInit();
            this.tabControlMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panelBase.ResumeLayout(false);
            this.panelBase.PerformLayout();
            this.panelMap.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.tabControlRtSelect.ResumeLayout(false);
            this.tabPageRT.ResumeLayout(false);
            this.tabPageRT.PerformLayout();
            this.tableLayoutPanelStreamingOnCar.ResumeLayout(false);
            this.tableLayoutPanelStreamingOnCar.PerformLayout();
            this.tableLayoutPanelRealTimePlayer.ResumeLayout(false);
            this.tableLayoutPanelRealTimePlayer.PerformLayout();
            this.tabPageEvent.ResumeLayout(false);
            this.tabPageRemoteConfig.ResumeLayout(false);
            this.tableLayoutPanelCarInfo.ResumeLayout(false);
            this.tableLayoutPanelCarInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomBar)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.tableLayoutPanelHeader.ResumeLayout(false);
            this.tableLayoutPanelHeader.PerformLayout();
            this.panelCarDisplayMode.ResumeLayout(false);
            this.panelCarDisplayMode.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tableLayoutPanelEventList.ResumeLayout(false);
            this.tableLayoutPanelEventList.PerformLayout();
            this.panelPlay.ResumeLayout(false);
            this.tableLayoutPanelPlay.ResumeLayout(false);
            this.tableLayoutPanelPlay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridEventList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventInfoBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWarn)).EndInit();
            this.panelErrorIcon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceErrorInfomation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.officeInfoBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ToolStripMenuItem menuOpen;
		private System.Windows.Forms.ToolStripMenuItem menuSave;
		private System.Windows.Forms.ToolStripMenuItem menuSaveAs;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem menuExit;
		private System.Windows.Forms.ToolStripMenuItem menuNew;
		private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.Timer timerGPSDraw;
        private System.Windows.Forms.Label labelUpdateDateCaption;
        private System.Windows.Forms.Label labelUpdateDate;
        private System.Windows.Forms.Label labelCarListCaption;
        internal System.Windows.Forms.DataGridView gridCarList;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TrackBar zoomBar;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panelPlay;
        private System.Windows.Forms.Label labelEventListCaption;
        private System.Windows.Forms.Label labelPanelPlayCarId;
        private System.Windows.Forms.Label labelPanelPlayDateCaption;
        private System.Windows.Forms.Label labelPanelPlayCarIdCaption;
        private System.Windows.Forms.Button buttonPlay8;
        private System.Windows.Forms.Button buttonPlay7;
        private System.Windows.Forms.Button buttonPlay6;
        private System.Windows.Forms.Button buttonPlay5;
        private System.Windows.Forms.Button buttonPlay4;
        private System.Windows.Forms.Button buttonPlay3;
        private System.Windows.Forms.Button buttonPlay2;
        private System.Windows.Forms.Button buttonPlay1;
        private System.Windows.Forms.BindingSource eventInfoBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridView gridEventList;
        private System.Windows.Forms.ProgressBar progressBarDownload;
        private System.Windows.Forms.Label labelDownloadStatus;
        private System.Windows.Forms.Label labelPanelPlayDate;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.Button buttonUpdateDeviceListUseDebugOnly;
        private System.Windows.Forms.ComboBox comboBoxOffice;
        private System.Windows.Forms.Timer timerStartMQTT;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.Label labelUserNameCaption;
        private System.Windows.Forms.Panel panelBase;
        private Microsoft.Data.Sqlite.SqliteCommand sqliteCommand1;
        private System.Windows.Forms.Panel panelMap;
        private MpgMap.MpgMap mpgMap;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.TabControl tabControlRtSelect;
        private System.Windows.Forms.TabPage tabPageRT;
        private System.Windows.Forms.TabPage tabPageEvent;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCarInfo;
        private System.Windows.Forms.Button buttonLeftPanelClose;
        private System.Windows.Forms.Label labelCarIdCaption;
        private System.Windows.Forms.Label labelCarStatusCaption;
        private System.Windows.Forms.Label labelRtCarId;
        private System.Windows.Forms.Label labelRtCarStatus;
        private System.Windows.Forms.TabPage tabPageRemoteConfig;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRealTimePlayer;
        private System.Windows.Forms.Button buttonRtStart;
        private System.Windows.Forms.Button buttonRtStop;
        private System.Windows.Forms.Label labelRtStatus;
        private System.Windows.Forms.Label labelRtElapsedCaption;
        private System.Windows.Forms.Label labelRtElapsed;
        private System.Windows.Forms.ProgressBar progressBarRtStart;
        private System.Windows.Forms.Button buttonShowDrivingMovie;
        private System.Windows.Forms.Button buttonShowRemoteSetting;
        private System.Windows.Forms.ComboBox comboBoxServerEnv;
        private System.Windows.Forms.Button buttonEventListUpdate;
        private System.Windows.Forms.ProgressBar progressBarEventListUpdate;
        private System.Windows.Forms.Label labelEventProc;
        private System.Windows.Forms.Button buttonEventDubug;
        private System.Windows.Forms.ProgressBar progressBarG;
        private System.Windows.Forms.Button buttonGetG;
        private System.Windows.Forms.Button buttonCancelG;
        private System.Windows.Forms.Label labelGstatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelStreamingOnCar;
        private System.Windows.Forms.Label labelStreamingOnCarCaption;
        private System.Windows.Forms.Label labelCarStreamElapsedCaption;
        private System.Windows.Forms.Label labelCarStreamFramesCaption;
        private System.Windows.Forms.Label labelCarStreamByteCaption;
        private System.Windows.Forms.Label labelCarStreamFpsCaption;
        private System.Windows.Forms.Label labelCarStreamKbpsCaption;
        private System.Windows.Forms.Label labelCarStreamDropsCaption;
        private System.Windows.Forms.Label labelCarStreamSpeedCaption;
        private System.Windows.Forms.Label labelCarStreamElapsed;
        private System.Windows.Forms.Label labelCarStreamFrames;
        private System.Windows.Forms.Label labelCarStreamBytes;
        private System.Windows.Forms.Label labelCarStreamFps;
        private System.Windows.Forms.Label labelCarStreamKbps;
        private System.Windows.Forms.Label labelCarStreamDrops;
        private System.Windows.Forms.Label labelCarStreamSpeed;
        private System.Windows.Forms.Timer timerStreamingPreparation;
        private System.Windows.Forms.Label labelRtRetryStatus;
        private System.Windows.Forms.BindingSource carListBindingSource;
        private System.Windows.Forms.Panel panelCarDisplayMode;
        private System.Windows.Forms.RadioButton radioButtonSelect;
        private System.Windows.Forms.RadioButton radioButtonALL;
        private System.Windows.Forms.BindingSource officeInfoBindingSource;
        private System.Windows.Forms.ComboBox comboBoxDebugUser;
        private System.Windows.Forms.PictureBox pictureBoxError;
        private System.Windows.Forms.PictureBox pictureBoxWarn;
        private System.Windows.Forms.BindingSource bindingSourceErrorInfomation;
        private System.Windows.Forms.Panel panelErrorIcon;
        private System.Windows.Forms.Label labelStreamingSessionRetry;
        private System.Windows.Forms.RadioButton radioButtonRtCh1;
        private System.Windows.Forms.RadioButton radioButtonRtCh2;
        private System.Windows.Forms.RadioButton radioButtonRtCh3;
        private System.Windows.Forms.RadioButton radioButtonRtCh4;
        private System.Windows.Forms.RadioButton radioButtonRtCh5;
        private System.Windows.Forms.RadioButton radioButtonRtCh6;
        private System.Windows.Forms.RadioButton radioButtonRtCh7;
        private System.Windows.Forms.RadioButton radioButtonRtCh8;
        private System.Windows.Forms.Label labelServerEnv;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn timestampDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn carIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn movieTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remarks;
        private System.Windows.Forms.DataGridViewTextBoxColumn MovieId;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelEventList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPlay;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn addressDataGridViewTextBoxColumn;
    }
}

