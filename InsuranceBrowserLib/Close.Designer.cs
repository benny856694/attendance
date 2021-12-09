namespace InsuranceBrowserLib
{
    partial class Close
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
            this.zButton1 = new ZXCL.WinFormUI.ZButton();
            this.zButton2 = new ZXCL.WinFormUI.ZButton();
            this.zCheckBox1 = new ZXCL.WinFormUI.ZCheckBox();
            this.zCheckBox2 = new ZXCL.WinFormUI.ZCheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(134, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "您选择了关闭按钮，您想";
            // 
            // zButton1
            // 
            this.zButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.zButton1.Location = new System.Drawing.Point(144, 173);
            this.zButton1.Name = "zButton1";
            this.zButton1.Radius = 5;
            this.zButton1.Size = new System.Drawing.Size(75, 28);
            this.zButton1.TabIndex = 0;
            this.zButton1.Text = "确定";
            this.zButton1.UseVisualStyleBackColor = true;
            this.zButton1.Click += new System.EventHandler(this.zButton1_Click);
            // 
            // zButton2
            // 
            this.zButton2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.zButton2.Location = new System.Drawing.Point(228, 173);
            this.zButton2.Name = "zButton2";
            this.zButton2.Radius = 5;
            this.zButton2.Size = new System.Drawing.Size(75, 28);
            this.zButton2.TabIndex = 2;
            this.zButton2.Text = "取消";
            this.zButton2.UseVisualStyleBackColor = true;
            this.zButton2.Click += new System.EventHandler(this.zButton2_Click);
            // 
            // zCheckBox1
            // 
            this.zCheckBox1.AutoSize = true;
            this.zCheckBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zCheckBox1.Location = new System.Drawing.Point(138, 84);
            this.zCheckBox1.Name = "zCheckBox1";
            this.zCheckBox1.ShowInnerBox = false;
            this.zCheckBox1.Size = new System.Drawing.Size(207, 21);
            this.zCheckBox1.TabIndex = 3;
            this.zCheckBox1.Text = "最小化到系统托盘区，不退出程序";
            this.zCheckBox1.UseVisualStyleBackColor = true;
            this.zCheckBox1.CheckedChanged += new System.EventHandler(this.zCheckBox1_CheckedChanged);
            // 
            // zCheckBox2
            // 
            this.zCheckBox2.AutoSize = true;
            this.zCheckBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zCheckBox2.Location = new System.Drawing.Point(138, 111);
            this.zCheckBox2.Name = "zCheckBox2";
            this.zCheckBox2.ShowInnerBox = false;
            this.zCheckBox2.Size = new System.Drawing.Size(75, 21);
            this.zCheckBox2.TabIndex = 4;
            this.zCheckBox2.Text = "退出程序";
            this.zCheckBox2.UseVisualStyleBackColor = true;
            this.zCheckBox2.CheckedChanged += new System.EventHandler(this.zCheckBox2_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::InsuranceBrowserLib.Properties.Resources.question;
            this.pictureBox1.Location = new System.Drawing.Point(28, 56);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(75, 75);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // Close
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.Transparent;
            this.CaptionHeight = 40;
            this.ClientSize = new System.Drawing.Size(441, 214);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.zCheckBox2);
            this.Controls.Add(this.zCheckBox1);
            this.Controls.Add(this.zButton2);
            this.Controls.Add(this.zButton1);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Close";
            this.Padding = new System.Windows.Forms.Padding(0, 41, 0, 0);
            this.Resizable = false;
            this.ShowIcon = false;
            this.ShowIconOnCaptionon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "  温馨提示";
            this.Activated += new System.EventHandler(this.Close_Activated);
            this.Load += new System.EventHandler(this.Close_Load);
            this.Shown += new System.EventHandler(this.Close_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private ZXCL.WinFormUI.ZButton zButton1;
        private ZXCL.WinFormUI.ZButton zButton2;
        private ZXCL.WinFormUI.ZCheckBox zCheckBox1;
        private ZXCL.WinFormUI.ZCheckBox zCheckBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}