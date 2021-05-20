
using MetroFramework.Native;
/**
* MetroFramework - Modern UI for WinForms
* 
* The MIT License (MIT)
* Copyright (c) 2011 Sven Walter, http://github.com/viperneo
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy of 
* this software and associated documentation files (the "Software"), to deal in the 
* Software without restriction, including without limitation the rights to use, copy, 
* modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
* and to permit persons to whom the Software is furnished to do so, subject to the 
* following conditions:
* 
* The above copyright notice and this permission notice shall be included in 
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
* INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
* PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
* HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
* CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
* OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security;
using System.Windows.Forms;

namespace ZXCL.WinFormUI
{
    #region Enums

    public enum ZScrollOrientation
    {
        Horizontal,
        Vertical
    }

    #endregion

    public class ZScrollBar : Control
    {
       

        #region Events

        public event ScrollEventHandler Scroll;

        private void OnScroll(ScrollEventType type, int oldValue, int newValue, ScrollOrientation orientation)
        {
            if (oldValue != newValue)
            {
                if (ValueChanged != null)
                    ValueChanged(this, curValue);
            }

            if (Scroll == null) return;

            if (orientation == ScrollOrientation.HorizontalScroll)
            {
                if (type != ScrollEventType.EndScroll && isFirstScrollEventHorizontal)
                {
                    type = ScrollEventType.First;
                }
                else if (!isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
                {
                    isFirstScrollEventHorizontal = true;
                }
            }
            else
            {
                if (type != ScrollEventType.EndScroll && isFirstScrollEventVertical)
                {
                    type = ScrollEventType.First;
                }
                else if (!isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
                {
                    isFirstScrollEventVertical = true;
                }
            }

            Scroll(this, new ScrollEventArgs(type, oldValue, newValue, orientation));
        }

        private void OnScrollForSet(ScrollEventType type, int oldValue, int newValue, ScrollOrientation orientation)
        {
            if (oldValue != newValue)
            {
                if (ValueChanged != null)
                    ValueChanged(this, curValue);
            }

            if (Scroll == null) return;

            if (orientation == ScrollOrientation.HorizontalScroll)
            {
                if (type != ScrollEventType.EndScroll && isFirstScrollEventHorizontal)
                {
                    type = ScrollEventType.First;
                }
                else if (!isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
                {
                    isFirstScrollEventHorizontal = true;
                }
            }
            else
            {
                if (type != ScrollEventType.EndScroll && isFirstScrollEventVertical)
                {
                    type = ScrollEventType.First;
                }
                else if (!isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
                {
                    isFirstScrollEventVertical = true;
                }
            }

            //Scroll(this, new ScrollEventArgs(type, oldValue, newValue, orientation));
        }

        #endregion

        #region Fields

        private bool isFirstScrollEventVertical = true;
        private bool isFirstScrollEventHorizontal = true;

        private bool inUpdate;

        private Rectangle clickedBarRectangle;
        private Rectangle thumbRectangle;

        private bool topBarClicked;
        private bool bottomBarClicked;
        private bool thumbClicked;

        private int thumbWidth = 6;
        private int thumbHeight;

        private int thumbBottomLimitBottom;
        private int thumbBottomLimitTop;
        private int thumbTopLimit;
        private int thumbPosition;

        private int trackPosition;

        private readonly Timer progressTimer = new Timer();

        private int mouseWheelBarPartitions = 10;

        public int MouseWheelBarPartitions
        {
            get { return mouseWheelBarPartitions; }
            set
            {
                if (value > 0)
                {
                    mouseWheelBarPartitions = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", "MouseWheelBarPartitions has to be greather than zero");
                }
            }
        }

        private bool isHovered;
        private bool isPressed;

        private bool useBarColor = false;
        [DefaultValue(false)]
        public bool UseBarColor
        {
            get { return useBarColor; }
            set { useBarColor = value; }
        }

        public int ScrollbarSize
        {
            get { return Orientation == ZScrollOrientation.Vertical ? Width : Height; }
            set
            {
                if (Orientation == ZScrollOrientation.Vertical)
                    Width = value;
                else
                    Height = value;
            }
        }

        private bool highlightOnWheel = false;
        [DefaultValue(false)]
        public bool HighlightOnWheel
        {
            get { return highlightOnWheel; }
            set { highlightOnWheel = value; }
        }

        private ZScrollOrientation metroOrientation = ZScrollOrientation.Vertical;
        private ScrollOrientation scrollOrientation = ScrollOrientation.VerticalScroll;

        public ZScrollOrientation Orientation
        {
            get { return metroOrientation; }

            set
            {
                if (value == metroOrientation)
                {
                    return;
                }

                metroOrientation = value;

                if (value == ZScrollOrientation.Vertical)
                {
                    scrollOrientation = ScrollOrientation.VerticalScroll;
                }
                else
                {
                    scrollOrientation = ScrollOrientation.HorizontalScroll;
                }

                Size = new Size(Height, Width);
                SetupScrollBar();
            }
        }

        private int minimum;
        private int maximum = 100;
        private int smallChange = 1;
        private int largeChange = 10;
        private int curValue;

        public int Minimum
        {
            get { return minimum; }
            set
            {
                if (minimum == value || value < 0 || value >= maximum)
                {
                    return;
                }

                minimum = value;
                if (curValue < value)
                {
                    curValue = value;
                }

                if (largeChange > (maximum - minimum))
                {
                    largeChange = maximum - minimum;
                }

                SetupScrollBar();

                if (curValue < value)
                {
                    dontUpdateColor = true;
                    Value = value;
                }
                else
                {
                    ChangeThumbPosition(GetThumbPosition());
                    Refresh();
                }
            }
        }

        public int Maximum
        {
            get { return maximum; }
            set
            {
                if (value == maximum || value < 1 || value <= minimum)
                {
                    return;
                }

                maximum = value;
                if (largeChange > (maximum - minimum))
                {
                    largeChange = maximum - minimum;
                }

                SetupScrollBar();

                if (curValue > value)
                {
                    dontUpdateColor = true;
                    Value = maximum;
                }
                else
                {
                    ChangeThumbPosition(GetThumbPosition());
                    Refresh();
                }
            }
        }

        [DefaultValue(1)]
        public int SmallChange
        {
            get { return smallChange; }
            set
            {
                if (value == smallChange || value < 1 || value >= largeChange)
                {
                    return;
                }

                smallChange = value;
                SetupScrollBar();
            }
        }

        [DefaultValue(5)]
        public int LargeChange
        {
            get { return largeChange; }
            set
            {
                if (value == largeChange || value < smallChange || value < 2)
                {
                    return;
                }

                if (value > (maximum - minimum))
                {
                    largeChange = maximum - minimum;
                }
                else
                {
                    largeChange = value;
                }

                SetupScrollBar();
            }
        }

        public bool ShowDrop { get; set; }

        #region ValueChangeEvent
        // Declare a delegate
        public delegate void ScrollValueChangedDelegate(object sender, int newValue);

        public event ScrollValueChangedDelegate ValueChanged;
        #endregion

        private bool dontUpdateColor = false;

        [DefaultValue(0)]
        [Browsable(false)]
        public int Value
        {
            get { return curValue; }

            set
            {
                if (curValue == value || value < minimum || value > maximum)
                {
                    return;
                }

                curValue = value;

                ChangeThumbPosition(GetThumbPosition());

                OnScroll(ScrollEventType.ThumbPosition, -1, value, scrollOrientation);

                if (!dontUpdateColor && highlightOnWheel)
                {
                    if (!isHovered)
                        isHovered = true;

                    if (autoHoverTimer == null)
                    {
                        autoHoverTimer = new Timer();
                        autoHoverTimer.Interval = 1000;
                        autoHoverTimer.Tick += new EventHandler(autoHoverTimer_Tick);
                        autoHoverTimer.Start();
                    }
                    else
                    {
                        autoHoverTimer.Stop();
                        autoHoverTimer.Start();
                    }
                }
                else
                {
                    dontUpdateColor = false;
                }

                Refresh();
            }
        }

        [DefaultValue(0)]
        [Browsable(false)]
        public int ValueSet
        {
            get { return curValue; }

            set
            {
                if (curValue == value || value < minimum || value > maximum)
                {
                    return;
                }

                curValue = value;

                ChangeThumbPosition(GetThumbPosition());

                OnScrollForSet(ScrollEventType.ThumbPosition, -1, value, scrollOrientation);

                if (!dontUpdateColor && highlightOnWheel)
                {
                    if (!isHovered)
                        isHovered = true;

                    if (autoHoverTimer == null)
                    {
                        autoHoverTimer = new Timer();
                        autoHoverTimer.Interval = 1000;
                        autoHoverTimer.Tick += new EventHandler(autoHoverTimer_Tick);
                        autoHoverTimer.Start();
                    }
                    else
                    {
                        autoHoverTimer.Stop();
                        autoHoverTimer.Start();
                    }
                }
                else
                {
                    dontUpdateColor = false;
                }

                Refresh();
            }
        }

        private void autoHoverTimer_Tick(object sender, EventArgs e)
        {
            isHovered = false;
            Invalidate();
            autoHoverTimer.Stop();
        }

        private Timer autoHoverTimer = null;

        #endregion

        #region Constructor

        public ZScrollBar()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint, true);

            Width = 10;
            Height = 200;

            SetupScrollBar();

            progressTimer.Interval = 20;
            progressTimer.Tick += ProgressTimerTick;
        }

        public ZScrollBar(ZScrollOrientation orientation)
            : this()
        {
            Orientation = orientation;
        }

        public ZScrollBar(ZScrollOrientation orientation, int width)
            : this(orientation)
        {
            Width = width;
        }

        public bool HitTest(Point point)
        {
            return thumbRectangle.Contains(point);
        }

        #endregion

        #region Update Methods

        [SecuritySafeCritical]
        public void BeginUpdate()
        {
            WinApi.SendMessage(Handle, (int)WinApi.Messages.WM_SETREDRAW, false, 0);
            inUpdate = true;
        }

        [SecuritySafeCritical]
        public void EndUpdate()
        {
            WinApi.SendMessage(Handle, (int)WinApi.Messages.WM_SETREDRAW, true, 0);
            inUpdate = false;
            SetupScrollBar();
            Refresh();
        }

        #endregion

        #region Paint Methods

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            try
            {
                Color backColor = BackColor;

                //if (!useCustomBackColor)
                //{
                //    if (Parent != null)
                //    {
                //        if (Parent is IMetroControl)
                //        {
                //            backColor = MetroPaint.BackColor.Form(Theme);
                //        }
                //        else
                //        {
                //            backColor = Parent.BackColor;
                //        }
                //    }
                //    else
                //    {
                //        backColor = MetroPaint.BackColor.Form(Theme);
                //    }
                //}

                if (backColor.A == 255)
                {
                    e.Graphics.Clear(backColor);
                    return;
                }

                base.OnPaintBackground(e);

              
            }
            catch
            {
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (GetStyle(ControlStyles.AllPaintingInWmPaint))
                {
                    OnPaintBackground(e);
                }

              
                OnPaintForeground(e);
            }
            catch
            {
                Invalidate();
            }
        }

        protected virtual void OnPaintForeground(PaintEventArgs e)
        {
            Color backColor, thumbColor, barColor;

            backColor = Color.Transparent;

            if (isHovered && !isPressed && Enabled)
            {
                thumbColor = Color.FromArgb(136, 136, 136);
                barColor = Color.FromArgb(242,242,242);
            }
            else if (isHovered && isPressed && Enabled)
            {
                thumbColor = Color.FromArgb(96, 96, 96);
                barColor = Color.FromArgb(242, 242, 242);
            }
            else if (!Enabled)
            {
                thumbColor = Color.FromArgb(242, 242, 242);
                barColor = Color.FromArgb(242, 242, 242);
            }
            else
            {
                thumbColor = Color.FromArgb(221, 221, 221);
                barColor = Color.FromArgb(242, 242, 242);
            }

            DrawScrollBar(e.Graphics, backColor, thumbColor, barColor);

        }

       

        private void DrawScrollBar(Graphics g, Color backColor, Color thumbColor, Color barColor)
        {
            if (useBarColor)
            {
                using (var b = new SolidBrush(barColor))
                {
                    g.FillRectangle(b, ClientRectangle);
                }
            }

            using (var b = new SolidBrush(barColor))
            {
                g.FillRectangle(b, ClientRectangle);
            }

            using (var b = new SolidBrush(backColor))
            {
                var thumbRect = new Rectangle(thumbRectangle.X - 1, thumbRectangle.Y - 1, thumbRectangle.Width + 2, thumbRectangle.Height + 2);
                g.FillRectangle(b, thumbRect);
            }

            using (var b = new SolidBrush(thumbColor))
            {
                g.FillRectangle(b, thumbRectangle);
            }

            if (ShowDrop)
                drawDrop(g);

        }

        void drawDrop(Graphics g)
        {
            SmoothingMode smoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.HighQuality;

            if (Orientation == ZScrollOrientation.Horizontal)
            {
                int o = ClientRectangle.Height / 2;
                int w = (int)(ClientRectangle.Height * 0.7);
                int h = w / 2;

                PointF a = new Point(5 + h, o - h);
                PointF b = new Point(5, o);
                PointF c = new Point(5 + h, o + h);


                PointF a1 = new Point(ClientRectangle.Width - 5 - h, o - h);
                PointF b1 = new Point(ClientRectangle.Width - 5, o);
                PointF c1 = new Point(ClientRectangle.Width - 5 - h, o + h);

                using (Pen p = new Pen(Color.Black, 1f))
                {
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(new PointF[] { a, b, c });
                    g.DrawPath(p, path);
                }

                using (Pen p = new Pen(Color.Black, 1f))
                {
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(new PointF[] { a1, b1, c1 });
                    g.DrawPath(p, path);
                }
            }
            else
            {

                int o = ClientRectangle.Width / 2;
                int w = (int)(ClientRectangle.Width * 0.7);
                int h = w / 2;

                PointF a = new Point(o, 5);
                PointF b = new Point(o - h, 5 + h);
                PointF c = new Point(o + h, 5 + h);


                PointF a1 = new Point(o, ClientRectangle.Height - 6);
                PointF b1 = new Point(o - h, ClientRectangle.Height - 6 - h);
                PointF c1 = new Point(o + h, ClientRectangle.Height - 6 - h);

                using (Pen p = new Pen(Color.Black, 1f))
                {
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(new PointF[] { b, a, c });
                    g.DrawPath(p, path);
                }

                using (Pen p = new Pen(Color.Black, 1f))
                {
                    GraphicsPath path = new GraphicsPath();
                    path.AddLines(new PointF[] { b1, a1, c1 });
                    g.DrawPath(p, path);
                }
            }
            g.SmoothingMode = smoothingMode;
        }

        #endregion

        #region Focus Methods

        protected override void OnGotFocus(EventArgs e)
        {
            Invalidate();

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnLostFocus(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            Invalidate();

            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnLeave(e);
        }

        #endregion

        #region Mouse Methods

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            int v = e.Delta / 120 * (maximum - minimum) / mouseWheelBarPartitions;
           

            if (Orientation == ZScrollOrientation.Vertical)
            {
                if (Value < v)
                    Value = 0;
                else
                    Value -= v;


            }
            else
            {
                if (Value + v < 0)
                    Value = 0;
                else
                    Value += v;
            }

           
        }

        bool isMouseOnDrop;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }

            base.OnMouseDown(e);

            Focus();

            if (e.Button == MouseButtons.Left)
            {

                var mouseLocation = e.Location;

                if(metroOrientation == ZScrollOrientation.Vertical)
                {
                    if (new Rectangle(0, 0, this.Width, this.Width).Contains(mouseLocation))
                        isMouseOnDrop = true;
                    else if (new Rectangle(0, this.ClientRectangle.Bottom - this.Width, this.Width, this.Width).Contains(mouseLocation))
                        isMouseOnDrop = true;
                }
                else
                {
                    if (new Rectangle(0, 0, this.Height, this.Height).Contains(mouseLocation))
                        isMouseOnDrop = true;
                    else if (new Rectangle(this.ClientRectangle.Width-this.Height, 0, this.Height, this.Height).Contains(mouseLocation))
                        isMouseOnDrop = true;
                }

                if (thumbRectangle.Contains(mouseLocation))
                {
                    thumbClicked = true;
                    thumbPosition = metroOrientation == ZScrollOrientation.Vertical ? mouseLocation.Y - thumbRectangle.Y : mouseLocation.X - thumbRectangle.X;

                    Invalidate(thumbRectangle);
                }
                else
                {
                    trackPosition = metroOrientation == ZScrollOrientation.Vertical ? mouseLocation.Y : mouseLocation.X;

                    if (trackPosition < (metroOrientation == ZScrollOrientation.Vertical ? thumbRectangle.Y : thumbRectangle.X))
                    {
                        topBarClicked = true;
                    }
                    else
                    {
                        bottomBarClicked = true;
                    }

                    ProgressThumb(true);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                trackPosition = metroOrientation == ZScrollOrientation.Vertical ? e.Y : e.X;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isPressed = false;
            isMouseOnDrop = false;

            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                if (thumbClicked)
                {
                    thumbClicked = false;
                    OnScroll(ScrollEventType.EndScroll, -1, curValue, scrollOrientation);
                }
                else if (topBarClicked)
                {
                    topBarClicked = false;
                    StopTimer();
                }
                else if (bottomBarClicked)
                {
                    bottomBarClicked = false;
                    StopTimer();
                }

                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            Invalidate();

            base.OnMouseLeave(e);

            ResetScrollStatus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
                if (thumbClicked)
                {
                    int oldScrollValue = curValue;

                    int pos = metroOrientation == ZScrollOrientation.Vertical ? e.Location.Y : e.Location.X;
                    int thumbSize = metroOrientation == ZScrollOrientation.Vertical ? (pos / Height) / thumbHeight : (pos / Width) / thumbWidth;

                    if (pos <= (thumbTopLimit + thumbPosition))
                    {
                        ChangeThumbPosition(thumbTopLimit);
                        curValue = minimum;
                        Invalidate();
                    }
                    else if (pos >= (thumbBottomLimitTop + thumbPosition))
                    {
                        ChangeThumbPosition(thumbBottomLimitTop);
                        curValue = maximum;
                        Invalidate();
                    }
                    else
                    {
                        ChangeThumbPosition(pos - thumbPosition);

                        int pixelRange, thumbPos;

                        if (Orientation == ZScrollOrientation.Vertical)
                        {
                            pixelRange = Height - thumbSize;
                            thumbPos = thumbRectangle.Y;
                        }
                        else
                        {
                            pixelRange = Width - thumbSize;
                            thumbPos = thumbRectangle.X;
                        }

                        float perc = 0f;

                        if (pixelRange != 0)
                        {
                            perc = (thumbPos) / (float)pixelRange;
                        }

                        curValue = Convert.ToInt32((perc * (maximum - minimum)) + minimum);
                    }

                    if (oldScrollValue != curValue)
                    {
                        OnScroll(ScrollEventType.ThumbTrack, oldScrollValue, curValue, scrollOrientation);
                        Refresh();
                    }
                }
            }
            else if (!ClientRectangle.Contains(e.Location))
            {
                ResetScrollStatus();
            }
            else if (e.Button == MouseButtons.None)
            {
                if (thumbRectangle.Contains(e.Location))
                {
                    Invalidate(thumbRectangle);
                }
                else if (ClientRectangle.Contains(e.Location))
                {
                    Invalidate();
                }
            }
        }

        #endregion

        #region Keyboard Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
            isHovered = true;
            isPressed = true;
            Invalidate();

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            isHovered = false;
            isPressed = false;
            Invalidate();

            base.OnKeyUp(e);
        }

        #endregion

        #region Management Methods

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);

            if (DesignMode)
            {
                SetupScrollBar();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetupScrollBar();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            var keyUp = Keys.Up;
            var keyDown = Keys.Down;

            if (Orientation == ZScrollOrientation.Horizontal)
            {
                keyUp = Keys.Left;
                keyDown = Keys.Right;
            }

            if (keyData == keyUp)
            {
                Value -= smallChange;

                return true;
            }

            if (keyData == keyDown)
            {
                Value += smallChange;

                return true;
            }

            if (keyData == Keys.PageUp)
            {
                Value = GetValue(false, true);

                return true;
            }

            if (keyData == Keys.PageDown)
            {
                if (curValue + largeChange > maximum)
                {
                    Value = maximum;
                }
                else
                {
                    Value += largeChange;
                }

                return true;
            }

            if (keyData == Keys.Home)
            {
                Value = minimum;

                return true;
            }

            if (keyData == Keys.End)
            {
                Value = maximum;

                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        private void SetupScrollBar()
        {
            if (inUpdate) return;

            if (Orientation == ZScrollOrientation.Vertical)
            {
                thumbWidth = Width > 0 ? Width : 10;
                thumbHeight = GetThumbSize();

                clickedBarRectangle = ClientRectangle;
                clickedBarRectangle.Inflate(-1, -1);

                thumbRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, thumbWidth, thumbHeight);

                thumbPosition = thumbRectangle.Height / 2;
                thumbBottomLimitBottom = ClientRectangle.Bottom;
                thumbBottomLimitTop = thumbBottomLimitBottom - thumbRectangle.Height;
                thumbTopLimit = ClientRectangle.Y;
            }
            else
            {
                thumbHeight = Height > 0 ? Height : 10;
                thumbWidth = GetThumbSize();

                clickedBarRectangle = ClientRectangle;
                clickedBarRectangle.Inflate(-1, -1);

                thumbRectangle = new Rectangle(ClientRectangle.X, ClientRectangle.Y, thumbWidth, thumbHeight);

                thumbPosition = thumbRectangle.Width / 2;
                thumbBottomLimitBottom = ClientRectangle.Right;
                thumbBottomLimitTop = thumbBottomLimitBottom - thumbRectangle.Width;
                thumbTopLimit = ClientRectangle.X;
            }

            ChangeThumbPosition(GetThumbPosition());

            Refresh();
        }

        private void ResetScrollStatus()
        {
            bottomBarClicked = topBarClicked = false;

            StopTimer();
            Refresh();
        }

        private void ProgressTimerTick(object sender, EventArgs e)
        {
             ProgressThumb(true);
        }

        private int GetValue(bool smallIncrement, bool up)
        {
            int newValue;

            if (up)
            {
                newValue = curValue - (smallIncrement ? smallChange : largeChange);

                if (newValue < minimum)
                {
                    newValue = minimum;
                }
            }
            else
            {
                newValue = curValue + (smallIncrement ? smallChange : largeChange);

                if (newValue > maximum)
                {
                    newValue = maximum;
                }
            }

            return newValue;
        }

        private int GetThumbPosition()
        {
            int pixelRange;

            if (thumbHeight == 0 || thumbWidth == 0)
            {
                return 0;
            }

            int thumbSize = metroOrientation == ZScrollOrientation.Vertical ? (thumbPosition / Height) / thumbHeight : (thumbPosition / Width) / thumbWidth;

            if (Orientation == ZScrollOrientation.Vertical)
            {
                pixelRange = Height - thumbSize;
            }
            else
            {
                pixelRange = Width - thumbSize;
            }

            int realRange = maximum - minimum;
            float perc = 0f;

            if (realRange != 0)
            {
                perc = (curValue - (float)minimum) / realRange;
            }

            return Math.Max(thumbTopLimit, Math.Min(thumbBottomLimitTop, Convert.ToInt32((perc * pixelRange))));
        }

        private int GetThumbSize()
        {
            int trackSize =
                metroOrientation == ZScrollOrientation.Vertical ?
                    Height : Width;

            if (maximum == 0 || largeChange == 0)
            {
                return trackSize;
            }

            float newThumbSize = (largeChange * (float)trackSize) / maximum;

            return Convert.ToInt32(Math.Min(trackSize, Math.Max(newThumbSize, 10f)));
        }

        private void EnableTimer()
        {
            if (!progressTimer.Enabled)
            {
                progressTimer.Interval = 600;
                progressTimer.Start();
            }
            else
            {
                progressTimer.Interval = 10;
            }
        }

        private void StopTimer()
        {
            progressTimer.Stop();
        }

        private void ChangeThumbPosition(int position)
        {
            if (Orientation == ZScrollOrientation.Vertical)
            {
                thumbRectangle.Y = position;
            }
            else
            {
                thumbRectangle.X = position;
            }
        }

        private void ProgressThumb(bool enableTimer)
        {
            int scrollOldValue = curValue;
            var type = ScrollEventType.First;
            int thumbSize, thumbPos;

            if (Orientation == ZScrollOrientation.Vertical)
            {
                thumbPos = thumbRectangle.Y;
                thumbSize = thumbRectangle.Height;
            }
            else
            {
                thumbPos = thumbRectangle.X;
                thumbSize = thumbRectangle.Width;
            }

            if ((bottomBarClicked && (thumbPos + thumbSize) < trackPosition))
            {
                type = ScrollEventType.LargeIncrement;

                curValue = GetValue(isMouseOnDrop, false);

                if (curValue == maximum)
                {
                    ChangeThumbPosition(thumbBottomLimitTop);

                    type = ScrollEventType.Last;
                }
                else
                {
                    ChangeThumbPosition(Math.Min(thumbBottomLimitTop, GetThumbPosition()));
                }
            }
            else if ((topBarClicked && thumbPos > trackPosition))
            {
                type = ScrollEventType.LargeDecrement;

                curValue = GetValue(isMouseOnDrop, true);

                if (curValue == minimum)
                {
                    ChangeThumbPosition(thumbTopLimit);

                    type = ScrollEventType.First;
                }
                else
                {
                    ChangeThumbPosition(Math.Max(thumbTopLimit, GetThumbPosition()));
                }
            }

            if (scrollOldValue != curValue)
            {
                OnScroll(type, scrollOldValue, curValue, scrollOrientation);

                Invalidate();

                if (enableTimer)
                {
                    EnableTimer();
                }
            }
        }

        #endregion
    }
}