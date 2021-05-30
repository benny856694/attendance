
namespace Dashboard
{
    partial class TestForm
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
            this.buttonIncColAndRow = new System.Windows.Forms.Button();
            this.decColAndRow = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.gridControl1 = new Dashboard.Controls.GridControl();
            this.SuspendLayout();
            // 
            // buttonIncColAndRow
            // 
            this.buttonIncColAndRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonIncColAndRow.Location = new System.Drawing.Point(509, 551);
            this.buttonIncColAndRow.Name = "buttonIncColAndRow";
            this.buttonIncColAndRow.Size = new System.Drawing.Size(143, 23);
            this.buttonIncColAndRow.TabIndex = 1;
            this.buttonIncColAndRow.Text = "Col++,Row++";
            this.buttonIncColAndRow.UseVisualStyleBackColor = true;
            this.buttonIncColAndRow.Click += new System.EventHandler(this.buttonIncColAndRow_Click);
            // 
            // decColAndRow
            // 
            this.decColAndRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.decColAndRow.Location = new System.Drawing.Point(332, 551);
            this.decColAndRow.Name = "decColAndRow";
            this.decColAndRow.Size = new System.Drawing.Size(128, 23);
            this.decColAndRow.TabIndex = 2;
            this.decColAndRow.Text = "Col--,Row--";
            this.decColAndRow.UseVisualStyleBackColor = true;
            this.decColAndRow.Click += new System.EventHandler(this.button2_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(332, 551);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(128, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Col--,Row--";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // gridControl1
            // 
            this.gridControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControl1.Location = new System.Drawing.Point(33, 14);
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(953, 466);
            this.gridControl1.TabIndex = 0;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 586);
            this.Controls.Add(this.decColAndRow);
            this.Controls.Add(this.buttonIncColAndRow);
            this.Controls.Add(this.gridControl1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.Load += new System.EventHandler(this.TestForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.GridControl gridControl1;
        private System.Windows.Forms.Button buttonIncColAndRow;
        private System.Windows.Forms.Button decColAndRow;
        private System.Windows.Forms.Button button2;
    }
}