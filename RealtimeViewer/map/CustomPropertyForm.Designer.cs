namespace SampleApplication
{
    partial class CustomPropertyForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.pictFillColor = new System.Windows.Forms.PictureBox();
            this.btnFillColor = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.pictLineColor = new System.Windows.Forms.PictureBox();
            this.btnLineColor = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pictTextColor = new System.Windows.Forms.PictureBox();
            this.btnTextColor = new System.Windows.Forms.Button();
            this.pictPreview = new System.Windows.Forms.PictureBox();
            this.btnIconRef = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtLineWidth = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictFillColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictLineColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictTextColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPreview)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "塗りつぶし色";
            // 
            // pictFillColor
            // 
            this.pictFillColor.Location = new System.Drawing.Point(77, 7);
            this.pictFillColor.Name = "pictFillColor";
            this.pictFillColor.Size = new System.Drawing.Size(64, 23);
            this.pictFillColor.TabIndex = 1;
            this.pictFillColor.TabStop = false;
            // 
            // btnFillColor
            // 
            this.btnFillColor.Location = new System.Drawing.Point(147, 7);
            this.btnFillColor.Name = "btnFillColor";
            this.btnFillColor.Size = new System.Drawing.Size(75, 23);
            this.btnFillColor.TabIndex = 2;
            this.btnFillColor.Text = "選択";
            this.btnFillColor.UseVisualStyleBackColor = true;
            this.btnFillColor.Click += new System.EventHandler(this.btnFillColor_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "線色";
            // 
            // pictLineColor
            // 
            this.pictLineColor.Location = new System.Drawing.Point(77, 36);
            this.pictLineColor.Name = "pictLineColor";
            this.pictLineColor.Size = new System.Drawing.Size(64, 23);
            this.pictLineColor.TabIndex = 1;
            this.pictLineColor.TabStop = false;
            // 
            // btnLineColor
            // 
            this.btnLineColor.Location = new System.Drawing.Point(147, 36);
            this.btnLineColor.Name = "btnLineColor";
            this.btnLineColor.Size = new System.Drawing.Size(75, 23);
            this.btnLineColor.TabIndex = 2;
            this.btnLineColor.Text = "選択";
            this.btnLineColor.UseVisualStyleBackColor = true;
            this.btnLineColor.Click += new System.EventHandler(this.btnLineColor_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "線幅";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "文字色";
            // 
            // pictTextColor
            // 
            this.pictTextColor.Location = new System.Drawing.Point(77, 94);
            this.pictTextColor.Name = "pictTextColor";
            this.pictTextColor.Size = new System.Drawing.Size(64, 23);
            this.pictTextColor.TabIndex = 1;
            this.pictTextColor.TabStop = false;
            // 
            // btnTextColor
            // 
            this.btnTextColor.Location = new System.Drawing.Point(147, 94);
            this.btnTextColor.Name = "btnTextColor";
            this.btnTextColor.Size = new System.Drawing.Size(75, 23);
            this.btnTextColor.TabIndex = 2;
            this.btnTextColor.Text = "選択";
            this.btnTextColor.UseVisualStyleBackColor = true;
            this.btnTextColor.Click += new System.EventHandler(this.btnTextColor_Click);
            // 
            // pictPreview
            // 
            this.pictPreview.Location = new System.Drawing.Point(77, 123);
            this.pictPreview.Name = "pictPreview";
            this.pictPreview.Size = new System.Drawing.Size(64, 64);
            this.pictPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictPreview.TabIndex = 1;
            this.pictPreview.TabStop = false;
            // 
            // btnIconRef
            // 
            this.btnIconRef.Location = new System.Drawing.Point(147, 164);
            this.btnIconRef.Name = "btnIconRef";
            this.btnIconRef.Size = new System.Drawing.Size(75, 23);
            this.btnIconRef.TabIndex = 2;
            this.btnIconRef.Text = "参照";
            this.btnIconRef.UseVisualStyleBackColor = true;
            this.btnIconRef.Click += new System.EventHandler(this.btnIconRef_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "アイコン";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(249, 228);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtLineWidth);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.btnIconRef);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnTextColor);
            this.tabPage1.Controls.Add(this.pictFillColor);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.btnLineColor);
            this.tabPage1.Controls.Add(this.pictLineColor);
            this.tabPage1.Controls.Add(this.btnFillColor);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.pictPreview);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.pictTextColor);
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(241, 203);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "カスタム情報";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtLineWidth
            // 
            this.txtLineWidth.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtLineWidth.Location = new System.Drawing.Point(77, 67);
            this.txtLineWidth.MaxLength = 3;
            this.txtLineWidth.Name = "txtLineWidth";
            this.txtLineWidth.Size = new System.Drawing.Size(40, 19);
            this.txtLineWidth.TabIndex = 4;
            this.txtLineWidth.Text = "2";
            this.txtLineWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLineWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLineWidth_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(119, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "px";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(63, 246);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(94, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(163, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CustomPropertyForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(271, 281);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomPropertyForm";
            this.Text = "カスタム情報プロパティ";
            ((System.ComponentModel.ISupportInitialize)(this.pictFillColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictLineColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictTextColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictPreview)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictFillColor;
        private System.Windows.Forms.Button btnFillColor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictLineColor;
        private System.Windows.Forms.Button btnLineColor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictTextColor;
        private System.Windows.Forms.Button btnTextColor;
        private System.Windows.Forms.PictureBox pictPreview;
        private System.Windows.Forms.Button btnIconRef;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtLineWidth;
        private System.Windows.Forms.Label label6;
    }
}