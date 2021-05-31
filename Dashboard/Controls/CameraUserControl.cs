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

        public string TopRightText
        {
            get => labelTopRight.Text;
            set => labelTopRight.Text = value;
        }

        public string BottomText
        {
            get => labelBottomCenter.Text;
            set => labelBottomCenter.Text = value;
        }

        public Image Image
        {
            set => pictureBox1.Image = value;
            get => pictureBox1.Image;
        }

        public Color BackgroundColor
        {
            set => this.BackColor = value;
        }

        public void Select()
        {
            this.Selected = true;
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

        public event EventHandler<MouseEventArgs> MouseClicked;

        public CameraUserControl()
        {
            InitializeComponent();
            this.MouseClick += CameraUserControl_MouseClick;
            this.pictureBox1.MouseClick += PictureBox1_MouseClick;
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
