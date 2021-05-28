
namespace Dashboard.Controls
{
    partial class CameraUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelBottomCenter = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelTopRight = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelBottomCenter
            // 
            this.labelBottomCenter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelBottomCenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBottomCenter.ForeColor = System.Drawing.Color.White;
            this.labelBottomCenter.Location = new System.Drawing.Point(0, 364);
            this.labelBottomCenter.Name = "labelBottomCenter";
            this.labelBottomCenter.Size = new System.Drawing.Size(514, 33);
            this.labelBottomCenter.TabIndex = 0;
            this.labelBottomCenter.Text = "label1";
            this.labelBottomCenter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(514, 364);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // labelTopRight
            // 
            this.labelTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTopRight.AutoEllipsis = true;
            this.labelTopRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTopRight.ForeColor = System.Drawing.Color.White;
            this.labelTopRight.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelTopRight.Location = new System.Drawing.Point(357, 10);
            this.labelTopRight.Name = "labelTopRight";
            this.labelTopRight.Size = new System.Drawing.Size(144, 24);
            this.labelTopRight.TabIndex = 2;
            this.labelTopRight.Text = "label2";
            this.labelTopRight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CameraUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.labelTopRight);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelBottomCenter);
            this.Name = "CameraUserControl";
            this.Size = new System.Drawing.Size(514, 397);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CameraUserControl_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelBottomCenter;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelTopRight;
    }
}
