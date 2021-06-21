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
                pictureBoxLeft.Image = value;
            }
            get => pictureBoxLeft.Image;
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

        public Image ImagePair1
        {
            get => pictureBoxPair1.Image;
            set
            {
                ShowPairingImages = true;
                pictureBoxPair1.Image = value;
            }
        }

        public Image ImagePair2
        {
            get => pictureBoxPair2.Image;
            set
            {
                ShowPairingImages = true;
                pictureBoxPair2.Image = value;
            }
        }

        public Image ImagePair3
        {
            get => pictureBoxPair3.Image;
            set
            {
                ShowPairingImages = true;
                pictureBoxPair3.Image = value;
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
                        splitContainer1.Panel2Collapsed = true;

                        break;
                    case DisplayMode.Double:
                        splitContainer1.Panel2Collapsed = false;
                        break;
                    default:
                        break;
                }

                _displayMode = value;
            }
        }

        public bool ShowPairingImages
        {
            set => panelPairing.Visible = value; 
        }

        public PictureBox[] PairingPictureBoxs => new PictureBox[] { pictureBoxPair1, pictureBoxPair2, pictureBoxPair3 };

        public void ClearPairImages()
        {
            pictureBoxPair1.Image = null;
            pictureBoxPair2.Image = null;
            pictureBoxPair3.Image = null;
        }


        public event EventHandler<MouseEventArgs> MouseClicked;

        public CameraUserControl()
        {
            InitializeComponent();
            this.MouseClick += CameraUserControl_MouseClick;
            this.pictureBoxLeft.MouseClick += PictureBox1_MouseClick;
            this.pictureBoxRight.MouseClick += PictureBox1_MouseClick;
            this.pictureBoxPair1.MouseClick += PictureBox1_MouseClick;
            this.pictureBoxPair2.MouseClick += PictureBox1_MouseClick;
            this.pictureBoxPair3.MouseClick += PictureBox1_MouseClick;
            this.labelTopRight.SizeChanged += LabelTopRight_SizeChanged;
        }

        public void Clear()
        {
            this.pictureBoxLeft.Image = null;
            this.pictureBoxRight.Image = null;
            this.pictureBoxPair1.Image = null;
            this.pictureBoxPair2.Image = null;
            this.pictureBoxPair3.Image = null;
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
