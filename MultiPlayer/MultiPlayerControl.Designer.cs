namespace VideoHelper
{
    partial class MultiPlayerControl
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_cam_rotate = new System.Windows.Forms.PictureBox();
            this.btn_fullscreen = new System.Windows.Forms.PictureBox();
            this.btn_disconnect = new System.Windows.Forms.PictureBox();
            this.btn_connect = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cam_rotate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_fullscreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_disconnect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_connect)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_cam_rotate);
            this.panel1.Controls.Add(this.btn_fullscreen);
            this.panel1.Controls.Add(this.btn_disconnect);
            this.panel1.Controls.Add(this.btn_connect);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 208);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(269, 30);
            this.panel1.TabIndex = 1;
            this.panel1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MultiPlayerControl_MouseClick);
            // 
            // btn_cam_rotate
            // 
            this.btn_cam_rotate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_cam_rotate.Location = new System.Drawing.Point(69, 0);
            this.btn_cam_rotate.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btn_cam_rotate.Name = "btn_cam_rotate";
            this.btn_cam_rotate.Size = new System.Drawing.Size(30, 30);
            this.btn_cam_rotate.TabIndex = 4;
            this.btn_cam_rotate.TabStop = false;
            this.toolTip1.SetToolTip(this.btn_cam_rotate, "云台控制");
            this.btn_cam_rotate.Visible = false;
            this.btn_cam_rotate.Click += new System.EventHandler(this.btn_cam_rotate_Click);
            // 
            // btn_fullscreen
            // 
            this.btn_fullscreen.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_fullscreen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_fullscreen.Location = new System.Drawing.Point(236, 0);
            this.btn_fullscreen.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btn_fullscreen.Name = "btn_fullscreen";
            this.btn_fullscreen.Size = new System.Drawing.Size(30, 30);
            this.btn_fullscreen.TabIndex = 3;
            this.btn_fullscreen.TabStop = false;
            this.toolTip1.SetToolTip(this.btn_fullscreen, "全屏播放");
            this.btn_fullscreen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btn_fullscreen_MouseClick);
            this.btn_fullscreen.MouseEnter += new System.EventHandler(this.btn_fullscreen_MouseEnter);
            this.btn_fullscreen.MouseLeave += new System.EventHandler(this.btn_fullscreen_MouseLeave);
            // 
            // btn_disconnect
            // 
            this.btn_disconnect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_disconnect.Location = new System.Drawing.Point(36, 0);
            this.btn_disconnect.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btn_disconnect.Name = "btn_disconnect";
            this.btn_disconnect.Size = new System.Drawing.Size(30, 30);
            this.btn_disconnect.TabIndex = 2;
            this.btn_disconnect.TabStop = false;
            this.toolTip1.SetToolTip(this.btn_disconnect, "切换模式");
            this.btn_disconnect.Visible = false;
            this.btn_disconnect.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btn_disconnect_MouseClick);
            this.btn_disconnect.MouseEnter += new System.EventHandler(this.btn_disconnect_MouseEnter);
            this.btn_disconnect.MouseLeave += new System.EventHandler(this.btn_disconnect_MouseLeave);
            // 
            // btn_connect
            // 
            this.btn_connect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_connect.Location = new System.Drawing.Point(3, 0);
            this.btn_connect.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(30, 30);
            this.btn_connect.TabIndex = 1;
            this.btn_connect.TabStop = false;
            this.toolTip1.SetToolTip(this.btn_connect, "播放视频");
            this.btn_connect.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btn_connect_MouseClick);
            this.btn_connect.MouseEnter += new System.EventHandler(this.btn_connect_MouseEnter);
            this.btn_connect.MouseLeave += new System.EventHandler(this.btn_connect_MouseLeave);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(269, 30);
            this.panel2.TabIndex = 2;
            this.panel2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MultiPlayerControl_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(152)))), ((int)(((byte)(152)))));
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "未链接";
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 30);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(269, 178);
            this.panel3.TabIndex = 3;
            this.panel3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MultiPlayerControl_MouseClick);
            this.panel3.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panel3_MouseDoubleClick);
            // 
            // MultiPlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "MultiPlayerControl";
            this.Size = new System.Drawing.Size(269, 238);
            this.Load += new System.EventHandler(this.MultiPlayerControl_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MultiPlayerControl_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MultiPlayerControl_MouseClick);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btn_cam_rotate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_fullscreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_disconnect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_connect)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox btn_connect;
        private System.Windows.Forms.PictureBox btn_fullscreen;
        private System.Windows.Forms.PictureBox btn_disconnect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox btn_cam_rotate;
        //private AxAXVLC.AxVLCPlugin2 axVLCPlugin21;
    }
}
