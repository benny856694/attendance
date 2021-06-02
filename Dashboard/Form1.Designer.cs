
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
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxRow = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1Col = new System.Windows.Forms.ComboBox();
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
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gridControl1);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.listBox1);
            resources.ApplyResources(this.panel3, "panel3");
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
            this.panel2.Controls.Add(this.label1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.comboBoxRow);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comboBox1Col);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // comboBoxRow
            // 
            this.comboBoxRow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRow.FormattingEnabled = true;
            this.comboBoxRow.Items.AddRange(new object[] {
            resources.GetString("comboBoxRow.Items"),
            resources.GetString("comboBoxRow.Items1"),
            resources.GetString("comboBoxRow.Items2"),
            resources.GetString("comboBoxRow.Items3")});
            resources.ApplyResources(this.comboBoxRow, "comboBoxRow");
            this.comboBoxRow.Name = "comboBoxRow";
            this.comboBoxRow.SelectedValueChanged += new System.EventHandler(this.comboBoxRow_SelectedValueChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboBox1Col
            // 
            this.comboBox1Col.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1Col.FormattingEnabled = true;
            this.comboBox1Col.Items.AddRange(new object[] {
            resources.GetString("comboBox1Col.Items"),
            resources.GetString("comboBox1Col.Items1"),
            resources.GetString("comboBox1Col.Items2"),
            resources.GetString("comboBox1Col.Items3")});
            resources.ApplyResources(this.comboBox1Col, "comboBox1Col");
            this.comboBox1Col.Name = "comboBox1Col";
            this.comboBox1Col.SelectedValueChanged += new System.EventHandler(this.comboBox1Col_SelectedValueChanged);
            // 
            // gridControl1
            // 
            this.gridControl1.BackColor = System.Drawing.Color.Black;
            this.gridControl1.Cols = 0;
            resources.ApplyResources(this.gridControl1, "gridControl1");
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Rows = 0;
            this.gridControl1.MouseEnter += new System.EventHandler(this.gridControl1_MouseEnter);
            this.gridControl1.MouseLeave += new System.EventHandler(this.gridControl1_MouseLeave);
            this.gridControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gridControl1_MouseMove);
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
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
        private System.Windows.Forms.ComboBox comboBox1Col;
    }
}

