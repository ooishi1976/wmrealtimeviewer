
using RealtimeViewer.Model;

namespace RealtimeViewer
{
    partial class MovieRequestWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.movieRequestWindowViewModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.gridEventList = new System.Windows.Forms.DataGridView();
            this.eventInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panelPlay = new System.Windows.Forms.Panel();
            this.buttonPlay8 = new System.Windows.Forms.Button();
            this.buttonPlay7 = new System.Windows.Forms.Button();
            this.buttonPlay6 = new System.Windows.Forms.Button();
            this.buttonPlay5 = new System.Windows.Forms.Button();
            this.buttonPlay4 = new System.Windows.Forms.Button();
            this.buttonPlay3 = new System.Windows.Forms.Button();
            this.buttonPlay2 = new System.Windows.Forms.Button();
            this.buttonPlay1 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.labelDownload = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelUploadComplete = new System.Windows.Forms.Label();
            this.labelValidateError = new System.Windows.Forms.Label();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelProgress = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.IsDownloadable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.timestampDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.carIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDownload = new System.Windows.Forms.DataGridViewButtonColumn();
            this.MovieId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsPlayable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.movieRequestWindowViewModelBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridEventList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventInfoBindingSource)).BeginInit();
            this.panelPlay.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(25, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "社番：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(86, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "0000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(383, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "取得対象とする時間を１０分以内の範囲で設定してください";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(302, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "時";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(302, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "時";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(384, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 20);
            this.label6.TabIndex = 5;
            this.label6.Text = "分から";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(384, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 20);
            this.label7.TabIndex = 10;
            this.label7.Text = "分まで";
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(454, 82);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(249, 36);
            this.buttonUpload.TabIndex = 11;
            this.buttonUpload.Text = "車載器からサーバーへ送信";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(1150, 768);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(124, 44);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "閉じる";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.movieRequestWindowViewModelBindingSource, "RangeStart", true));
            this.dateTimePicker1.Location = new System.Drawing.Point(79, 68);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(165, 26);
            this.dateTimePicker1.TabIndex = 1;
            // 
            // movieRequestWindowViewModelBindingSource
            // 
            this.movieRequestWindowViewModelBindingSource.DataSource = typeof(RealtimeViewer.MovieRequestWindowViewModel);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.movieRequestWindowViewModelBindingSource, "RangeEnd", true));
            this.dateTimePicker2.Location = new System.Drawing.Point(79, 104);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(165, 26);
            this.dateTimePicker2.TabIndex = 6;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.movieRequestWindowViewModelBindingSource, "StartHour", true));
            this.numericUpDown1.Location = new System.Drawing.Point(250, 68);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(46, 26);
            this.numericUpDown1.TabIndex = 2;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.movieRequestWindowViewModelBindingSource, "StartMinute", true));
            this.numericUpDown2.Location = new System.Drawing.Point(332, 68);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(46, 26);
            this.numericUpDown2.TabIndex = 4;
            this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.movieRequestWindowViewModelBindingSource, "EndHour", true));
            this.numericUpDown3.Location = new System.Drawing.Point(250, 104);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(46, 26);
            this.numericUpDown3.TabIndex = 7;
            this.numericUpDown3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.movieRequestWindowViewModelBindingSource, "EndMinute", true));
            this.numericUpDown4.Location = new System.Drawing.Point(332, 104);
            this.numericUpDown4.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(46, 26);
            this.numericUpDown4.TabIndex = 9;
            this.numericUpDown4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(779, 132);
            this.progressBar1.MarqueeAnimationSpeed = 50;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(266, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Visible = false;
            // 
            // gridEventList
            // 
            this.gridEventList.AllowUserToAddRows = false;
            this.gridEventList.AllowUserToDeleteRows = false;
            this.gridEventList.AllowUserToResizeRows = false;
            this.gridEventList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridEventList.AutoGenerateColumns = false;
            this.gridEventList.ColumnHeadersHeight = 26;
            this.gridEventList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridEventList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsDownloadable,
            this.timestampDataGridViewTextBoxColumn,
            this.carIdDataGridViewTextBoxColumn,
            this.Remarks,
            this.ColumnDownload,
            this.MovieId,
            this.IsPlayable});
            this.gridEventList.DataSource = this.eventInfoBindingSource;
            this.gridEventList.Location = new System.Drawing.Point(33, 79);
            this.gridEventList.Margin = new System.Windows.Forms.Padding(2);
            this.gridEventList.Name = "gridEventList";
            this.gridEventList.RowHeadersVisible = false;
            this.gridEventList.RowHeadersWidth = 4;
            this.gridEventList.Size = new System.Drawing.Size(670, 448);
            this.gridEventList.TabIndex = 2;
            this.gridEventList.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.GridEventList_CellFormatting);
            this.gridEventList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridEventList_CellValueChanged);
            this.gridEventList.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.GridEventList_ColumnHeaderMouseClick);
            this.gridEventList.CurrentCellDirtyStateChanged += new System.EventHandler(this.GridEventList_CurrentCellDirtyStateChanged);
            // 
            // eventInfoBindingSource
            // 
            this.eventInfoBindingSource.DataSource = typeof(RealtimeViewer.Model.EventInfo);
            // 
            // panelPlay
            // 
            this.panelPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPlay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPlay.Controls.Add(this.buttonPlay8);
            this.panelPlay.Controls.Add(this.buttonPlay7);
            this.panelPlay.Controls.Add(this.buttonPlay6);
            this.panelPlay.Controls.Add(this.buttonPlay5);
            this.panelPlay.Controls.Add(this.buttonPlay4);
            this.panelPlay.Controls.Add(this.buttonPlay3);
            this.panelPlay.Controls.Add(this.buttonPlay2);
            this.panelPlay.Controls.Add(this.buttonPlay1);
            this.panelPlay.Location = new System.Drawing.Point(779, 248);
            this.panelPlay.Name = "panelPlay";
            this.panelPlay.Size = new System.Drawing.Size(504, 101);
            this.panelPlay.TabIndex = 8;
            this.panelPlay.Visible = false;
            // 
            // buttonPlay8
            // 
            this.buttonPlay8.Enabled = false;
            this.buttonPlay8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay8.Location = new System.Drawing.Point(380, 57);
            this.buttonPlay8.Name = "buttonPlay8";
            this.buttonPlay8.Size = new System.Drawing.Size(100, 25);
            this.buttonPlay8.TabIndex = 7;
            this.buttonPlay8.Text = "Ch8 再生";
            this.buttonPlay8.UseVisualStyleBackColor = true;
            this.buttonPlay8.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay7
            // 
            this.buttonPlay7.Enabled = false;
            this.buttonPlay7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay7.Location = new System.Drawing.Point(260, 57);
            this.buttonPlay7.Name = "buttonPlay7";
            this.buttonPlay7.Size = new System.Drawing.Size(100, 25);
            this.buttonPlay7.TabIndex = 6;
            this.buttonPlay7.Text = "Ch7 再生";
            this.buttonPlay7.UseVisualStyleBackColor = true;
            this.buttonPlay7.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay6
            // 
            this.buttonPlay6.Enabled = false;
            this.buttonPlay6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay6.Location = new System.Drawing.Point(140, 57);
            this.buttonPlay6.Name = "buttonPlay6";
            this.buttonPlay6.Size = new System.Drawing.Size(100, 25);
            this.buttonPlay6.TabIndex = 5;
            this.buttonPlay6.Text = "Ch6 再生";
            this.buttonPlay6.UseVisualStyleBackColor = true;
            this.buttonPlay6.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay5
            // 
            this.buttonPlay5.Enabled = false;
            this.buttonPlay5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay5.Location = new System.Drawing.Point(20, 57);
            this.buttonPlay5.Name = "buttonPlay5";
            this.buttonPlay5.Size = new System.Drawing.Size(100, 25);
            this.buttonPlay5.TabIndex = 4;
            this.buttonPlay5.Text = "Ch5 再生";
            this.buttonPlay5.UseVisualStyleBackColor = true;
            this.buttonPlay5.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay4
            // 
            this.buttonPlay4.Enabled = false;
            this.buttonPlay4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay4.Location = new System.Drawing.Point(380, 17);
            this.buttonPlay4.Name = "buttonPlay4";
            this.buttonPlay4.Size = new System.Drawing.Size(100, 25);
            this.buttonPlay4.TabIndex = 3;
            this.buttonPlay4.Text = "Ch4 再生";
            this.buttonPlay4.UseVisualStyleBackColor = true;
            this.buttonPlay4.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay3
            // 
            this.buttonPlay3.Enabled = false;
            this.buttonPlay3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay3.Location = new System.Drawing.Point(260, 17);
            this.buttonPlay3.Name = "buttonPlay3";
            this.buttonPlay3.Size = new System.Drawing.Size(100, 25);
            this.buttonPlay3.TabIndex = 2;
            this.buttonPlay3.Text = "Ch3 再生";
            this.buttonPlay3.UseVisualStyleBackColor = true;
            this.buttonPlay3.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay2
            // 
            this.buttonPlay2.Enabled = false;
            this.buttonPlay2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay2.Location = new System.Drawing.Point(140, 17);
            this.buttonPlay2.Name = "buttonPlay2";
            this.buttonPlay2.Size = new System.Drawing.Size(100, 25);
            this.buttonPlay2.TabIndex = 1;
            this.buttonPlay2.Text = "Ch2 再生";
            this.buttonPlay2.UseVisualStyleBackColor = true;
            this.buttonPlay2.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // buttonPlay1
            // 
            this.buttonPlay1.Enabled = false;
            this.buttonPlay1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonPlay1.Location = new System.Drawing.Point(20, 17);
            this.buttonPlay1.Name = "buttonPlay1";
            this.buttonPlay1.Size = new System.Drawing.Size(100, 25);
            this.buttonPlay1.TabIndex = 0;
            this.buttonPlay1.Text = "Ch1 再生";
            this.buttonPlay1.UseVisualStyleBackColor = true;
            this.buttonPlay1.Click += new System.EventHandler(this.buttonPlay1_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(780, 96);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(247, 20);
            this.label12.TabIndex = 5;
            this.label12.Text = "車載器からサーバにアップロード中です";
            this.label12.Visible = false;
            // 
            // labelDownload
            // 
            this.labelDownload.AutoSize = true;
            this.labelDownload.Location = new System.Drawing.Point(782, 85);
            this.labelDownload.Name = "labelDownload";
            this.labelDownload.Size = new System.Drawing.Size(207, 20);
            this.labelDownload.TabIndex = 4;
            this.labelDownload.Text = "映像データのダウンロード中です";
            this.labelDownload.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(780, 106);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(129, 20);
            this.label8.TabIndex = 6;
            this.label8.Text = "映像の準備中です";
            this.label8.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(36, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(405, 20);
            this.label9.TabIndex = 0;
            this.label9.Text = "再生したい映像にチェックをつけてダウンロードを開始してください";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelUploadComplete);
            this.groupBox1.Controls.Add(this.labelValidateError);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.buttonUpload);
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.dateTimePicker2);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Controls.Add(this.numericUpDown4);
            this.groupBox1.Controls.Add(this.numericUpDown2);
            this.groupBox1.Controls.Add(this.numericUpDown3);
            this.groupBox1.Location = new System.Drawing.Point(25, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(724, 166);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "車載器からサーバーへデータを格納";
            // 
            // labelUploadComplete
            // 
            this.labelUploadComplete.AutoSize = true;
            this.labelUploadComplete.Location = new System.Drawing.Point(451, 134);
            this.labelUploadComplete.Name = "labelUploadComplete";
            this.labelUploadComplete.Size = new System.Drawing.Size(0, 20);
            this.labelUploadComplete.TabIndex = 17;
            // 
            // labelValidateError
            // 
            this.labelValidateError.AutoSize = true;
            this.labelValidateError.ForeColor = System.Drawing.Color.Red;
            this.labelValidateError.Location = new System.Drawing.Point(76, 145);
            this.labelValidateError.Name = "labelValidateError";
            this.labelValidateError.Size = new System.Drawing.Size(174, 20);
            this.labelValidateError.TabIndex = 12;
            this.labelValidateError.Text = "これはサンプルのエラーです";
            // 
            // buttonDownload
            // 
            this.buttonDownload.Enabled = false;
            this.buttonDownload.Location = new System.Drawing.Point(488, 32);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(215, 36);
            this.buttonDownload.TabIndex = 1;
            this.buttonDownload.Text = "サーバーからダウンロード";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.ButtonDownload_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.buttonDownload);
            this.groupBox2.Controls.Add(this.gridEventList);
            this.groupBox2.Location = new System.Drawing.Point(25, 248);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(724, 564);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "サーバー上のドラレコ映像";
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.Location = new System.Drawing.Point(784, 170);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(0, 20);
            this.labelProgress.TabIndex = 27;
            this.labelProgress.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(797, 365);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(153, 20);
            this.label10.TabIndex = 9;
            this.label10.Text = "音声データがありません";
            this.label10.Visible = false;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(797, 383);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(164, 20);
            this.label11.TabIndex = 10;
            this.label11.Text = "再生ステータスのサンプル";
            this.label11.Visible = false;
            // 
            // IsDownloadable
            // 
            this.IsDownloadable.FalseValue = "0";
            this.IsDownloadable.HeaderText = "";
            this.IsDownloadable.MinimumWidth = 25;
            this.IsDownloadable.Name = "IsDownloadable";
            this.IsDownloadable.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.IsDownloadable.TrueValue = "1";
            this.IsDownloadable.Width = 25;
            // 
            // timestampDataGridViewTextBoxColumn
            // 
            this.timestampDataGridViewTextBoxColumn.DataPropertyName = "Timestamp";
            this.timestampDataGridViewTextBoxColumn.FillWeight = 200F;
            this.timestampDataGridViewTextBoxColumn.HeaderText = "日付";
            this.timestampDataGridViewTextBoxColumn.MinimumWidth = 167;
            this.timestampDataGridViewTextBoxColumn.Name = "timestampDataGridViewTextBoxColumn";
            this.timestampDataGridViewTextBoxColumn.ReadOnly = true;
            this.timestampDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.timestampDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.timestampDataGridViewTextBoxColumn.Width = 167;
            // 
            // carIdDataGridViewTextBoxColumn
            // 
            this.carIdDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.carIdDataGridViewTextBoxColumn.DataPropertyName = "CarId";
            this.carIdDataGridViewTextBoxColumn.HeaderText = "社番";
            this.carIdDataGridViewTextBoxColumn.MinimumWidth = 65;
            this.carIdDataGridViewTextBoxColumn.Name = "carIdDataGridViewTextBoxColumn";
            this.carIdDataGridViewTextBoxColumn.ReadOnly = true;
            this.carIdDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.carIdDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.carIdDataGridViewTextBoxColumn.Width = 66;
            // 
            // Remarks
            // 
            this.Remarks.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Remarks.DataPropertyName = "Remarks";
            this.Remarks.FillWeight = 300F;
            this.Remarks.HeaderText = "備考";
            this.Remarks.MinimumWidth = 10;
            this.Remarks.Name = "Remarks";
            this.Remarks.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Remarks.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // ColumnDownload
            // 
            this.ColumnDownload.DataPropertyName = "MovieId";
            this.ColumnDownload.HeaderText = "ダウンロード";
            this.ColumnDownload.MinimumWidth = 10;
            this.ColumnDownload.Name = "ColumnDownload";
            this.ColumnDownload.ReadOnly = true;
            this.ColumnDownload.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnDownload.Text = "開始";
            this.ColumnDownload.UseColumnTextForButtonValue = true;
            this.ColumnDownload.Visible = false;
            this.ColumnDownload.Width = 66;
            // 
            // MovieId
            // 
            this.MovieId.DataPropertyName = "MovieId";
            this.MovieId.HeaderText = "MovieId";
            this.MovieId.Name = "MovieId";
            this.MovieId.ReadOnly = true;
            this.MovieId.Visible = false;
            this.MovieId.Width = 69;
            // 
            // IsPlayable
            // 
            this.IsPlayable.DataPropertyName = "IsPlayable";
            this.IsPlayable.FalseValue = "false";
            this.IsPlayable.HeaderText = "IsPlayable";
            this.IsPlayable.Name = "IsPlayable";
            this.IsPlayable.TrueValue = "true";
            this.IsPlayable.Visible = false;
            // 
            // MovieRequestWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(1313, 850);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.labelDownload);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.panelPlay);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MovieRequestWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ドラレコ映像";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MovieRequestWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MovieRequestWindow_FormClosed);
            this.Load += new System.EventHandler(this.MovieRequestWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.movieRequestWindowViewModelBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridEventList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventInfoBindingSource)).EndInit();
            this.panelPlay.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.BindingSource eventInfoBindingSource;
        private System.Windows.Forms.DataGridView gridEventList;
        private System.Windows.Forms.Panel panelPlay;
        private System.Windows.Forms.Button buttonPlay8;
        private System.Windows.Forms.Button buttonPlay7;
        private System.Windows.Forms.Button buttonPlay6;
        private System.Windows.Forms.Button buttonPlay5;
        private System.Windows.Forms.Button buttonPlay4;
        private System.Windows.Forms.Button buttonPlay3;
        private System.Windows.Forms.Button buttonPlay2;
        private System.Windows.Forms.Button buttonPlay1;
        private System.Windows.Forms.BindingSource movieRequestWindowViewModelBindingSource;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelDownload;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Label labelValidateError;
        private System.Windows.Forms.Label labelUploadComplete;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsDownloadable;
        private System.Windows.Forms.DataGridViewTextBoxColumn timestampDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn carIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remarks;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnDownload;
        private System.Windows.Forms.DataGridViewTextBoxColumn MovieId;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsPlayable;
    }
}