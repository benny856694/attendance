namespace huaanClient
{
    partial class Camera
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
            this.btnLogin = new ZXCL.WinFormUI.ZButton();
            this.videoSourcePlayer1 = new AForge.Controls.VideoSourcePlayer();
            this.plView = new System.Windows.Forms.FlowLayoutPanel();
            this.skinSplitContainer1 = new CCWin.SkinControl.SkinSplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.skinSplitContainer1)).BeginInit();
            this.skinSplitContainer1.Panel1.SuspendLayout();
            this.skinSplitContainer1.Panel2.SuspendLayout();
            this.skinSplitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogin.BackColorHover = System.Drawing.Color.SkyBlue;
            this.btnLogin.BackColorMouseDown = System.Drawing.Color.DodgerBlue;
            this.btnLogin.BackColorNormal = System.Drawing.Color.DeepSkyBlue;
            this.btnLogin.BorderColorFocus = System.Drawing.Color.Transparent;
            this.btnLogin.BorderColorNormal = System.Drawing.Color.Transparent;
            this.btnLogin.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(283, 532);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Radius = 10;
            this.btnLogin.Size = new System.Drawing.Size(467, 50);
            this.btnLogin.TabIndex = 26;
            this.btnLogin.Text = "拍   照";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // videoSourcePlayer1
            // 
            this.videoSourcePlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoSourcePlayer1.Location = new System.Drawing.Point(0, 0);
            this.videoSourcePlayer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.videoSourcePlayer1.Name = "videoSourcePlayer1";
            this.videoSourcePlayer1.Size = new System.Drawing.Size(482, 457);
            this.videoSourcePlayer1.TabIndex = 27;
            this.videoSourcePlayer1.Text = "videoSourcePlayer1";
            this.videoSourcePlayer1.VideoSource = null;
            // 
            // plView
            // 
            this.plView.AutoScroll = true;
            this.plView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.plView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plView.Location = new System.Drawing.Point(0, 0);
            this.plView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.plView.Name = "plView";
            this.plView.Size = new System.Drawing.Size(243, 457);
            this.plView.TabIndex = 28;
            // 
            // skinSplitContainer1
            // 
            this.skinSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.skinSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.skinSplitContainer1.Location = new System.Drawing.Point(21, 55);
            this.skinSplitContainer1.Name = "skinSplitContainer1";
            // 
            // skinSplitContainer1.Panel1
            // 
            this.skinSplitContainer1.Panel1.Controls.Add(this.plView);
            // 
            // skinSplitContainer1.Panel2
            // 
            this.skinSplitContainer1.Panel2.Controls.Add(this.videoSourcePlayer1);
            this.skinSplitContainer1.Size = new System.Drawing.Size(729, 457);
            this.skinSplitContainer1.SplitterDistance = 243;
            this.skinSplitContainer1.TabIndex = 29;
            // 
            // Camera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BorderColor = System.Drawing.Color.White;
            this.CaptionLeftSpacing = 10;
            this.ClientSize = new System.Drawing.Size(769, 615);
            this.Controls.Add(this.skinSplitContainer1);
            this.Controls.Add(this.btnLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Camera";
            this.Resizable = false;
            this.ShowIconOnCaptionon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "拍照";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Camera_FormClosing);
            this.Load += new System.EventHandler(this.Camera_Load);
            this.skinSplitContainer1.Panel1.ResumeLayout(false);
            this.skinSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.skinSplitContainer1)).EndInit();
            this.skinSplitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private ZXCL.WinFormUI.ZButton btnLogin;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer1;
        private System.Windows.Forms.FlowLayoutPanel plView;
        private CCWin.SkinControl.SkinSplitContainer skinSplitContainer1;
    }
}