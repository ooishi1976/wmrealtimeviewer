namespace RealtimeViewer
{
    partial class WaitingForm
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
            this.label17 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelAuthWarning = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label17.Font = new System.Drawing.Font("UD デジタル 教科書体 NK-R", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label17.ForeColor = System.Drawing.Color.White;
            this.label17.Location = new System.Drawing.Point(89, 72);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(163, 22);
            this.label17.TabIndex = 3;
            this.label17.Text = "車両データ取得中";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(69, 112);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(203, 28);
            this.progressBar1.TabIndex = 4;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 5250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("UD デジタル 教科書体 NK-R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonCancel.Location = new System.Drawing.Point(120, 180);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 30);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelAuthWarning
            // 
            this.labelAuthWarning.AutoSize = true;
            this.labelAuthWarning.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelAuthWarning.Font = new System.Drawing.Font("UD デジタル 教科書体 NK-R", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelAuthWarning.ForeColor = System.Drawing.Color.Red;
            this.labelAuthWarning.Location = new System.Drawing.Point(64, 26);
            this.labelAuthWarning.Name = "labelAuthWarning";
            this.labelAuthWarning.Size = new System.Drawing.Size(219, 25);
            this.labelAuthWarning.TabIndex = 6;
            this.labelAuthWarning.Text = "指紋認証してください";
            // 
            // WaitingForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(96)))), ((int)(((byte)(96)))));
            this.ClientSize = new System.Drawing.Size(350, 240);
            this.Controls.Add(this.labelAuthWarning);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label17);
            this.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WaitingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WaitingForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.WaitingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelAuthWarning;
    }
}