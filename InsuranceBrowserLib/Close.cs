using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXCL.WinFormUI;

namespace InsuranceBrowserLib
{
    public partial class Close : ZForm
    {
        string language;
        public Close(string language)
        {
            this.language = language;
            InitializeComponent();
        }

        private void Close_Load(object sender, EventArgs e)
        {
            //根据不同语言设置 文字
            if (language.Contains("US"))
            {
                this.Text = " Reminder";
                label1.Text = "You have selected the close button and you want to";
                zCheckBox1.Text = "Minimize to the system tray area, do not exit the program";
                zCheckBox2.Text = "Exit the program";
                zButton1.Text = "Determine";
                zButton2.Text = "Cancel";
            }
            else if (language.Contains("JPN"))
            {
                this.Text = " 暖かいヒント";
                label1.Text = "閉じるボタンを選択しました";
                zCheckBox1.Text = "システムトレイエリアに最小化して、プログラムを終了しません";
                zCheckBox2.Text = "プログラムを終了";
                zButton1.Text = "を選択します";
                zButton2.Text = "キャンセル";
            }
            else if (language.Contains("FR"))
            {
                this.Size = new System.Drawing.Size(700, 214);
                this.Text = " Conseils chaleureux";
                label1.Text = "Vous avez sélectionné le bouton Fermer et vous voulez";
                zCheckBox1.Text = "Réduire au minimum la zone du plateau système sans quitter le programme";
                zCheckBox2.Text = "Procédure de sortie";
                zButton1.Text = "C'est sûr";
                zButton2.Text = "Annulation";
            }
            zCheckBox2.Checked = true;    
        }

        private void zButton1_Click(object sender, EventArgs e)
        {
            if (zCheckBox1.Checked==true)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (zCheckBox2.Checked == true)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
        }

        private void zCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (zCheckBox1.Checked==true)
            {
                zCheckBox2.Checked = false;
            }
            else
            {
                zCheckBox2.Checked = true;
            }
        }

        private void zCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (zCheckBox2.Checked == true)
            {
                zCheckBox1.Checked = false;
            }
            else
            {
                zCheckBox1.Checked = true;
            }
        }

        private void zButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Close_Activated(object sender, EventArgs e)
        {
            zCheckBox2.Focus();
            zButton1.Focus();
        }
    }
}
