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
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.BackColorHover = System.Drawing.Color.SkyBlue;
            this.btnLogin.BackColorMouseDown = System.Drawing.Color.DodgerBlue;
            this.btnLogin.BackColorNormal = System.Drawing.Color.DeepSkyBlue;
            this.btnLogin.BorderColorFocus = System.Drawing.Color.Transparent;
            this.btnLogin.BorderColorNormal = System.Drawing.Color.Transparent;
            this.btnLogin.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Location = new System.Drawing.Point(212, 423);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Radius = 10;
            this.btnLogin.Size = new System.Drawing.Size(350, 40);
            this.btnLogin.TabIndex = 26;
            this.btnLogin.Text = "拍   照";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // videoSourcePlayer1
            // 
            this.videoSourcePlayer1.Location = new System.Drawing.Point(212, 49);
            this.videoSourcePlayer1.Name = "videoSourcePlayer1";
            this.videoSourcePlayer1.Size = new System.Drawing.Size(350, 350);
            this.videoSourcePlayer1.TabIndex = 27;
            this.videoSourcePlayer1.Text = "videoSourcePlayer1";
            this.videoSourcePlayer1.VideoSource = null;
            // 
            // plView
            // 
            this.plView.Location = new System.Drawing.Point(19, 49);
            this.plView.Name = "plView";
            this.plView.Size = new System.Drawing.Size(187, 350);
            this.plView.TabIndex = 28;
            // 
            // Camera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.White;
            this.CaptionLeftSpacing = 10;
            this.ClientSize = new System.Drawing.Size(577, 492);
            this.Controls.Add(this.plView);
            this.Controls.Add(this.videoSourcePlayer1);
            this.Controls.Add(this.btnLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Camera";
            this.Resizable = false;
            this.ShowIconOnCaptionon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "拍照";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Camera_FormClosing);
            this.Load += new System.EventHandler(this.Camera_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private ZXCL.WinFormUI.ZButton btnLogin;
        private AForge.Controls.VideoSourcePlayer videoSourcePlayer1;
        private System.Windows.Forms.FlowLayoutPanel plView;
    }
}