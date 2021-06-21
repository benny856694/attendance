
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
            this.labelTopRight = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBoxLeft = new System.Windows.Forms.PictureBox();
            this.pictureBoxRight = new System.Windows.Forms.PictureBox();
            this.panelPairing = new System.Windows.Forms.Panel();
            this.panelMainImages = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanelPairing = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxPair1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxPair2 = new System.Windows.Forms.PictureBox();
            this.pictureBoxPair3 = new System.Windows.Forms.PictureBox();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            this.panelPairing.SuspendLayout();
            this.panelMainImages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanelPairing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPair1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPair2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPair3)).BeginInit();
            this.SuspendLayout();
            // 
            // labelBottomCenter
            // 
            this.labelBottomCenter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelBottomCenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBottomCenter.ForeColor = System.Drawing.Color.White;
            this.labelBottomCenter.Location = new System.Drawing.Point(2, 361);
            this.labelBottomCenter.Name = "labelBottomCenter";
            this.labelBottomCenter.Size = new System.Drawing.Size(510, 32);
            this.labelBottomCenter.TabIndex = 0;
            this.labelBottomCenter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTopRight
            // 
            this.labelTopRight.AutoEllipsis = true;
            this.labelTopRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTopRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTopRight.ForeColor = System.Drawing.Color.White;
            this.labelTopRight.Location = new System.Drawing.Point(0, 0);
            this.labelTopRight.Name = "labelTopRight";
            this.labelTopRight.Size = new System.Drawing.Size(510, 36);
            this.labelTopRight.TabIndex = 2;
            this.labelTopRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.labelTopRight);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(2, 3);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(510, 36);
            this.panelTop.TabIndex = 3;
            // 
            // pictureBoxLeft
            // 
            this.pictureBoxLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxLeft.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLeft.Name = "pictureBoxLeft";
            this.pictureBoxLeft.Size = new System.Drawing.Size(204, 322);
            this.pictureBoxLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLeft.TabIndex = 0;
            this.pictureBoxLeft.TabStop = false;
            // 
            // pictureBoxRight
            // 
            this.pictureBoxRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxRight.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxRight.Name = "pictureBoxRight";
            this.pictureBoxRight.Size = new System.Drawing.Size(201, 322);
            this.pictureBoxRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxRight.TabIndex = 1;
            this.pictureBoxRight.TabStop = false;
            // 
            // panelPairing
            // 
            this.panelPairing.Controls.Add(this.tableLayoutPanelPairing);
            this.panelPairing.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelPairing.Location = new System.Drawing.Point(408, 39);
            this.panelPairing.Name = "panelPairing";
            this.panelPairing.Size = new System.Drawing.Size(104, 322);
            this.panelPairing.TabIndex = 5;
            // 
            // panelMainImages
            // 
            this.panelMainImages.Controls.Add(this.splitContainer1);
            this.panelMainImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainImages.Location = new System.Drawing.Point(2, 39);
            this.panelMainImages.Name = "panelMainImages";
            this.panelMainImages.Size = new System.Drawing.Size(406, 322);
            this.panelMainImages.TabIndex = 6;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pictureBoxLeft);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pictureBoxRight);
            this.splitContainer1.Size = new System.Drawing.Size(406, 322);
            this.splitContainer1.SplitterDistance = 204;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanelPairing
            // 
            this.tableLayoutPanelPairing.ColumnCount = 1;
            this.tableLayoutPanelPairing.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelPairing.Controls.Add(this.pictureBoxPair3, 0, 2);
            this.tableLayoutPanelPairing.Controls.Add(this.pictureBoxPair2, 0, 1);
            this.tableLayoutPanelPairing.Controls.Add(this.pictureBoxPair1, 0, 0);
            this.tableLayoutPanelPairing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelPairing.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelPairing.Name = "tableLayoutPanelPairing";
            this.tableLayoutPanelPairing.RowCount = 3;
            this.tableLayoutPanelPairing.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelPairing.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelPairing.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanelPairing.Size = new System.Drawing.Size(104, 322);
            this.tableLayoutPanelPairing.TabIndex = 0;
            // 
            // pictureBoxPair1
            // 
            this.pictureBoxPair1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPair1.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxPair1.Name = "pictureBoxPair1";
            this.pictureBoxPair1.Size = new System.Drawing.Size(98, 101);
            this.pictureBoxPair1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPair1.TabIndex = 0;
            this.pictureBoxPair1.TabStop = false;
            // 
            // pictureBoxPair2
            // 
            this.pictureBoxPair2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPair2.Location = new System.Drawing.Point(3, 110);
            this.pictureBoxPair2.Name = "pictureBoxPair2";
            this.pictureBoxPair2.Size = new System.Drawing.Size(98, 101);
            this.pictureBoxPair2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPair2.TabIndex = 1;
            this.pictureBoxPair2.TabStop = false;
            // 
            // pictureBoxPair3
            // 
            this.pictureBoxPair3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPair3.Location = new System.Drawing.Point(3, 217);
            this.pictureBoxPair3.Name = "pictureBoxPair3";
            this.pictureBoxPair3.Size = new System.Drawing.Size(98, 102);
            this.pictureBoxPair3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPair3.TabIndex = 2;
            this.pictureBoxPair3.TabStop = false;
            // 
            // CameraUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelMainImages);
            this.Controls.Add(this.panelPairing);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.labelBottomCenter);
            this.Name = "CameraUserControl";
            this.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Size = new System.Drawing.Size(514, 396);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CameraUserControl_Paint);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            this.panelPairing.ResumeLayout(false);
            this.panelMainImages.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanelPairing.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPair1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPair2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPair3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelBottomCenter;
        private System.Windows.Forms.Label labelTopRight;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox pictureBoxLeft;
        private System.Windows.Forms.PictureBox pictureBoxRight;
        private System.Windows.Forms.Panel panelPairing;
        private System.Windows.Forms.Panel panelMainImages;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPairing;
        private System.Windows.Forms.PictureBox pictureBoxPair3;
        private System.Windows.Forms.PictureBox pictureBoxPair2;
        private System.Windows.Forms.PictureBox pictureBoxPair1;
    }
}
