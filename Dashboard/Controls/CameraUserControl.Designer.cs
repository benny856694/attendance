
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
            Utilities.BunifuPages.BunifuAnimatorNS.Animation animation1 = new Utilities.BunifuPages.BunifuAnimatorNS.Animation();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraUserControl));
            this.labelBottomCenter = new System.Windows.Forms.Label();
            this.pictureBoxSingle = new System.Windows.Forms.PictureBox();
            this.labelTopRight = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bunifuPagesImageContainer = new Bunifu.UI.WinForms.BunifuPages();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxLeft = new System.Windows.Forms.PictureBox();
            this.pictureBoxRight = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSingle)).BeginInit();
            this.panel1.SuspendLayout();
            this.bunifuPagesImageContainer.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            this.SuspendLayout();
            // 
            // labelBottomCenter
            // 
            this.labelBottomCenter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelBottomCenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBottomCenter.ForeColor = System.Drawing.Color.White;
            this.labelBottomCenter.Location = new System.Drawing.Point(2, 333);
            this.labelBottomCenter.Name = "labelBottomCenter";
            this.labelBottomCenter.Size = new System.Drawing.Size(510, 30);
            this.labelBottomCenter.TabIndex = 0;
            this.labelBottomCenter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxSingle
            // 
            this.pictureBoxSingle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxSingle.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxSingle.Name = "pictureBoxSingle";
            this.pictureBoxSingle.Size = new System.Drawing.Size(496, 265);
            this.pictureBoxSingle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxSingle.TabIndex = 1;
            this.pictureBoxSingle.TabStop = false;
            // 
            // labelTopRight
            // 
            this.labelTopRight.AutoEllipsis = true;
            this.labelTopRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTopRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTopRight.ForeColor = System.Drawing.Color.White;
            this.labelTopRight.Location = new System.Drawing.Point(0, 0);
            this.labelTopRight.Name = "labelTopRight";
            this.labelTopRight.Size = new System.Drawing.Size(510, 33);
            this.labelTopRight.TabIndex = 2;
            this.labelTopRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelTopRight);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(510, 33);
            this.panel1.TabIndex = 3;
            // 
            // bunifuPagesImageContainer
            // 
            this.bunifuPagesImageContainer.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.bunifuPagesImageContainer.AllowTransitions = false;
            this.bunifuPagesImageContainer.Controls.Add(this.tabPage1);
            this.bunifuPagesImageContainer.Controls.Add(this.tabPage2);
            this.bunifuPagesImageContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bunifuPagesImageContainer.Location = new System.Drawing.Point(2, 36);
            this.bunifuPagesImageContainer.Multiline = true;
            this.bunifuPagesImageContainer.Name = "bunifuPagesImageContainer";
            this.bunifuPagesImageContainer.Page = this.tabPage2;
            this.bunifuPagesImageContainer.PageIndex = 1;
            this.bunifuPagesImageContainer.PageName = "tabPage2";
            this.bunifuPagesImageContainer.PageTitle = "tabPage2";
            this.bunifuPagesImageContainer.SelectedIndex = 0;
            this.bunifuPagesImageContainer.Size = new System.Drawing.Size(510, 297);
            this.bunifuPagesImageContainer.TabIndex = 4;
            animation1.AnimateOnlyDifferences = false;
            animation1.BlindCoeff = ((System.Drawing.PointF)(resources.GetObject("animation1.BlindCoeff")));
            animation1.LeafCoeff = 0F;
            animation1.MaxTime = 1F;
            animation1.MinTime = 0F;
            animation1.MosaicCoeff = ((System.Drawing.PointF)(resources.GetObject("animation1.MosaicCoeff")));
            animation1.MosaicShift = ((System.Drawing.PointF)(resources.GetObject("animation1.MosaicShift")));
            animation1.MosaicSize = 0;
            animation1.Padding = new System.Windows.Forms.Padding(0);
            animation1.RotateCoeff = 0F;
            animation1.RotateLimit = 0F;
            animation1.ScaleCoeff = ((System.Drawing.PointF)(resources.GetObject("animation1.ScaleCoeff")));
            animation1.SlideCoeff = ((System.Drawing.PointF)(resources.GetObject("animation1.SlideCoeff")));
            animation1.TimeCoeff = 0F;
            animation1.TransparencyCoeff = 0F;
            this.bunifuPagesImageContainer.Transition = animation1;
            this.bunifuPagesImageContainer.TransitionType = Utilities.BunifuPages.BunifuAnimatorNS.AnimationType.Custom;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Black;
            this.tabPage1.Controls.Add(this.pictureBoxSingle);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(502, 271);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Black;
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(502, 271);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxLeft, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxRight, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(496, 265);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pictureBoxLeft
            // 
            this.pictureBoxLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxLeft.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxLeft.Name = "pictureBoxLeft";
            this.pictureBoxLeft.Size = new System.Drawing.Size(242, 259);
            this.pictureBoxLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLeft.TabIndex = 0;
            this.pictureBoxLeft.TabStop = false;
            // 
            // pictureBoxRight
            // 
            this.pictureBoxRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxRight.Location = new System.Drawing.Point(251, 3);
            this.pictureBoxRight.Name = "pictureBoxRight";
            this.pictureBoxRight.Size = new System.Drawing.Size(242, 259);
            this.pictureBoxRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxRight.TabIndex = 1;
            this.pictureBoxRight.TabStop = false;
            // 
            // CameraUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.bunifuPagesImageContainer);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelBottomCenter);
            this.Name = "CameraUserControl";
            this.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Size = new System.Drawing.Size(514, 366);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CameraUserControl_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSingle)).EndInit();
            this.panel1.ResumeLayout(false);
            this.bunifuPagesImageContainer.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelBottomCenter;
        private System.Windows.Forms.PictureBox pictureBoxSingle;
        private System.Windows.Forms.Label labelTopRight;
        private System.Windows.Forms.Panel panel1;
        private Bunifu.UI.WinForms.BunifuPages bunifuPagesImageContainer;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBoxLeft;
        private System.Windows.Forms.PictureBox pictureBoxRight;
    }
}
