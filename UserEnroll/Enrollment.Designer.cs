namespace UserEnroll
{
    partial class Enrollment
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false</param>
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
            this.btnBack = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxRole = new System.Windows.Forms.ComboBox();
            this.buttonEnroll = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(388, 379);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(200, 40);
            this.btnBack.TabIndex = 6;
            this.btnBack.Text = "保存せず閉じる";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 18);
            this.label1.TabIndex = 8;
            this.label1.Text = "名前";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(29, 58);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(269, 26);
            this.textBoxName.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 18);
            this.label2.TabIndex = 10;
            this.label2.Text = "権限";
            // 
            // comboBoxRole
            // 
            this.comboBoxRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRole.FormattingEnabled = true;
            this.comboBoxRole.Items.AddRange(new object[] {
            "特権",
            "営業所",
            "一般",
            "その他"});
            this.comboBoxRole.Location = new System.Drawing.Point(29, 156);
            this.comboBoxRole.Name = "comboBoxRole";
            this.comboBoxRole.Size = new System.Drawing.Size(269, 26);
            this.comboBoxRole.TabIndex = 11;
            // 
            // buttonEnroll
            // 
            this.buttonEnroll.Enabled = false;
            this.buttonEnroll.Location = new System.Drawing.Point(29, 379);
            this.buttonEnroll.Name = "buttonEnroll";
            this.buttonEnroll.Size = new System.Drawing.Size(200, 40);
            this.buttonEnroll.TabIndex = 12;
            this.buttonEnroll.Text = "ユーザ情報登録";
            this.buttonEnroll.UseVisualStyleBackColor = true;
            this.buttonEnroll.Click += new System.EventHandler(this.ButtonEnroll_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(100, 257);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(129, 23);
            this.progressBar1.Step = 25;
            this.progressBar1.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 262);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 18);
            this.label3.TabIndex = 14;
            this.label3.Text = "登録状況";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(97, 306);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 18);
            this.label4.TabIndex = 15;
            this.label4.Text = "登録する指を";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(388, 156);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(200, 96);
            this.richTextBox1.TabIndex = 16;
            this.richTextBox1.Text = "特権ユーザーは、ユーザー登録・編集が可能です。";
            // 
            // Enrollment
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonEnroll);
            this.Controls.Add(this.comboBoxRole);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBack);
            this.Font = new System.Drawing.Font("UD デジタル 教科書体 NK-R", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1024, 768);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(374, 312);
            this.Name = "Enrollment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ユーザ登録";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Enrollment_FormClosed);
            this.Load += new System.EventHandler(this.Enrollment_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxRole;
        private System.Windows.Forms.Button buttonEnroll;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}