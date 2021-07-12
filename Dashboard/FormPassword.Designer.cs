
namespace Dashboard
{
    partial class FormPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPassword));
            Bunifu.UI.WinForms.BunifuTextBox.StateProperties stateProperties5 = new Bunifu.UI.WinForms.BunifuTextBox.StateProperties();
            Bunifu.UI.WinForms.BunifuTextBox.StateProperties stateProperties6 = new Bunifu.UI.WinForms.BunifuTextBox.StateProperties();
            Bunifu.UI.WinForms.BunifuTextBox.StateProperties stateProperties7 = new Bunifu.UI.WinForms.BunifuTextBox.StateProperties();
            Bunifu.UI.WinForms.BunifuTextBox.StateProperties stateProperties8 = new Bunifu.UI.WinForms.BunifuTextBox.StateProperties();
            this.label1 = new System.Windows.Forms.Label();
            this.bunifuTextBoxPassword = new Bunifu.UI.WinForms.BunifuTextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // bunifuTextBoxPassword
            // 
            this.bunifuTextBoxPassword.AcceptsReturn = false;
            this.bunifuTextBoxPassword.AcceptsTab = false;
            resources.ApplyResources(this.bunifuTextBoxPassword, "bunifuTextBoxPassword");
            this.bunifuTextBoxPassword.AnimationSpeed = 200;
            this.bunifuTextBoxPassword.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.None;
            this.bunifuTextBoxPassword.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None;
            this.bunifuTextBoxPassword.BackColor = System.Drawing.Color.Transparent;
            this.bunifuTextBoxPassword.BorderColorActive = System.Drawing.Color.DodgerBlue;
            this.bunifuTextBoxPassword.BorderColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.bunifuTextBoxPassword.BorderColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(181)))), ((int)(((byte)(255)))));
            this.bunifuTextBoxPassword.BorderColorIdle = System.Drawing.Color.Silver;
            this.bunifuTextBoxPassword.BorderRadius = 1;
            this.bunifuTextBoxPassword.BorderThickness = 1;
            this.bunifuTextBoxPassword.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.bunifuTextBoxPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.bunifuTextBoxPassword.DefaultFont = new System.Drawing.Font("Segoe UI", 9.25F);
            this.bunifuTextBoxPassword.DefaultText = "";
            this.bunifuTextBoxPassword.FillColor = System.Drawing.Color.White;
            this.bunifuTextBoxPassword.HideSelection = true;
            this.bunifuTextBoxPassword.IconLeft = null;
            this.bunifuTextBoxPassword.IconLeftCursor = System.Windows.Forms.Cursors.IBeam;
            this.bunifuTextBoxPassword.IconPadding = 10;
            this.bunifuTextBoxPassword.IconRight = null;
            this.bunifuTextBoxPassword.IconRightCursor = System.Windows.Forms.Cursors.IBeam;
            this.bunifuTextBoxPassword.Lines = new string[0];
            this.bunifuTextBoxPassword.MaxLength = 32767;
            this.bunifuTextBoxPassword.Modified = false;
            this.bunifuTextBoxPassword.Multiline = false;
            this.bunifuTextBoxPassword.Name = "bunifuTextBoxPassword";
            stateProperties5.BorderColor = System.Drawing.Color.DodgerBlue;
            stateProperties5.FillColor = System.Drawing.Color.Empty;
            stateProperties5.ForeColor = System.Drawing.Color.Empty;
            stateProperties5.PlaceholderForeColor = System.Drawing.Color.Empty;
            this.bunifuTextBoxPassword.OnActiveState = stateProperties5;
            stateProperties6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            stateProperties6.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            stateProperties6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            stateProperties6.PlaceholderForeColor = System.Drawing.Color.DarkGray;
            this.bunifuTextBoxPassword.OnDisabledState = stateProperties6;
            stateProperties7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(181)))), ((int)(((byte)(255)))));
            stateProperties7.FillColor = System.Drawing.Color.Empty;
            stateProperties7.ForeColor = System.Drawing.Color.Empty;
            stateProperties7.PlaceholderForeColor = System.Drawing.Color.Empty;
            this.bunifuTextBoxPassword.OnHoverState = stateProperties7;
            stateProperties8.BorderColor = System.Drawing.Color.Silver;
            stateProperties8.FillColor = System.Drawing.Color.White;
            stateProperties8.ForeColor = System.Drawing.Color.Empty;
            stateProperties8.PlaceholderForeColor = System.Drawing.Color.Empty;
            this.bunifuTextBoxPassword.OnIdleState = stateProperties8;
            this.bunifuTextBoxPassword.PasswordChar = '●';
            this.bunifuTextBoxPassword.PlaceholderForeColor = System.Drawing.Color.Silver;
            this.bunifuTextBoxPassword.PlaceholderText = "输入密码";
            this.bunifuTextBoxPassword.ReadOnly = false;
            this.bunifuTextBoxPassword.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.bunifuTextBoxPassword.SelectedText = "";
            this.bunifuTextBoxPassword.SelectionLength = 0;
            this.bunifuTextBoxPassword.SelectionStart = 0;
            this.bunifuTextBoxPassword.ShortcutsEnabled = true;
            this.bunifuTextBoxPassword.Style = Bunifu.UI.WinForms.BunifuTextBox._Style.Bunifu;
            this.bunifuTextBoxPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.bunifuTextBoxPassword.TextMarginBottom = 0;
            this.bunifuTextBoxPassword.TextMarginLeft = 3;
            this.bunifuTextBoxPassword.TextMarginTop = 0;
            this.bunifuTextBoxPassword.TextPlaceholder = "输入密码";
            this.bunifuTextBoxPassword.UseSystemPasswordChar = true;
            this.bunifuTextBoxPassword.WordWrap = true;
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormPassword
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.bunifuTextBoxPassword);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPassword";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Bunifu.UI.WinForms.BunifuTextBox bunifuTextBoxPassword;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button button1;
    }
}