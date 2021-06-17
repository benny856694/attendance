
namespace Dashboard
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bunifuImageSyncFace = new Bunifu.UI.WinForms.BunifuImageButton();
            this.bunifuImageButtonOptions = new Bunifu.UI.WinForms.BunifuImageButton();
            this.bunifuImageButton1 = new Bunifu.UI.WinForms.BunifuImageButton();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxRow = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxCol = new System.Windows.Forms.ComboBox();
            this.gridControl1 = new Dashboard.Controls.GridControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.Controls.Add(this.gridControl1);
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.listBox1);
            this.panel3.Name = "panel3";
            // 
            // listBox1
            // 
            resources.ApplyResources(this.listBox1, "listBox1");
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Name = "listBox1";
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.bunifuImageSyncFace);
            this.panel2.Controls.Add(this.bunifuImageButtonOptions);
            this.panel2.Controls.Add(this.bunifuImageButton1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Name = "panel2";
            // 
            // bunifuImageSyncFace
            // 
            resources.ApplyResources(this.bunifuImageSyncFace, "bunifuImageSyncFace");
            this.bunifuImageSyncFace.ActiveImage = null;
            this.bunifuImageSyncFace.AllowAnimations = true;
            this.bunifuImageSyncFace.AllowBuffering = false;
            this.bunifuImageSyncFace.AllowToggling = false;
            this.bunifuImageSyncFace.AllowZooming = false;
            this.bunifuImageSyncFace.AllowZoomingOnFocus = false;
            this.bunifuImageSyncFace.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageSyncFace.DialogResult = System.Windows.Forms.DialogResult.None;
            this.bunifuImageSyncFace.ErrorImage = ((System.Drawing.Image)(resources.GetObject("bunifuImageSyncFace.ErrorImage")));
            this.bunifuImageSyncFace.FadeWhenInactive = false;
            this.bunifuImageSyncFace.Flip = Bunifu.UI.WinForms.BunifuImageButton.FlipOrientation.Normal;
            this.bunifuImageSyncFace.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageSyncFace.Image")));
            this.bunifuImageSyncFace.ImageActive = null;
            this.bunifuImageSyncFace.ImageLocation = null;
            this.bunifuImageSyncFace.ImageMargin = 0;
            this.bunifuImageSyncFace.ImageSize = new System.Drawing.Size(24, 24);
            this.bunifuImageSyncFace.ImageZoomSize = new System.Drawing.Size(24, 24);
            this.bunifuImageSyncFace.InitialImage = ((System.Drawing.Image)(resources.GetObject("bunifuImageSyncFace.InitialImage")));
            this.bunifuImageSyncFace.Name = "bunifuImageSyncFace";
            this.bunifuImageSyncFace.Rotation = 0;
            this.bunifuImageSyncFace.ShowActiveImage = true;
            this.bunifuImageSyncFace.ShowCursorChanges = true;
            this.bunifuImageSyncFace.ShowImageBorders = true;
            this.bunifuImageSyncFace.ShowSizeMarkers = false;
            this.bunifuImageSyncFace.ToolTipText = "";
            this.bunifuImageSyncFace.WaitOnLoad = false;
            this.bunifuImageSyncFace.Zoom = 0;
            this.bunifuImageSyncFace.ZoomSpeed = 10;
            this.bunifuImageSyncFace.Click += new System.EventHandler(this.bunifuImageSyncFace_Click);
            // 
            // bunifuImageButtonOptions
            // 
            resources.ApplyResources(this.bunifuImageButtonOptions, "bunifuImageButtonOptions");
            this.bunifuImageButtonOptions.ActiveImage = null;
            this.bunifuImageButtonOptions.AllowAnimations = true;
            this.bunifuImageButtonOptions.AllowBuffering = false;
            this.bunifuImageButtonOptions.AllowToggling = false;
            this.bunifuImageButtonOptions.AllowZooming = false;
            this.bunifuImageButtonOptions.AllowZoomingOnFocus = false;
            this.bunifuImageButtonOptions.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButtonOptions.DialogResult = System.Windows.Forms.DialogResult.None;
            this.bunifuImageButtonOptions.ErrorImage = ((System.Drawing.Image)(resources.GetObject("bunifuImageButtonOptions.ErrorImage")));
            this.bunifuImageButtonOptions.FadeWhenInactive = false;
            this.bunifuImageButtonOptions.Flip = Bunifu.UI.WinForms.BunifuImageButton.FlipOrientation.Normal;
            this.bunifuImageButtonOptions.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButtonOptions.Image")));
            this.bunifuImageButtonOptions.ImageActive = null;
            this.bunifuImageButtonOptions.ImageLocation = null;
            this.bunifuImageButtonOptions.ImageMargin = 0;
            this.bunifuImageButtonOptions.ImageSize = new System.Drawing.Size(24, 24);
            this.bunifuImageButtonOptions.ImageZoomSize = new System.Drawing.Size(24, 24);
            this.bunifuImageButtonOptions.InitialImage = ((System.Drawing.Image)(resources.GetObject("bunifuImageButtonOptions.InitialImage")));
            this.bunifuImageButtonOptions.Name = "bunifuImageButtonOptions";
            this.bunifuImageButtonOptions.Rotation = 0;
            this.bunifuImageButtonOptions.ShowActiveImage = true;
            this.bunifuImageButtonOptions.ShowCursorChanges = true;
            this.bunifuImageButtonOptions.ShowImageBorders = true;
            this.bunifuImageButtonOptions.ShowSizeMarkers = false;
            this.bunifuImageButtonOptions.ToolTipText = "";
            this.bunifuImageButtonOptions.WaitOnLoad = false;
            this.bunifuImageButtonOptions.Zoom = 0;
            this.bunifuImageButtonOptions.ZoomSpeed = 10;
            this.bunifuImageButtonOptions.Click += new System.EventHandler(this.bunifuImageButtonOptions_Click);
            // 
            // bunifuImageButton1
            // 
            resources.ApplyResources(this.bunifuImageButton1, "bunifuImageButton1");
            this.bunifuImageButton1.ActiveImage = null;
            this.bunifuImageButton1.AllowAnimations = true;
            this.bunifuImageButton1.AllowBuffering = false;
            this.bunifuImageButton1.AllowToggling = false;
            this.bunifuImageButton1.AllowZooming = false;
            this.bunifuImageButton1.AllowZoomingOnFocus = false;
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.bunifuImageButton1.ErrorImage = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.ErrorImage")));
            this.bunifuImageButton1.FadeWhenInactive = false;
            this.bunifuImageButton1.Flip = Bunifu.UI.WinForms.BunifuImageButton.FlipOrientation.Normal;
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.ImageLocation = null;
            this.bunifuImageButton1.ImageMargin = 0;
            this.bunifuImageButton1.ImageSize = new System.Drawing.Size(24, 24);
            this.bunifuImageButton1.ImageZoomSize = new System.Drawing.Size(24, 24);
            this.bunifuImageButton1.InitialImage = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.InitialImage")));
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Rotation = 0;
            this.bunifuImageButton1.ShowActiveImage = true;
            this.bunifuImageButton1.ShowCursorChanges = true;
            this.bunifuImageButton1.ShowImageBorders = true;
            this.bunifuImageButton1.ShowSizeMarkers = false;
            this.bunifuImageButton1.ToolTipText = "";
            this.bunifuImageButton1.WaitOnLoad = false;
            this.bunifuImageButton1.Zoom = 0;
            this.bunifuImageButton1.ZoomSpeed = 10;
            this.bunifuImageButton1.Click += new System.EventHandler(this.buttonSearchDevice_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.comboBoxRow);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comboBoxCol);
            this.panel1.Name = "panel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBoxRow
            // 
            resources.ApplyResources(this.comboBoxRow, "comboBoxRow");
            this.comboBoxRow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRow.FormattingEnabled = true;
            this.comboBoxRow.Name = "comboBoxRow";
            this.comboBoxRow.SelectedValueChanged += new System.EventHandler(this.comboBoxRow_SelectedValueChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboBoxCol
            // 
            resources.ApplyResources(this.comboBoxCol, "comboBoxCol");
            this.comboBoxCol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCol.FormattingEnabled = true;
            this.comboBoxCol.Name = "comboBoxCol";
            this.comboBoxCol.SelectedValueChanged += new System.EventHandler(this.comboBox1Col_SelectedValueChanged);
            // 
            // gridControl1
            // 
            resources.ApplyResources(this.gridControl1, "gridControl1");
            this.gridControl1.BackColor = System.Drawing.Color.Black;
            this.gridControl1.Cols = 0;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Rows = 0;
            this.gridControl1.MouseEnter += new System.EventHandler(this.gridControl1_MouseEnter);
            this.gridControl1.MouseLeave += new System.EventHandler(this.gridControl1_MouseLeave);
            this.gridControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridControl1_MouseMove);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private Controls.GridControl gridControl1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxRow;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxCol;
        private Bunifu.UI.WinForms.BunifuImageButton bunifuImageButton1;
        private Bunifu.UI.WinForms.BunifuImageButton bunifuImageButtonOptions;
        private Bunifu.UI.WinForms.BunifuImageButton bunifuImageSyncFace;
    }
}

