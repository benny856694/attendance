namespace ZXCL.WinFormUI
{
    partial class ZForm
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
            this.Tip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // Tip
            // 
            this.Tip.AutomaticDelay = 1000;
            this.Tip.AutoPopDelay = 1000;
            this.Tip.InitialDelay = 1000;
            this.Tip.ReshowDelay = 200;
            // 
            // ZForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "ZForm";
            this.Padding = new System.Windows.Forms.Padding(0, 31, 0, 0);
            this.Text = "ZForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip Tip;
    }
}