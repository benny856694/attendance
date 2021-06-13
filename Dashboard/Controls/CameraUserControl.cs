using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dashboard.Controls
{
    public partial class CameraUserControl : UserControl, ISelectable
    {
        private bool _selected;
        private DisplayMode _displayMode;

        public string TopRightText
        {
            get => labelTopRight.Text;
            set
            {
                labelTopRight.Text = value;

            }
        }

        public string BottomText
        {
            get => labelBottomCenter.Text;
            set => labelBottomCenter.Text = value;
        }

        public Image Image
        {
            set
            {
                Mode = DisplayMode.Single;
                pictureBoxSingle.Image = value;
            }
            get => pictureBoxSingle.Image;
        }

        public Image ImageLeft
        {
            get => pictureBoxLeft.Image;
            set
            {
                Mode = DisplayMode.Double;
                pictureBoxLeft.Image = value;
            }
        }

        public Image ImageRight
        {
            get => pictureBoxRight.Image;
            set
            {
                Mode = DisplayMode.Double;
                pictureBoxRight.Image = value;
            }
        }

        public Color BackgroundColor
        {
            set => this.BackColor = value;
        }

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                Invalidate();
            }
        }

        public DisplayMode Mode 
        {
            get => _displayMode;
            set
            {
                switch (value)
                {
                    case DisplayMode.Single:
                        bunifuPagesImageContainer.SelectedIndex = 0;
                        break;
                    case DisplayMode.Double:
                        bunifuPagesImageContainer.SelectedIndex = 1;
                        break;
                    default:
                        break;
                }
            }
        }


        public event EventHandler<MouseEventArgs> MouseClicked;

        public CameraUserControl()
        {
            InitializeComponent();
            this.MouseClick += CameraUserControl_MouseClick;
            this.pictureBoxSingle.MouseClick += PictureBox1_MouseClick;
            this.labelTopRight.SizeChanged += LabelTopRight_SizeChanged;
        }

        public void Clear()
        {
            var pic = this.pictureBoxSingle.Image;
            this.pictureBoxSingle.Image = null;
            this.labelTopRight.Text = "";
            this.labelBottomCenter.Text = "";
            this.BackColor = Color.Black;
        }

        private void LabelTopRight_SizeChanged(object sender, EventArgs e)
        {
            //labelTopRight.Left = this.Width - 5 - labelTopRight.Width;
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            this.MouseClicked?.Invoke(this, e);
        }

        private void CameraUserControl_MouseClick(object sender, MouseEventArgs e)
        {
            this.MouseClicked?.Invoke(this, e);
        }

        private void CameraUserControl_Paint(object sender, PaintEventArgs e)
        {
            if (_selected)
            {
                using (Pen pen = new Pen(Color.White, 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, Width - 4, Height - 4);
                }
            }
        }

    }
}
