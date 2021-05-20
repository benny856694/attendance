namespace huaanClient
{
    partial class LoginForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.btnLogin = new ZXCL.WinFormUI.ZButton();
            this.lbStatus = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Language_Selection1 = new ZXCL.WinFormUI.ZComboBox();
            this.tbPassword = new ZXCL.WinFormUI.ZTextBoxEx();
            this.tbusername = new ZXCL.WinFormUI.ZTextBoxEx();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.btnLogin.Location = new System.Drawing.Point(68, 256);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Radius = 10;
            this.btnLogin.Size = new System.Drawing.Size(289, 40);
            this.btnLogin.TabIndex = 25;
            this.btnLogin.Text = "登  录";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lbStatus
            // 
            this.lbStatus.ForeColor = System.Drawing.Color.OrangeRed;
            this.lbStatus.Location = new System.Drawing.Point(4, 305);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(385, 24);
            this.lbStatus.TabIndex = 26;
            // 
            // Language_Selection1
            // 
            this.Language_Selection1.BorderColorHover = System.Drawing.Color.Transparent;
            this.Language_Selection1.Items.AddRange(new object[] {
            "中文",
            "English",
            "日本語"});
            this.Language_Selection1.Location = new System.Drawing.Point(278, 224);
            this.Language_Selection1.Name = "Language_Selection1";
            this.Language_Selection1.SelectedIndex = -1;
            this.Language_Selection1.ShowBorder = false;
            this.Language_Selection1.Size = new System.Drawing.Size(79, 23);
            this.Language_Selection1.TabIndex = 28;
            this.Language_Selection1.Text = "zComboBox1";
            this.Language_Selection1.WaterText = "语言";
            this.Language_Selection1.SelectedIndexChanged += new System.EventHandler(this.zComboBox1_SelectedIndexChanged);
            // 
            // tbPassword
            // 
            this.tbPassword.BackColor = System.Drawing.Color.White;
            this.tbPassword.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbPassword.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPassword.Image = global::huaanClient.Properties.Resources.password;
            this.tbPassword.Location = new System.Drawing.Point(68, 176);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '●';
            this.tbPassword.ShowBorder = false;
            this.tbPassword.ShowClose = true;
            this.tbPassword.Size = new System.Drawing.Size(289, 40);
            this.tbPassword.TabIndex = 24;
            this.tbPassword.Text = "123456";
            this.tbPassword.UseSystemPasswordChar = true;
            this.tbPassword.WaterText = "密码";
            // 
            // tbusername
            // 
            this.tbusername.BackColor = System.Drawing.Color.White;
            this.tbusername.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tbusername.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbusername.Image = global::huaanClient.Properties.Resources.user;
            this.tbusername.Location = new System.Drawing.Point(68, 118);
            this.tbusername.Name = "tbusername";
            this.tbusername.ShowBorder = false;
            this.tbusername.ShowClose = true;
            this.tbusername.Size = new System.Drawing.Size(289, 40);
            this.tbusername.TabIndex = 23;
            this.tbusername.Text = "admin";
            this.tbusername.WaterText = "账号";
            // 
            // pbClose
            // 
            this.pbClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbClose.BackColor = System.Drawing.Color.Transparent;
            this.pbClose.BackgroundImage = global::huaanClient.Properties.Resources.sysbtn_close_normal;
            this.pbClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbClose.Location = new System.Drawing.Point(382, 16);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(44, 38);
            this.pbClose.TabIndex = 22;
            this.pbClose.TabStop = false;
            this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
            this.pbClose.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbClose_MouseDown);
            this.pbClose.MouseEnter += new System.EventHandler(this.pbClose_MouseEnter);
            this.pbClose.MouseLeave += new System.EventHandler(this.pbClose_MouseLeave);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = global::huaanClient.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(68, 30);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(289, 42);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 29;
            this.pictureBox1.TabStop = false;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BackgroundImage = global::huaanClient.Properties.Resources.Loginbackground;
            this.BorderColor = System.Drawing.Color.Transparent;
            this.CaptionColorEnd = System.Drawing.Color.WhiteSmoke;
            this.CaptionColorStart = System.Drawing.Color.WhiteSmoke;
            this.CaptionFont = new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CaptionHeight = 100;
            this.CaptionLeftSpacing = 70;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(429, 344);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Language_Selection1);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbusername);
            this.Controls.Add(this.pbClose);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.Padding = new System.Windows.Forms.Padding(0, 101, 0, 0);
            this.Resizable = false;
            this.ShowIconOnCaptionon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " 人 脸 识 别 系 统";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbClose;
        private ZXCL.WinFormUI.ZTextBoxEx tbusername;
        private ZXCL.WinFormUI.ZTextBoxEx tbPassword;
        private ZXCL.WinFormUI.ZButton btnLogin;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.ToolTip toolTip1;
        private ZXCL.WinFormUI.ZComboBox Language_Selection1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}