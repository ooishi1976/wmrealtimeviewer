
namespace RealtimeViewer.Controls
{
    partial class ConfigPanel
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelConfigMain = new System.Windows.Forms.Panel();
            this.groupBoxServers = new System.Windows.Forms.GroupBox();
            this.comboBoxServers = new System.Windows.Forms.ComboBox();
            this.serverInfoDataSourceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.configPanelModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBoxPrePostDuration = new System.Windows.Forms.GroupBox();
            this.labelPrepostDuration = new System.Windows.Forms.Label();
            this.numericUpDownPrepostDuration = new System.Windows.Forms.NumericUpDown();
            this.groupBoxLogPath = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelLogFileDirectory = new System.Windows.Forms.TableLayoutPanel();
            this.buttonDirectoryClear = new System.Windows.Forms.Button();
            this.labelLogFileDirectory = new System.Windows.Forms.Label();
            this.buttonOpenLogDirectory = new System.Windows.Forms.Button();
            this.groupBoxNotification = new System.Windows.Forms.GroupBox();
            this.checkBoxPopUpEmergency = new System.Windows.Forms.CheckBox();
            this.groupBoxOfficeSetting = new System.Windows.Forms.GroupBox();
            this.dataGridViewOffice = new System.Windows.Forms.DataGridView();
            this.visibleDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.locationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.companyIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.officeInfoDataSourceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBoxRealtimeSetting = new System.Windows.Forms.GroupBox();
            this.numericUpDownSessionWait = new System.Windows.Forms.NumericUpDown();
            this.labelWaitSession = new System.Windows.Forms.Label();
            this.numericUpDownRequestWait = new System.Windows.Forms.NumericUpDown();
            this.labelWaitRequest = new System.Windows.Forms.Label();
            this.numericUpDownSessionRetryCount = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRequestRetryCount = new System.Windows.Forms.NumericUpDown();
            this.labelRetrySession = new System.Windows.Forms.Label();
            this.labelRetryRequest = new System.Windows.Forms.Label();
            this.panelConfigButton = new System.Windows.Forms.Panel();
            this.buttonConfigCancel = new System.Windows.Forms.Button();
            this.buttonConfigApply = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.panelConfigMain.SuspendLayout();
            this.groupBoxServers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serverInfoDataSourceBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configPanelModelBindingSource)).BeginInit();
            this.groupBoxPrePostDuration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrepostDuration)).BeginInit();
            this.groupBoxLogPath.SuspendLayout();
            this.tableLayoutPanelLogFileDirectory.SuspendLayout();
            this.groupBoxNotification.SuspendLayout();
            this.groupBoxOfficeSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOffice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.officeInfoDataSourceBindingSource)).BeginInit();
            this.groupBoxRealtimeSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSessionWait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRequestWait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSessionRetryCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRequestRetryCount)).BeginInit();
            this.panelConfigButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelConfigMain
            // 
            this.panelConfigMain.Controls.Add(this.groupBoxServers);
            this.panelConfigMain.Controls.Add(this.groupBoxPrePostDuration);
            this.panelConfigMain.Controls.Add(this.groupBoxLogPath);
            this.panelConfigMain.Controls.Add(this.groupBoxNotification);
            this.panelConfigMain.Controls.Add(this.groupBoxOfficeSetting);
            this.panelConfigMain.Controls.Add(this.groupBoxRealtimeSetting);
            this.panelConfigMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConfigMain.Location = new System.Drawing.Point(0, 0);
            this.panelConfigMain.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.panelConfigMain.Name = "panelConfigMain";
            this.panelConfigMain.Size = new System.Drawing.Size(1517, 1059);
            this.panelConfigMain.TabIndex = 0;
            // 
            // groupBoxServers
            // 
            this.groupBoxServers.Controls.Add(this.comboBoxServers);
            this.groupBoxServers.Location = new System.Drawing.Point(287, 614);
            this.groupBoxServers.Name = "groupBoxServers";
            this.groupBoxServers.Size = new System.Drawing.Size(340, 67);
            this.groupBoxServers.TabIndex = 5;
            this.groupBoxServers.TabStop = false;
            this.groupBoxServers.Text = "接続サーバー";
            // 
            // comboBoxServers
            // 
            this.comboBoxServers.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.configPanelModelBindingSource, "OperationServer", true));
            this.comboBoxServers.DataSource = this.serverInfoDataSourceBindingSource;
            this.comboBoxServers.DisplayMember = "Name";
            this.comboBoxServers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxServers.FormattingEnabled = true;
            this.comboBoxServers.Location = new System.Drawing.Point(17, 28);
            this.comboBoxServers.Name = "comboBoxServers";
            this.comboBoxServers.Size = new System.Drawing.Size(309, 26);
            this.comboBoxServers.TabIndex = 0;
            this.comboBoxServers.ValueMember = "Id";
            // 
            // serverInfoDataSourceBindingSource
            // 
            this.serverInfoDataSourceBindingSource.DataMember = "ServerInfoDataSource";
            this.serverInfoDataSourceBindingSource.DataSource = this.configPanelModelBindingSource;
            this.serverInfoDataSourceBindingSource.CurrentChanged += new System.EventHandler(this.serverInfoDataSourceBindingSource_CurrentChanged);
            // 
            // configPanelModelBindingSource
            // 
            this.configPanelModelBindingSource.DataSource = typeof(RealtimeViewer.Controls.ConfigPanelModel);
            // 
            // groupBoxPrePostDuration
            // 
            this.groupBoxPrePostDuration.Controls.Add(this.labelPrepostDuration);
            this.groupBoxPrePostDuration.Controls.Add(this.numericUpDownPrepostDuration);
            this.groupBoxPrePostDuration.Location = new System.Drawing.Point(22, 614);
            this.groupBoxPrePostDuration.Name = "groupBoxPrePostDuration";
            this.groupBoxPrePostDuration.Size = new System.Drawing.Size(238, 67);
            this.groupBoxPrePostDuration.TabIndex = 4;
            this.groupBoxPrePostDuration.TabStop = false;
            this.groupBoxPrePostDuration.Text = "Gセンサー値検索範囲(時間)";
            // 
            // labelPrepostDuration
            // 
            this.labelPrepostDuration.AutoSize = true;
            this.labelPrepostDuration.Location = new System.Drawing.Point(144, 32);
            this.labelPrepostDuration.Name = "labelPrepostDuration";
            this.labelPrepostDuration.Size = new System.Drawing.Size(40, 18);
            this.labelPrepostDuration.TabIndex = 1;
            this.labelPrepostDuration.Text = "秒間";
            // 
            // numericUpDownPrepostDuration
            // 
            this.numericUpDownPrepostDuration.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.configPanelModelBindingSource, "PrepostDuration", true));
            this.numericUpDownPrepostDuration.Location = new System.Drawing.Point(17, 28);
            this.numericUpDownPrepostDuration.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownPrepostDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPrepostDuration.Name = "numericUpDownPrepostDuration";
            this.numericUpDownPrepostDuration.Size = new System.Drawing.Size(120, 26);
            this.numericUpDownPrepostDuration.TabIndex = 0;
            this.numericUpDownPrepostDuration.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBoxLogPath
            // 
            this.groupBoxLogPath.Controls.Add(this.tableLayoutPanelLogFileDirectory);
            this.groupBoxLogPath.Location = new System.Drawing.Point(22, 494);
            this.groupBoxLogPath.Name = "groupBoxLogPath";
            this.groupBoxLogPath.Size = new System.Drawing.Size(608, 100);
            this.groupBoxLogPath.TabIndex = 3;
            this.groupBoxLogPath.TabStop = false;
            this.groupBoxLogPath.Text = "ログファイル出力先";
            // 
            // tableLayoutPanelLogFileDirectory
            // 
            this.tableLayoutPanelLogFileDirectory.ColumnCount = 2;
            this.tableLayoutPanelLogFileDirectory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelLogFileDirectory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelLogFileDirectory.Controls.Add(this.buttonDirectoryClear, 1, 1);
            this.tableLayoutPanelLogFileDirectory.Controls.Add(this.labelLogFileDirectory, 0, 0);
            this.tableLayoutPanelLogFileDirectory.Controls.Add(this.buttonOpenLogDirectory, 0, 1);
            this.tableLayoutPanelLogFileDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelLogFileDirectory.Location = new System.Drawing.Point(3, 22);
            this.tableLayoutPanelLogFileDirectory.Name = "tableLayoutPanelLogFileDirectory";
            this.tableLayoutPanelLogFileDirectory.RowCount = 2;
            this.tableLayoutPanelLogFileDirectory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelLogFileDirectory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelLogFileDirectory.Size = new System.Drawing.Size(602, 75);
            this.tableLayoutPanelLogFileDirectory.TabIndex = 1;
            // 
            // buttonDirectoryClear
            // 
            this.buttonDirectoryClear.Location = new System.Drawing.Point(122, 42);
            this.buttonDirectoryClear.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.buttonDirectoryClear.Name = "buttonDirectoryClear";
            this.buttonDirectoryClear.Size = new System.Drawing.Size(113, 30);
            this.buttonDirectoryClear.TabIndex = 4;
            this.buttonDirectoryClear.Text = "フォルダクリア";
            this.buttonDirectoryClear.UseVisualStyleBackColor = true;
            this.buttonDirectoryClear.Click += new System.EventHandler(this.ButtonDirectoryClear_Click);
            // 
            // labelLogFileDirectory
            // 
            this.labelLogFileDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLogFileDirectory.AutoSize = true;
            this.tableLayoutPanelLogFileDirectory.SetColumnSpan(this.labelLogFileDirectory, 2);
            this.labelLogFileDirectory.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.configPanelModelBindingSource, "LogFileDirectory", true));
            this.labelLogFileDirectory.Location = new System.Drawing.Point(3, 3);
            this.labelLogFileDirectory.Margin = new System.Windows.Forms.Padding(3);
            this.labelLogFileDirectory.Name = "labelLogFileDirectory";
            this.labelLogFileDirectory.Size = new System.Drawing.Size(596, 31);
            this.labelLogFileDirectory.TabIndex = 0;
            this.labelLogFileDirectory.Text = "label1";
            this.labelLogFileDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOpenLogDirectory
            // 
            this.buttonOpenLogDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOpenLogDirectory.Location = new System.Drawing.Point(3, 42);
            this.buttonOpenLogDirectory.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.buttonOpenLogDirectory.Name = "buttonOpenLogDirectory";
            this.buttonOpenLogDirectory.Size = new System.Drawing.Size(113, 30);
            this.buttonOpenLogDirectory.TabIndex = 1;
            this.buttonOpenLogDirectory.Text = "フォルダ選択";
            this.buttonOpenLogDirectory.UseVisualStyleBackColor = true;
            this.buttonOpenLogDirectory.Click += new System.EventHandler(this.ButtonOpenLogDirectory_Click);
            // 
            // groupBoxNotification
            // 
            this.groupBoxNotification.Controls.Add(this.checkBoxPopUpEmergency);
            this.groupBoxNotification.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.configPanelModelBindingSource, "IsApplyConfig", true));
            this.groupBoxNotification.Font = new System.Drawing.Font("UD デジタル 教科書体 NK-R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBoxNotification.Location = new System.Drawing.Point(22, 404);
            this.groupBoxNotification.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBoxNotification.Name = "groupBoxNotification";
            this.groupBoxNotification.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBoxNotification.Size = new System.Drawing.Size(608, 67);
            this.groupBoxNotification.TabIndex = 2;
            this.groupBoxNotification.TabStop = false;
            this.groupBoxNotification.Text = "通知設定";
            // 
            // checkBoxPopUpEmergency
            // 
            this.checkBoxPopUpEmergency.AutoSize = true;
            this.checkBoxPopUpEmergency.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.configPanelModelBindingSource, "UseEmergencyPopUp", true));
            this.checkBoxPopUpEmergency.Location = new System.Drawing.Point(17, 29);
            this.checkBoxPopUpEmergency.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.checkBoxPopUpEmergency.Name = "checkBoxPopUpEmergency";
            this.checkBoxPopUpEmergency.Size = new System.Drawing.Size(164, 22);
            this.checkBoxPopUpEmergency.TabIndex = 0;
            this.checkBoxPopUpEmergency.Text = "緊急通報を表示する";
            this.checkBoxPopUpEmergency.UseVisualStyleBackColor = true;
            // 
            // groupBoxOfficeSetting
            // 
            this.groupBoxOfficeSetting.Controls.Add(this.dataGridViewOffice);
            this.groupBoxOfficeSetting.Font = new System.Drawing.Font("UD デジタル 教科書体 NK-R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBoxOfficeSetting.Location = new System.Drawing.Point(22, 159);
            this.groupBoxOfficeSetting.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBoxOfficeSetting.Name = "groupBoxOfficeSetting";
            this.groupBoxOfficeSetting.Padding = new System.Windows.Forms.Padding(17, 15, 17, 15);
            this.groupBoxOfficeSetting.Size = new System.Drawing.Size(608, 225);
            this.groupBoxOfficeSetting.TabIndex = 1;
            this.groupBoxOfficeSetting.TabStop = false;
            this.groupBoxOfficeSetting.Text = "営業所リスト表示設定";
            // 
            // dataGridViewOffice
            // 
            this.dataGridViewOffice.AllowUserToAddRows = false;
            this.dataGridViewOffice.AllowUserToDeleteRows = false;
            this.dataGridViewOffice.AllowUserToResizeRows = false;
            this.dataGridViewOffice.AutoGenerateColumns = false;
            this.dataGridViewOffice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOffice.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.visibleDataGridViewCheckBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.locationDataGridViewTextBoxColumn,
            this.idDataGridViewTextBoxColumn,
            this.companyIdDataGridViewTextBoxColumn});
            this.dataGridViewOffice.DataSource = this.officeInfoDataSourceBindingSource;
            this.dataGridViewOffice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewOffice.Location = new System.Drawing.Point(17, 34);
            this.dataGridViewOffice.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dataGridViewOffice.Name = "dataGridViewOffice";
            this.dataGridViewOffice.RowHeadersVisible = false;
            this.dataGridViewOffice.RowTemplate.Height = 21;
            this.dataGridViewOffice.Size = new System.Drawing.Size(574, 176);
            this.dataGridViewOffice.TabIndex = 0;
            this.dataGridViewOffice.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.DataGridViewOffice_CellBeginEdit);
            this.dataGridViewOffice.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewOffice_CellEndEdit);
            this.dataGridViewOffice.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.DataGridViewOffice_CellParsing);
            this.dataGridViewOffice.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DataGridViewOffice_CellValidating);
            this.dataGridViewOffice.CurrentCellDirtyStateChanged += new System.EventHandler(this.DataGridViewOffice_CurrentCellDirtyStateChanged);
            // 
            // visibleDataGridViewCheckBoxColumn
            // 
            this.visibleDataGridViewCheckBoxColumn.DataPropertyName = "Visible";
            this.visibleDataGridViewCheckBoxColumn.HeaderText = "";
            this.visibleDataGridViewCheckBoxColumn.MinimumWidth = 25;
            this.visibleDataGridViewCheckBoxColumn.Name = "visibleDataGridViewCheckBoxColumn";
            this.visibleDataGridViewCheckBoxColumn.Width = 25;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "営業所名";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // locationDataGridViewTextBoxColumn
            // 
            this.locationDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.locationDataGridViewTextBoxColumn.DataPropertyName = "Location";
            this.locationDataGridViewTextBoxColumn.HeaderText = "所在地(緯度, 経度)";
            this.locationDataGridViewTextBoxColumn.MinimumWidth = 84;
            this.locationDataGridViewTextBoxColumn.Name = "locationDataGridViewTextBoxColumn";
            this.locationDataGridViewTextBoxColumn.Width = 119;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            this.idDataGridViewTextBoxColumn.Visible = false;
            // 
            // companyIdDataGridViewTextBoxColumn
            // 
            this.companyIdDataGridViewTextBoxColumn.DataPropertyName = "CompanyId";
            this.companyIdDataGridViewTextBoxColumn.HeaderText = "CompanyId";
            this.companyIdDataGridViewTextBoxColumn.Name = "companyIdDataGridViewTextBoxColumn";
            this.companyIdDataGridViewTextBoxColumn.ReadOnly = true;
            this.companyIdDataGridViewTextBoxColumn.Visible = false;
            // 
            // officeInfoDataSourceBindingSource
            // 
            this.officeInfoDataSourceBindingSource.DataMember = "OfficeInfoDataSource";
            this.officeInfoDataSourceBindingSource.DataSource = this.configPanelModelBindingSource;
            // 
            // groupBoxRealtimeSetting
            // 
            this.groupBoxRealtimeSetting.Controls.Add(this.numericUpDownSessionWait);
            this.groupBoxRealtimeSetting.Controls.Add(this.labelWaitSession);
            this.groupBoxRealtimeSetting.Controls.Add(this.numericUpDownRequestWait);
            this.groupBoxRealtimeSetting.Controls.Add(this.labelWaitRequest);
            this.groupBoxRealtimeSetting.Controls.Add(this.numericUpDownSessionRetryCount);
            this.groupBoxRealtimeSetting.Controls.Add(this.numericUpDownRequestRetryCount);
            this.groupBoxRealtimeSetting.Controls.Add(this.labelRetrySession);
            this.groupBoxRealtimeSetting.Controls.Add(this.labelRetryRequest);
            this.groupBoxRealtimeSetting.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.configPanelModelBindingSource, "IsApplyConfig", true));
            this.groupBoxRealtimeSetting.Font = new System.Drawing.Font("UD デジタル 教科書体 NK-R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBoxRealtimeSetting.Location = new System.Drawing.Point(22, 27);
            this.groupBoxRealtimeSetting.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBoxRealtimeSetting.Name = "groupBoxRealtimeSetting";
            this.groupBoxRealtimeSetting.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.groupBoxRealtimeSetting.Size = new System.Drawing.Size(608, 114);
            this.groupBoxRealtimeSetting.TabIndex = 0;
            this.groupBoxRealtimeSetting.TabStop = false;
            this.groupBoxRealtimeSetting.Text = "リアルタイム再生設定";
            // 
            // numericUpDownSessionWait
            // 
            this.numericUpDownSessionWait.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.configPanelModelBindingSource, "StreamingSessionWait", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownSessionWait.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.numericUpDownSessionWait.Location = new System.Drawing.Point(499, 29);
            this.numericUpDownSessionWait.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.numericUpDownSessionWait.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDownSessionWait.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownSessionWait.Name = "numericUpDownSessionWait";
            this.numericUpDownSessionWait.Size = new System.Drawing.Size(82, 26);
            this.numericUpDownSessionWait.TabIndex = 3;
            this.numericUpDownSessionWait.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // labelWaitSession
            // 
            this.labelWaitSession.AutoSize = true;
            this.labelWaitSession.Location = new System.Drawing.Point(293, 31);
            this.labelWaitSession.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelWaitSession.Name = "labelWaitSession";
            this.labelWaitSession.Size = new System.Drawing.Size(196, 18);
            this.labelWaitSession.TabIndex = 2;
            this.labelWaitSession.Text = "配信セッション待ち時間(秒)";
            // 
            // numericUpDownRequestWait
            // 
            this.numericUpDownRequestWait.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.configPanelModelBindingSource, "StreamingRequestWait", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownRequestWait.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.numericUpDownRequestWait.Location = new System.Drawing.Point(186, 29);
            this.numericUpDownRequestWait.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.numericUpDownRequestWait.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDownRequestWait.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownRequestWait.Name = "numericUpDownRequestWait";
            this.numericUpDownRequestWait.Size = new System.Drawing.Size(82, 26);
            this.numericUpDownRequestWait.TabIndex = 1;
            this.numericUpDownRequestWait.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // labelWaitRequest
            // 
            this.labelWaitRequest.AutoSize = true;
            this.labelWaitRequest.Location = new System.Drawing.Point(12, 31);
            this.labelWaitRequest.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelWaitRequest.Name = "labelWaitRequest";
            this.labelWaitRequest.Size = new System.Drawing.Size(164, 18);
            this.labelWaitRequest.TabIndex = 0;
            this.labelWaitRequest.Text = "配信要求待ち時間(秒)";
            // 
            // numericUpDownSessionRetryCount
            // 
            this.numericUpDownSessionRetryCount.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.configPanelModelBindingSource, "StreamingSessionRetryCount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownSessionRetryCount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.numericUpDownSessionRetryCount.Location = new System.Drawing.Point(499, 71);
            this.numericUpDownSessionRetryCount.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.numericUpDownSessionRetryCount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownSessionRetryCount.Name = "numericUpDownSessionRetryCount";
            this.numericUpDownSessionRetryCount.Size = new System.Drawing.Size(82, 26);
            this.numericUpDownSessionRetryCount.TabIndex = 7;
            // 
            // numericUpDownRequestRetryCount
            // 
            this.numericUpDownRequestRetryCount.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.configPanelModelBindingSource, "StreamingRequestRetryCount", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.numericUpDownRequestRetryCount.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.numericUpDownRequestRetryCount.Location = new System.Drawing.Point(186, 71);
            this.numericUpDownRequestRetryCount.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.numericUpDownRequestRetryCount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownRequestRetryCount.Name = "numericUpDownRequestRetryCount";
            this.numericUpDownRequestRetryCount.Size = new System.Drawing.Size(82, 26);
            this.numericUpDownRequestRetryCount.TabIndex = 5;
            // 
            // labelRetrySession
            // 
            this.labelRetrySession.AutoSize = true;
            this.labelRetrySession.Location = new System.Drawing.Point(293, 73);
            this.labelRetrySession.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelRetrySession.Name = "labelRetrySession";
            this.labelRetrySession.Size = new System.Drawing.Size(183, 18);
            this.labelRetrySession.TabIndex = 6;
            this.labelRetrySession.Text = "配信セッションリトライ回数";
            // 
            // labelRetryRequest
            // 
            this.labelRetryRequest.AutoSize = true;
            this.labelRetryRequest.Location = new System.Drawing.Point(12, 73);
            this.labelRetryRequest.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelRetryRequest.Name = "labelRetryRequest";
            this.labelRetryRequest.Size = new System.Drawing.Size(151, 18);
            this.labelRetryRequest.TabIndex = 4;
            this.labelRetryRequest.Text = "配信要求リトライ回数";
            // 
            // panelConfigButton
            // 
            this.panelConfigButton.Controls.Add(this.buttonConfigCancel);
            this.panelConfigButton.Controls.Add(this.buttonConfigApply);
            this.panelConfigButton.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.configPanelModelBindingSource, "IsApplyConfig", true));
            this.panelConfigButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelConfigButton.Location = new System.Drawing.Point(0, 1009);
            this.panelConfigButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.panelConfigButton.Name = "panelConfigButton";
            this.panelConfigButton.Size = new System.Drawing.Size(1517, 50);
            this.panelConfigButton.TabIndex = 1;
            // 
            // buttonConfigCancel
            // 
            this.buttonConfigCancel.Font = new System.Drawing.Font("UD デジタル 教科書体 NK-R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonConfigCancel.Location = new System.Drawing.Point(136, 13);
            this.buttonConfigCancel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.buttonConfigCancel.Name = "buttonConfigCancel";
            this.buttonConfigCancel.Size = new System.Drawing.Size(100, 25);
            this.buttonConfigCancel.TabIndex = 1;
            this.buttonConfigCancel.Text = "元に戻す";
            this.buttonConfigCancel.UseVisualStyleBackColor = true;
            this.buttonConfigCancel.Click += new System.EventHandler(this.ButtonConfigCancel_Click);
            // 
            // buttonConfigApply
            // 
            this.buttonConfigApply.Font = new System.Drawing.Font("UD デジタル 教科書体 NK-R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonConfigApply.Location = new System.Drawing.Point(22, 13);
            this.buttonConfigApply.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.buttonConfigApply.Name = "buttonConfigApply";
            this.buttonConfigApply.Size = new System.Drawing.Size(100, 25);
            this.buttonConfigApply.TabIndex = 0;
            this.buttonConfigApply.Text = "適用";
            this.buttonConfigApply.UseVisualStyleBackColor = true;
            this.buttonConfigApply.Click += new System.EventHandler(this.ButtonConfigApply_Click);
            // 
            // ConfigPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelConfigButton);
            this.Controls.Add(this.panelConfigMain);
            this.Font = new System.Drawing.Font("UD デジタル 教科書体 NK-R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "ConfigPanel";
            this.Size = new System.Drawing.Size(1517, 1059);
            this.panelConfigMain.ResumeLayout(false);
            this.groupBoxServers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.serverInfoDataSourceBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configPanelModelBindingSource)).EndInit();
            this.groupBoxPrePostDuration.ResumeLayout(false);
            this.groupBoxPrePostDuration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPrepostDuration)).EndInit();
            this.groupBoxLogPath.ResumeLayout(false);
            this.tableLayoutPanelLogFileDirectory.ResumeLayout(false);
            this.tableLayoutPanelLogFileDirectory.PerformLayout();
            this.groupBoxNotification.ResumeLayout(false);
            this.groupBoxNotification.PerformLayout();
            this.groupBoxOfficeSetting.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOffice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.officeInfoDataSourceBindingSource)).EndInit();
            this.groupBoxRealtimeSetting.ResumeLayout(false);
            this.groupBoxRealtimeSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSessionWait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRequestWait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSessionRetryCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRequestRetryCount)).EndInit();
            this.panelConfigButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelConfigMain;
        private System.Windows.Forms.GroupBox groupBoxNotification;
        private System.Windows.Forms.CheckBox checkBoxPopUpEmergency;
        private System.Windows.Forms.GroupBox groupBoxOfficeSetting;
        private System.Windows.Forms.DataGridView dataGridViewOffice;
        private System.Windows.Forms.GroupBox groupBoxRealtimeSetting;
        private System.Windows.Forms.NumericUpDown numericUpDownSessionWait;
        private System.Windows.Forms.Label labelWaitSession;
        private System.Windows.Forms.NumericUpDown numericUpDownRequestWait;
        private System.Windows.Forms.Label labelWaitRequest;
        private System.Windows.Forms.NumericUpDown numericUpDownSessionRetryCount;
        private System.Windows.Forms.NumericUpDown numericUpDownRequestRetryCount;
        private System.Windows.Forms.Label labelRetrySession;
        private System.Windows.Forms.Label labelRetryRequest;
        private System.Windows.Forms.BindingSource configPanelModelBindingSource;
        private System.Windows.Forms.BindingSource officeInfoDataSourceBindingSource;
        private System.Windows.Forms.Panel panelConfigButton;
        private System.Windows.Forms.Button buttonConfigCancel;
        private System.Windows.Forms.Button buttonConfigApply;
        private System.Windows.Forms.DataGridViewCheckBoxColumn visibleDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn locationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn companyIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.GroupBox groupBoxLogPath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLogFileDirectory;
        private System.Windows.Forms.Button buttonOpenLogDirectory;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label labelLogFileDirectory;
        private System.Windows.Forms.Button buttonDirectoryClear;
        private System.Windows.Forms.GroupBox groupBoxPrePostDuration;
        private System.Windows.Forms.Label labelPrepostDuration;
        private System.Windows.Forms.NumericUpDown numericUpDownPrepostDuration;
        private System.Windows.Forms.GroupBox groupBoxServers;
        private System.Windows.Forms.ComboBox comboBoxServers;
        private System.Windows.Forms.BindingSource serverInfoDataSourceBindingSource;
    }
}
