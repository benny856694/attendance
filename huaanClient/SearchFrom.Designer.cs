namespace huaanClient
{
    partial class SearchFrom
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchFrom));
            this.panel1 = new System.Windows.Forms.Panel();
            this.DataGridView = new CCWin.SkinControl.SkinDataGridView();
            this.rout = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mac = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Subnetmask = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.platform = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.syatem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ip_textbox = new CCWin.SkinControl.SkinTextBox();
            this.usertextbox = new CCWin.SkinControl.SkinTextBox();
            this.pd_textbox = new CCWin.SkinControl.SkinTextBox();
            this.loginbtn = new ZXCL.WinFormUI.ZButton();
            this.Importbtn = new ZXCL.WinFormUI.ZButton();
            this.Importfacebtn = new ZXCL.WinFormUI.ZButton();
            this.searchbtn = new ZXCL.WinFormUI.ZButton();
            this.skinLabel1 = new CCWin.SkinControl.SkinLabel();
            this.skinLabel2 = new CCWin.SkinControl.SkinLabel();
            this.skinLabel3 = new CCWin.SkinControl.SkinLabel();
            this.networ_address = new CCWin.SkinControl.SkinTextBox();
            this.Subnet_mask = new CCWin.SkinControl.SkinTextBox();
            this.gateway = new CCWin.SkinControl.SkinTextBox();
            this.skinCheckBox1 = new CCWin.SkinControl.SkinCheckBox();
            this.zButton1 = new ZXCL.WinFormUI.ZButton();
            this.FilePath = new CCWin.SkinControl.SkinTextBox();
            this.zButton2 = new ZXCL.WinFormUI.ZButton();
            this.skinCheckBox2 = new CCWin.SkinControl.SkinCheckBox();
            this.skinCheckBox3 = new CCWin.SkinControl.SkinCheckBox();
            this.skinLabel4 = new CCWin.SkinControl.SkinLabel();
            this.zButton3 = new ZXCL.WinFormUI.ZButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DataGridView);
            this.panel1.Location = new System.Drawing.Point(7, 32);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1086, 379);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // DataGridView
            // 
            this.DataGridView.AllowUserToAddRows = false;
            this.DataGridView.AllowUserToDeleteRows = false;
            this.DataGridView.AllowUserToResizeColumns = false;
            this.DataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(246)))), ((int)(((byte)(253)))));
            this.DataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.DataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.RaisedVertical;
            this.DataGridView.ColumnFont = null;
            this.DataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(246)))), ((int)(((byte)(239)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rout,
            this.ip,
            this.mac,
            this.Subnetmask,
            this.platform,
            this.syatem});
            this.DataGridView.ColumnSelectForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(188)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.DataGridView.EnableHeadersVisualStyles = false;
            this.DataGridView.GridColor = System.Drawing.SystemColors.ButtonHighlight;
            this.DataGridView.HeadFont = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DataGridView.HeadSelectBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.DataGridView.HeadSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.DataGridView.Location = new System.Drawing.Point(0, 0);
            this.DataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.DataGridView.Name = "DataGridView";
            this.DataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.DataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.DataGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.DataGridView.RowTemplate.Height = 23;
            this.DataGridView.Size = new System.Drawing.Size(1085, 378);
            this.DataGridView.SkinGridColor = System.Drawing.SystemColors.ButtonHighlight;
            this.DataGridView.TabIndex = 0;
            this.DataGridView.TitleBack = null;
            this.DataGridView.TitleBackColorBegin = System.Drawing.Color.White;
            this.DataGridView.TitleBackColorEnd = System.Drawing.Color.White;
            this.DataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellClick);
            this.DataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellContentClick);
            this.DataGridView.ColumnHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_ColumnHeaderMouseDoubleClick);
            // 
            // rout
            // 
            this.rout.HeaderText = "选择";
            this.rout.Name = "rout";
            this.rout.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ip
            // 
            this.ip.HeaderText = "IP地址";
            this.ip.Name = "ip";
            this.ip.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ip.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ip.Width = 200;
            // 
            // mac
            // 
            this.mac.HeaderText = "MAC地址";
            this.mac.Name = "mac";
            this.mac.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.mac.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.mac.Width = 200;
            // 
            // Subnetmask
            // 
            this.Subnetmask.HeaderText = "子网掩码";
            this.Subnetmask.Name = "Subnetmask";
            this.Subnetmask.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Subnetmask.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Subnetmask.Width = 200;
            // 
            // platform
            // 
            this.platform.HeaderText = "平台";
            this.platform.Name = "platform";
            this.platform.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.platform.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.platform.Width = 170;
            // 
            // syatem
            // 
            this.syatem.HeaderText = "系统";
            this.syatem.Name = "syatem";
            this.syatem.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.syatem.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.syatem.Width = 170;
            // 
            // ip_textbox
            // 
            this.ip_textbox.BackColor = System.Drawing.Color.Transparent;
            this.ip_textbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ip_textbox.DownBack = null;
            this.ip_textbox.Icon = null;
            this.ip_textbox.IconIsButton = false;
            this.ip_textbox.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.ip_textbox.IsPasswordChat = '\0';
            this.ip_textbox.IsSystemPasswordChar = false;
            this.ip_textbox.Lines = new string[0];
            this.ip_textbox.Location = new System.Drawing.Point(22, 416);
            this.ip_textbox.Margin = new System.Windows.Forms.Padding(0);
            this.ip_textbox.MaxLength = 32767;
            this.ip_textbox.MinimumSize = new System.Drawing.Size(28, 28);
            this.ip_textbox.MouseBack = null;
            this.ip_textbox.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.ip_textbox.Multiline = false;
            this.ip_textbox.Name = "ip_textbox";
            this.ip_textbox.NormlBack = null;
            this.ip_textbox.Padding = new System.Windows.Forms.Padding(5);
            this.ip_textbox.ReadOnly = false;
            this.ip_textbox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.ip_textbox.Size = new System.Drawing.Size(184, 28);
            // 
            // 
            // 
            this.ip_textbox.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ip_textbox.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ip_textbox.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.ip_textbox.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.ip_textbox.SkinTxt.Margin = new System.Windows.Forms.Padding(2);
            this.ip_textbox.SkinTxt.Name = "BaseText";
            this.ip_textbox.SkinTxt.Size = new System.Drawing.Size(172, 18);
            this.ip_textbox.SkinTxt.TabIndex = 0;
            this.ip_textbox.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.ip_textbox.SkinTxt.WaterText = "IP地址";
            this.ip_textbox.TabIndex = 4;
            this.ip_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.ip_textbox.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.ip_textbox.WaterText = "IP地址";
            this.ip_textbox.WordWrap = true;
            // 
            // usertextbox
            // 
            this.usertextbox.BackColor = System.Drawing.Color.Transparent;
            this.usertextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.usertextbox.DownBack = null;
            this.usertextbox.Icon = null;
            this.usertextbox.IconIsButton = false;
            this.usertextbox.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.usertextbox.IsPasswordChat = '\0';
            this.usertextbox.IsSystemPasswordChar = false;
            this.usertextbox.Lines = new string[0];
            this.usertextbox.Location = new System.Drawing.Point(218, 416);
            this.usertextbox.Margin = new System.Windows.Forms.Padding(0);
            this.usertextbox.MaxLength = 32767;
            this.usertextbox.MinimumSize = new System.Drawing.Size(28, 28);
            this.usertextbox.MouseBack = null;
            this.usertextbox.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.usertextbox.Multiline = false;
            this.usertextbox.Name = "usertextbox";
            this.usertextbox.NormlBack = null;
            this.usertextbox.Padding = new System.Windows.Forms.Padding(5);
            this.usertextbox.ReadOnly = false;
            this.usertextbox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.usertextbox.Size = new System.Drawing.Size(184, 28);
            // 
            // 
            // 
            this.usertextbox.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.usertextbox.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usertextbox.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.usertextbox.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.usertextbox.SkinTxt.Margin = new System.Windows.Forms.Padding(2);
            this.usertextbox.SkinTxt.Name = "BaseText";
            this.usertextbox.SkinTxt.Size = new System.Drawing.Size(172, 18);
            this.usertextbox.SkinTxt.TabIndex = 0;
            this.usertextbox.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.usertextbox.SkinTxt.WaterText = "用户名";
            this.usertextbox.TabIndex = 5;
            this.usertextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.usertextbox.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.usertextbox.WaterText = "用户名";
            this.usertextbox.WordWrap = true;
            // 
            // pd_textbox
            // 
            this.pd_textbox.BackColor = System.Drawing.Color.Transparent;
            this.pd_textbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pd_textbox.DownBack = null;
            this.pd_textbox.Icon = null;
            this.pd_textbox.IconIsButton = false;
            this.pd_textbox.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.pd_textbox.IsPasswordChat = '*';
            this.pd_textbox.IsSystemPasswordChar = false;
            this.pd_textbox.Lines = new string[0];
            this.pd_textbox.Location = new System.Drawing.Point(416, 416);
            this.pd_textbox.Margin = new System.Windows.Forms.Padding(0);
            this.pd_textbox.MaxLength = 32767;
            this.pd_textbox.MinimumSize = new System.Drawing.Size(28, 28);
            this.pd_textbox.MouseBack = null;
            this.pd_textbox.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.pd_textbox.Multiline = false;
            this.pd_textbox.Name = "pd_textbox";
            this.pd_textbox.NormlBack = null;
            this.pd_textbox.Padding = new System.Windows.Forms.Padding(5);
            this.pd_textbox.ReadOnly = false;
            this.pd_textbox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.pd_textbox.Size = new System.Drawing.Size(184, 28);
            // 
            // 
            // 
            this.pd_textbox.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pd_textbox.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pd_textbox.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.pd_textbox.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.pd_textbox.SkinTxt.Margin = new System.Windows.Forms.Padding(2);
            this.pd_textbox.SkinTxt.Name = "BaseText";
            this.pd_textbox.SkinTxt.PasswordChar = '*';
            this.pd_textbox.SkinTxt.Size = new System.Drawing.Size(172, 18);
            this.pd_textbox.SkinTxt.TabIndex = 0;
            this.pd_textbox.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.pd_textbox.SkinTxt.WaterText = "密码";
            this.pd_textbox.TabIndex = 6;
            this.pd_textbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.pd_textbox.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.pd_textbox.WaterText = "密码";
            this.pd_textbox.WordWrap = true;
            // 
            // loginbtn
            // 
            this.loginbtn.BackColorHover = System.Drawing.Color.Teal;
            this.loginbtn.BackColorMouseDown = System.Drawing.Color.Teal;
            this.loginbtn.BackColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.loginbtn.BorderColorFocus = System.Drawing.Color.Transparent;
            this.loginbtn.BorderColorNormal = System.Drawing.Color.Transparent;
            this.loginbtn.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.loginbtn.ForeColor = System.Drawing.Color.White;
            this.loginbtn.Location = new System.Drawing.Point(22, 458);
            this.loginbtn.Margin = new System.Windows.Forms.Padding(2);
            this.loginbtn.Name = "loginbtn";
            this.loginbtn.Radius = 15;
            this.loginbtn.Size = new System.Drawing.Size(89, 35);
            this.loginbtn.TabIndex = 7;
            this.loginbtn.Text = "登   录";
            this.loginbtn.UseVisualStyleBackColor = true;
            // 
            // Importbtn
            // 
            this.Importbtn.BackColorHover = System.Drawing.Color.Teal;
            this.Importbtn.BackColorMouseDown = System.Drawing.Color.Teal;
            this.Importbtn.BackColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Importbtn.BorderColorFocus = System.Drawing.Color.Transparent;
            this.Importbtn.BorderColorNormal = System.Drawing.Color.Transparent;
            this.Importbtn.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Importbtn.ForeColor = System.Drawing.Color.White;
            this.Importbtn.Location = new System.Drawing.Point(156, 458);
            this.Importbtn.Margin = new System.Windows.Forms.Padding(2);
            this.Importbtn.Name = "Importbtn";
            this.Importbtn.Radius = 15;
            this.Importbtn.Size = new System.Drawing.Size(90, 34);
            this.Importbtn.TabIndex = 8;
            this.Importbtn.Text = "导入参数";
            this.Importbtn.UseVisualStyleBackColor = true;
            // 
            // Importfacebtn
            // 
            this.Importfacebtn.BackColorHover = System.Drawing.Color.Teal;
            this.Importfacebtn.BackColorMouseDown = System.Drawing.Color.Teal;
            this.Importfacebtn.BackColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Importfacebtn.BorderColorFocus = System.Drawing.Color.Transparent;
            this.Importfacebtn.BorderColorNormal = System.Drawing.Color.Transparent;
            this.Importfacebtn.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Importfacebtn.ForeColor = System.Drawing.Color.White;
            this.Importfacebtn.Location = new System.Drawing.Point(296, 460);
            this.Importfacebtn.Margin = new System.Windows.Forms.Padding(2);
            this.Importfacebtn.Name = "Importfacebtn";
            this.Importfacebtn.Radius = 15;
            this.Importfacebtn.Size = new System.Drawing.Size(91, 34);
            this.Importfacebtn.TabIndex = 9;
            this.Importfacebtn.Text = "导入人脸库";
            this.Importfacebtn.UseVisualStyleBackColor = true;
            // 
            // searchbtn
            // 
            this.searchbtn.BackColorHover = System.Drawing.Color.Teal;
            this.searchbtn.BackColorMouseDown = System.Drawing.Color.Teal;
            this.searchbtn.BackColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.searchbtn.BorderColorFocus = System.Drawing.Color.Transparent;
            this.searchbtn.BorderColorNormal = System.Drawing.Color.Transparent;
            this.searchbtn.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.searchbtn.ForeColor = System.Drawing.Color.White;
            this.searchbtn.Location = new System.Drawing.Point(839, 416);
            this.searchbtn.Margin = new System.Windows.Forms.Padding(2);
            this.searchbtn.Name = "searchbtn";
            this.searchbtn.Radius = 15;
            this.searchbtn.Size = new System.Drawing.Size(238, 76);
            this.searchbtn.TabIndex = 10;
            this.searchbtn.Text = "搜   索";
            this.searchbtn.UseVisualStyleBackColor = true;
            this.searchbtn.Click += new System.EventHandler(this.searchbtn_Click);
            // 
            // skinLabel1
            // 
            this.skinLabel1.AutoSize = true;
            this.skinLabel1.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel1.BorderColor = System.Drawing.Color.White;
            this.skinLabel1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel1.Location = new System.Drawing.Point(19, 503);
            this.skinLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.skinLabel1.Name = "skinLabel1";
            this.skinLabel1.Size = new System.Drawing.Size(115, 17);
            this.skinLabel1.TabIndex = 11;
            this.skinLabel1.Text = "IP设置（选中一个）";
            // 
            // skinLabel2
            // 
            this.skinLabel2.AutoSize = true;
            this.skinLabel2.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel2.BorderColor = System.Drawing.Color.White;
            this.skinLabel2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel2.Location = new System.Drawing.Point(604, 503);
            this.skinLabel2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.skinLabel2.Name = "skinLabel2";
            this.skinLabel2.Size = new System.Drawing.Size(128, 17);
            this.skinLabel2.TabIndex = 12;
            this.skinLabel2.Text = "设备升级（选中一个）";
            // 
            // skinLabel3
            // 
            this.skinLabel3.AutoSize = true;
            this.skinLabel3.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel3.BorderColor = System.Drawing.Color.White;
            this.skinLabel3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel3.ForeColor = System.Drawing.Color.Red;
            this.skinLabel3.Location = new System.Drawing.Point(892, 633);
            this.skinLabel3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.skinLabel3.Name = "skinLabel3";
            this.skinLabel3.Size = new System.Drawing.Size(200, 17);
            this.skinLabel3.TabIndex = 13;
            this.skinLabel3.Text = "温馨提示：搜索功能需要关闭防火墙";
            this.skinLabel3.Click += new System.EventHandler(this.skinLabel3_Click);
            // 
            // networ_address
            // 
            this.networ_address.BackColor = System.Drawing.Color.Transparent;
            this.networ_address.DownBack = null;
            this.networ_address.Icon = null;
            this.networ_address.IconIsButton = false;
            this.networ_address.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.networ_address.IsPasswordChat = '\0';
            this.networ_address.IsSystemPasswordChar = false;
            this.networ_address.Lines = new string[0];
            this.networ_address.Location = new System.Drawing.Point(22, 529);
            this.networ_address.Margin = new System.Windows.Forms.Padding(0);
            this.networ_address.MaxLength = 32767;
            this.networ_address.MinimumSize = new System.Drawing.Size(28, 28);
            this.networ_address.MouseBack = null;
            this.networ_address.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.networ_address.Multiline = false;
            this.networ_address.Name = "networ_address";
            this.networ_address.NormlBack = null;
            this.networ_address.Padding = new System.Windows.Forms.Padding(5);
            this.networ_address.ReadOnly = false;
            this.networ_address.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.networ_address.Size = new System.Drawing.Size(366, 28);
            // 
            // 
            // 
            this.networ_address.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.networ_address.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.networ_address.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.networ_address.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.networ_address.SkinTxt.Margin = new System.Windows.Forms.Padding(2);
            this.networ_address.SkinTxt.Name = "BaseText";
            this.networ_address.SkinTxt.Size = new System.Drawing.Size(356, 18);
            this.networ_address.SkinTxt.TabIndex = 0;
            this.networ_address.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.networ_address.SkinTxt.WaterText = "网络地址";
            this.networ_address.TabIndex = 14;
            this.networ_address.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.networ_address.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.networ_address.WaterText = "网络地址";
            this.networ_address.WordWrap = true;
            // 
            // Subnet_mask
            // 
            this.Subnet_mask.BackColor = System.Drawing.Color.Transparent;
            this.Subnet_mask.DownBack = null;
            this.Subnet_mask.Icon = null;
            this.Subnet_mask.IconIsButton = false;
            this.Subnet_mask.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.Subnet_mask.IsPasswordChat = '\0';
            this.Subnet_mask.IsSystemPasswordChar = false;
            this.Subnet_mask.Lines = new string[0];
            this.Subnet_mask.Location = new System.Drawing.Point(22, 563);
            this.Subnet_mask.Margin = new System.Windows.Forms.Padding(0);
            this.Subnet_mask.MaxLength = 32767;
            this.Subnet_mask.MinimumSize = new System.Drawing.Size(28, 28);
            this.Subnet_mask.MouseBack = null;
            this.Subnet_mask.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.Subnet_mask.Multiline = false;
            this.Subnet_mask.Name = "Subnet_mask";
            this.Subnet_mask.NormlBack = null;
            this.Subnet_mask.Padding = new System.Windows.Forms.Padding(5);
            this.Subnet_mask.ReadOnly = false;
            this.Subnet_mask.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.Subnet_mask.Size = new System.Drawing.Size(366, 28);
            // 
            // 
            // 
            this.Subnet_mask.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Subnet_mask.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Subnet_mask.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Subnet_mask.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.Subnet_mask.SkinTxt.Margin = new System.Windows.Forms.Padding(2);
            this.Subnet_mask.SkinTxt.Name = "BaseText";
            this.Subnet_mask.SkinTxt.Size = new System.Drawing.Size(356, 18);
            this.Subnet_mask.SkinTxt.TabIndex = 0;
            this.Subnet_mask.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.Subnet_mask.SkinTxt.WaterText = "子网掩码";
            this.Subnet_mask.TabIndex = 15;
            this.Subnet_mask.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.Subnet_mask.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.Subnet_mask.WaterText = "子网掩码";
            this.Subnet_mask.WordWrap = true;
            // 
            // gateway
            // 
            this.gateway.BackColor = System.Drawing.Color.Transparent;
            this.gateway.DownBack = null;
            this.gateway.Icon = null;
            this.gateway.IconIsButton = false;
            this.gateway.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.gateway.IsPasswordChat = '\0';
            this.gateway.IsSystemPasswordChar = false;
            this.gateway.Lines = new string[0];
            this.gateway.Location = new System.Drawing.Point(22, 596);
            this.gateway.Margin = new System.Windows.Forms.Padding(0);
            this.gateway.MaxLength = 32767;
            this.gateway.MinimumSize = new System.Drawing.Size(28, 20);
            this.gateway.MouseBack = null;
            this.gateway.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.gateway.Multiline = false;
            this.gateway.Name = "gateway";
            this.gateway.NormlBack = null;
            this.gateway.Padding = new System.Windows.Forms.Padding(5);
            this.gateway.ReadOnly = false;
            this.gateway.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gateway.Size = new System.Drawing.Size(366, 28);
            // 
            // 
            // 
            this.gateway.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gateway.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gateway.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.gateway.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.gateway.SkinTxt.Margin = new System.Windows.Forms.Padding(2);
            this.gateway.SkinTxt.Name = "BaseText";
            this.gateway.SkinTxt.Size = new System.Drawing.Size(356, 18);
            this.gateway.SkinTxt.TabIndex = 0;
            this.gateway.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.gateway.SkinTxt.WaterText = "默认网关";
            this.gateway.TabIndex = 15;
            this.gateway.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.gateway.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.gateway.WaterText = "默认网关";
            this.gateway.WordWrap = true;
            // 
            // skinCheckBox1
            // 
            this.skinCheckBox1.AutoSize = true;
            this.skinCheckBox1.BackColor = System.Drawing.Color.Transparent;
            this.skinCheckBox1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinCheckBox1.DownBack = null;
            this.skinCheckBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinCheckBox1.Location = new System.Drawing.Point(404, 524);
            this.skinCheckBox1.Margin = new System.Windows.Forms.Padding(2);
            this.skinCheckBox1.MouseBack = null;
            this.skinCheckBox1.Name = "skinCheckBox1";
            this.skinCheckBox1.NormlBack = null;
            this.skinCheckBox1.SelectedDownBack = null;
            this.skinCheckBox1.SelectedMouseBack = null;
            this.skinCheckBox1.SelectedNormlBack = null;
            this.skinCheckBox1.Size = new System.Drawing.Size(62, 21);
            this.skinCheckBox1.TabIndex = 16;
            this.skinCheckBox1.Text = "统一IP";
            this.skinCheckBox1.UseVisualStyleBackColor = false;
            // 
            // zButton1
            // 
            this.zButton1.BackColorHover = System.Drawing.Color.Teal;
            this.zButton1.BackColorMouseDown = System.Drawing.Color.Teal;
            this.zButton1.BackColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.zButton1.BorderColorFocus = System.Drawing.Color.Transparent;
            this.zButton1.BorderColorNormal = System.Drawing.Color.Transparent;
            this.zButton1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zButton1.ForeColor = System.Drawing.Color.White;
            this.zButton1.Location = new System.Drawing.Point(404, 563);
            this.zButton1.Margin = new System.Windows.Forms.Padding(2);
            this.zButton1.Name = "zButton1";
            this.zButton1.Radius = 15;
            this.zButton1.Size = new System.Drawing.Size(70, 62);
            this.zButton1.TabIndex = 17;
            this.zButton1.Text = "设  置";
            this.zButton1.UseVisualStyleBackColor = true;
            // 
            // FilePath
            // 
            this.FilePath.BackColor = System.Drawing.Color.Transparent;
            this.FilePath.DownBack = null;
            this.FilePath.Icon = null;
            this.FilePath.IconIsButton = false;
            this.FilePath.IconMouseState = CCWin.SkinClass.ControlState.Normal;
            this.FilePath.IsPasswordChat = '\0';
            this.FilePath.IsSystemPasswordChar = false;
            this.FilePath.Lines = new string[0];
            this.FilePath.Location = new System.Drawing.Point(607, 529);
            this.FilePath.Margin = new System.Windows.Forms.Padding(0);
            this.FilePath.MaxLength = 32767;
            this.FilePath.MinimumSize = new System.Drawing.Size(28, 28);
            this.FilePath.MouseBack = null;
            this.FilePath.MouseState = CCWin.SkinClass.ControlState.Normal;
            this.FilePath.Multiline = false;
            this.FilePath.Name = "FilePath";
            this.FilePath.NormlBack = null;
            this.FilePath.Padding = new System.Windows.Forms.Padding(5);
            this.FilePath.ReadOnly = false;
            this.FilePath.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.FilePath.Size = new System.Drawing.Size(366, 28);
            // 
            // 
            // 
            this.FilePath.SkinTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FilePath.SkinTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FilePath.SkinTxt.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.FilePath.SkinTxt.Location = new System.Drawing.Point(5, 5);
            this.FilePath.SkinTxt.Margin = new System.Windows.Forms.Padding(2);
            this.FilePath.SkinTxt.Name = "BaseText";
            this.FilePath.SkinTxt.Size = new System.Drawing.Size(356, 18);
            this.FilePath.SkinTxt.TabIndex = 0;
            this.FilePath.SkinTxt.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.FilePath.SkinTxt.WaterText = "升级文件路径";
            this.FilePath.TabIndex = 18;
            this.FilePath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.FilePath.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.FilePath.WaterText = "升级文件路径";
            this.FilePath.WordWrap = true;
            // 
            // zButton2
            // 
            this.zButton2.BackColorHover = System.Drawing.Color.Teal;
            this.zButton2.BackColorMouseDown = System.Drawing.Color.Teal;
            this.zButton2.BackColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.zButton2.BorderColorFocus = System.Drawing.Color.Transparent;
            this.zButton2.BorderColorNormal = System.Drawing.Color.Transparent;
            this.zButton2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zButton2.ForeColor = System.Drawing.Color.White;
            this.zButton2.Location = new System.Drawing.Point(1006, 563);
            this.zButton2.Margin = new System.Windows.Forms.Padding(2);
            this.zButton2.Name = "zButton2";
            this.zButton2.Radius = 15;
            this.zButton2.Size = new System.Drawing.Size(70, 62);
            this.zButton2.TabIndex = 19;
            this.zButton2.Text = "升  级";
            this.zButton2.UseVisualStyleBackColor = true;
            // 
            // skinCheckBox2
            // 
            this.skinCheckBox2.AutoSize = true;
            this.skinCheckBox2.BackColor = System.Drawing.Color.Transparent;
            this.skinCheckBox2.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinCheckBox2.DownBack = null;
            this.skinCheckBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinCheckBox2.Location = new System.Drawing.Point(607, 575);
            this.skinCheckBox2.Margin = new System.Windows.Forms.Padding(2);
            this.skinCheckBox2.MaximumSize = new System.Drawing.Size(85, 85);
            this.skinCheckBox2.MouseBack = null;
            this.skinCheckBox2.Name = "skinCheckBox2";
            this.skinCheckBox2.NormlBack = null;
            this.skinCheckBox2.SelectedDownBack = null;
            this.skinCheckBox2.SelectedMouseBack = null;
            this.skinCheckBox2.SelectedNormlBack = null;
            this.skinCheckBox2.Size = new System.Drawing.Size(75, 21);
            this.skinCheckBox2.TabIndex = 20;
            this.skinCheckBox2.Text = "检查版本";
            this.skinCheckBox2.UseVisualStyleBackColor = false;
            // 
            // skinCheckBox3
            // 
            this.skinCheckBox3.AutoSize = true;
            this.skinCheckBox3.BackColor = System.Drawing.Color.Transparent;
            this.skinCheckBox3.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinCheckBox3.DownBack = null;
            this.skinCheckBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinCheckBox3.Location = new System.Drawing.Point(607, 600);
            this.skinCheckBox3.Margin = new System.Windows.Forms.Padding(2);
            this.skinCheckBox3.MouseBack = null;
            this.skinCheckBox3.Name = "skinCheckBox3";
            this.skinCheckBox3.NormlBack = null;
            this.skinCheckBox3.SelectedDownBack = null;
            this.skinCheckBox3.SelectedMouseBack = null;
            this.skinCheckBox3.SelectedNormlBack = null;
            this.skinCheckBox3.Size = new System.Drawing.Size(111, 21);
            this.skinCheckBox3.TabIndex = 21;
            this.skinCheckBox3.Text = "升级完成后重启";
            this.skinCheckBox3.UseVisualStyleBackColor = false;
            // 
            // skinLabel4
            // 
            this.skinLabel4.AutoSize = true;
            this.skinLabel4.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel4.BorderColor = System.Drawing.Color.White;
            this.skinLabel4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel4.ForeColor = System.Drawing.Color.Red;
            this.skinLabel4.Location = new System.Drawing.Point(604, 421);
            this.skinLabel4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.skinLabel4.Name = "skinLabel4";
            this.skinLabel4.Size = new System.Drawing.Size(212, 17);
            this.skinLabel4.TabIndex = 22;
            this.skinLabel4.Text = "单击“选择”标题处进行是否全选操作";
            // 
            // zButton3
            // 
            this.zButton3.BackColorHover = System.Drawing.Color.Teal;
            this.zButton3.BackColorMouseDown = System.Drawing.Color.Teal;
            this.zButton3.BackColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.zButton3.BorderColorFocus = System.Drawing.Color.Transparent;
            this.zButton3.BorderColorNormal = System.Drawing.Color.Transparent;
            this.zButton3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zButton3.ForeColor = System.Drawing.Color.White;
            this.zButton3.Location = new System.Drawing.Point(986, 529);
            this.zButton3.Margin = new System.Windows.Forms.Padding(2);
            this.zButton3.Name = "zButton3";
            this.zButton3.Radius = 15;
            this.zButton3.Size = new System.Drawing.Size(91, 28);
            this.zButton3.TabIndex = 23;
            this.zButton3.Text = "选择路径";
            this.zButton3.UseVisualStyleBackColor = true;
            this.zButton3.Click += new System.EventHandler(this.zButton3_Click);
            // 
            // SearchFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BorderColor = System.Drawing.Color.White;
            this.CaptionColorStart = System.Drawing.Color.WhiteSmoke;
            this.CaptionFont = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CaptionHeight = 25;
            this.CaptionLeftSpacing = 1;
            this.ClientSize = new System.Drawing.Size(1100, 650);
            this.Controls.Add(this.zButton3);
            this.Controls.Add(this.skinLabel4);
            this.Controls.Add(this.skinCheckBox3);
            this.Controls.Add(this.skinCheckBox2);
            this.Controls.Add(this.zButton2);
            this.Controls.Add(this.FilePath);
            this.Controls.Add(this.zButton1);
            this.Controls.Add(this.skinCheckBox1);
            this.Controls.Add(this.gateway);
            this.Controls.Add(this.Subnet_mask);
            this.Controls.Add(this.networ_address);
            this.Controls.Add(this.skinLabel3);
            this.Controls.Add(this.skinLabel2);
            this.Controls.Add(this.skinLabel1);
            this.Controls.Add(this.searchbtn);
            this.Controls.Add(this.Importfacebtn);
            this.Controls.Add(this.Importbtn);
            this.Controls.Add(this.loginbtn);
            this.Controls.Add(this.pd_textbox);
            this.Controls.Add(this.usertextbox);
            this.Controls.Add(this.ip_textbox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1100, 650);
            this.MinimumSize = new System.Drawing.Size(1100, 650);
            this.Name = "SearchFrom";
            this.Padding = new System.Windows.Forms.Padding(0, 26, 0, 0);
            this.Resizable = false;
            this.Text = "搜索更多";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.Shown += new System.EventHandler(this.SearchFrom_Shown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private CCWin.SkinControl.SkinTextBox ip_textbox;
        private CCWin.SkinControl.SkinTextBox usertextbox;
        private CCWin.SkinControl.SkinTextBox pd_textbox;
        private ZXCL.WinFormUI.ZButton loginbtn;
        private ZXCL.WinFormUI.ZButton Importbtn;
        private ZXCL.WinFormUI.ZButton Importfacebtn;
        private ZXCL.WinFormUI.ZButton searchbtn;
        private CCWin.SkinControl.SkinLabel skinLabel1;
        private CCWin.SkinControl.SkinLabel skinLabel2;
        private CCWin.SkinControl.SkinLabel skinLabel3;
        private CCWin.SkinControl.SkinTextBox networ_address;
        private CCWin.SkinControl.SkinTextBox Subnet_mask;
        private CCWin.SkinControl.SkinTextBox gateway;
        private CCWin.SkinControl.SkinCheckBox skinCheckBox1;
        private ZXCL.WinFormUI.ZButton zButton1;
        private CCWin.SkinControl.SkinTextBox FilePath;
        private ZXCL.WinFormUI.ZButton zButton2;
        private CCWin.SkinControl.SkinCheckBox skinCheckBox2;
        private CCWin.SkinControl.SkinCheckBox skinCheckBox3;
        private CCWin.SkinControl.SkinDataGridView DataGridView;
        private CCWin.SkinControl.SkinLabel skinLabel4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn rout;
        private System.Windows.Forms.DataGridViewTextBoxColumn ip;
        private System.Windows.Forms.DataGridViewTextBoxColumn mac;
        private System.Windows.Forms.DataGridViewTextBoxColumn Subnetmask;
        private System.Windows.Forms.DataGridViewTextBoxColumn platform;
        private System.Windows.Forms.DataGridViewTextBoxColumn syatem;
        private ZXCL.WinFormUI.ZButton zButton3;
    }
}