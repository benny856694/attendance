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
        private int l;
        public Close(string language)
        {
            this.language = language;
            InitializeComponent();
            this.l = zCheckBox1.Left;
        }

        private void Close_Load(object sender, EventArgs e)
        {
            //根据不同语言设置 文字
            if (language.Contains("US"))
            {
                this.Text = " Quit";
                label1.Text = "You have clicked the close button and do you want to";
                zCheckBox1.Text = "Minimize to the system tray area, do not exit the program";
                zCheckBox2.Text = "Exit the program";
                zButton1.Text = "OK";
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
                this.Text = " Conseils chaleureux";
                label1.Text = "Vous avez sélectionné le bouton Fermer et vous voulez";
                zCheckBox1.Text = "Réduire au minimum la zone du plateau système sans quitter le programme";
                zCheckBox2.Text = "Procédure de sortie";
                zButton1.Text = "C'est sûr";
                zButton2.Text = "Annulation";
            }
            else if (language.Contains("vi"))
            {
                this.Text = "Nhắc lại";
                label1.Text = "Bạn đã chọn nút đóng và bạn muốn";
                zCheckBox1.Text = "Thu nhỏ vùng khay hệ thống, không được thoát khỏi chương trình.";
                zCheckBox2.Text = "Thoát chương trình.";
                zButton1.Text = "Phải.";
                zButton2.Text = "Không";
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
                if (!needShowCloseDialog(3))//判断是否需要显示警告弹窗,警告用户不能长期不运行程序
                {
                    Properties.Settings1.Default.LastCloseDate = DateTime.Now;
                    Properties.Settings1.Default.Save();
                    this.DialogResult = DialogResult.No;
                    this.Close();
                }
                else
                {
                    this.Hide();
                    string alert = Properties.Strings.Alert;
                    string exitTip = Properties.Strings.ExitTip;
                    string AlertExitMsg = Properties.Strings.AlertExitMsg;
                    DialogResult dr = MessageBox.Show(""+ AlertExitMsg + "\r\n"+ exitTip + "", alert, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    if (dr == DialogResult.OK)
                    {
                        Properties.Settings1.Default.LastCloseDate = DateTime.Now;
                        Properties.Settings1.Default.Save();
                        this.DialogResult = DialogResult.No;
                        this.Close();
                    }
                    else
                    {
                        this.Show();
                    }
                }
            }
        }
        //是否需要显示退出警告提示弹窗
        private bool needShowCloseDialog(int dayTime)
        {
            //获取今天时间
            DateTime now = DateTime.Now;
            //获取上次关闭应用程序时间
            var lastCloseDate=Properties.Settings1.Default.LastCloseDate;
            var minValue = DateTime.Parse("2020-1-1 0:00:00");
            if (minValue.Subtract(lastCloseDate).Days == 0)
            {//初始不需要显示警告
                return false;
            }
            //如果上次关闭应用程序时间与今天相差超过dayTime天，则要提示
            if (now.Subtract(lastCloseDate).Days > dayTime)
            {
                return true;
            }
            return false;
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

        private void Close_Shown(object sender, EventArgs e)
        {
            var maxWidth = CalcMaxRight();

            if (maxWidth > this.Width)
            {
                var deltaWidth = maxWidth - this.Width + 20;
                this.Width += deltaWidth;
                this.Left -= deltaWidth / 2;
            }


            //if (this.labelX1.Bottom > this.ok.Top)
            //{
            //    var delta = this.labelX1.Bottom - this.bottom;
            //    this.Height += delta;
            //    this.Top -= delta / 2;
            //}
        }

        private int CalcMaxRight()
        {
            var w = 0;
            var ctrls = new Control[] { this.zCheckBox1, this.zCheckBox2, this.label1 };
            w = ctrls.Max(c => c.Right);
            return w;
        }
    }
}
