﻿namespace huaanClient
{
    partial class Iplist
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Iplabel = new System.Windows.Forms.Label();
            this.btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Iplabel
            // 
            this.Iplabel.AutoSize = true;
            this.Iplabel.Location = new System.Drawing.Point(17, 9);
            this.Iplabel.Name = "Iplabel";
            this.Iplabel.Size = new System.Drawing.Size(17, 12);
            this.Iplabel.TabIndex = 0;
            this.Iplabel.Text = "IP";
            // 
            // btn
            // 
            this.btn.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn.ForeColor = System.Drawing.Color.Blue;
            this.btn.Location = new System.Drawing.Point(112, 4);
            this.btn.Name = "btn";
            this.btn.Size = new System.Drawing.Size(50, 22);
            this.btn.TabIndex = 4;
            this.btn.Text = "切换";
            this.btn.UseVisualStyleBackColor = true;
            // 
            // Iplist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn);
            this.Controls.Add(this.Iplabel);
            this.Name = "Iplist";
            this.Size = new System.Drawing.Size(172, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Iplabel;
        private System.Windows.Forms.Button btn;
    }
}
