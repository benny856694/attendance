using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MultiPlayer.Properties;

namespace VideoHelper
{
    public partial class MultiPlayerPanel : UserControl
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32")]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        public MultiPlayerPanel()
        {
            InitializeComponent();
        }

        public void Pause()
        {
            for (int i = 0; i < m_activeTLP.Controls.Count; ++i)
            {
                Control c = m_activeTLP.Controls[i];
                if (c is MultiPlayerControl)
                {
                    (c as MultiPlayerControl).MPCPause();
                }
            }
        }

        public void Resume()
        {
            for (int i = 0; i < m_activeTLP.Controls.Count; ++i)
            {
                Control c = m_activeTLP.Controls[i];
                if (c is MultiPlayerControl)
                {
                    (c as MultiPlayerControl).MPCResume();
                }
            }
        }

        private bool fs;
        public bool IsFullScreen => fs;
        public void ToggleFullScreen()
        {
            if(m_fsMPC != null)
            {
                m_fsMPC.ExitFullScreen();
                return;
            }

            for (int i = 0; i < m_activeTLP.Controls.Count; ++i)
            {
                Control c = m_activeTLP.Controls[i];
                if (c is MultiPlayerControl)
                {
                    (c as MultiPlayerControl).MPCPause();
                }
            }
            Control control = panel2;
            if (!fs)
            {
                control.Dock = DockStyle.None;
                control.Left = 0;
                control.Top = 0;
                control.Width = Screen.PrimaryScreen.Bounds.Width;
                control.Height = Screen.PrimaryScreen.Bounds.Height;
                SetParent(control.Handle, IntPtr.Zero);
                SetForegroundWindow(control.Handle);
                fs = true;
            }
            else
            {
                SetParent(control.Handle, Handle);
                control.Dock = DockStyle.Fill;
                fs = false;
            }

            for (int i = 0; i < m_activeTLP.Controls.Count; ++i)
            {
                Control c = m_activeTLP.Controls[i];
                if (c is MultiPlayerControl)
                {
                    (c as MultiPlayerControl).MPCResume();
                }
            }
        }

        private MultiPlayerControl m_fsMPC = null;
        private void Mpc_MPCLeaveFullScreen(object sender, EventArgs e)
        {
            m_fsMPC = null;
        }

        private void Mpc_MPCEnterFullScreen(object sender, EventArgs e)
        {
            m_fsMPC = sender as MultiPlayerControl;
        }

        public static MultiPlayerControl ConnectCamera(Holder h)
        {
            MultiPlayerControl mpc = m_activeMPC;
            if (!activeChangedByUser) // 如果未手动选过，当用于多次尝试连接设备，则选取多画面中的下一个位置显示
            {
                if (mpc == null)
                {
                    mpc = mpc1;
                }
                else
                {
                    bool find = false;
                    foreach(Control ctrl in m_activeTLP.Controls)
                    {
                        if(ctrl is MultiPlayerControl)
                        {
                            if (find)
                            {
                                mpc = ctrl as MultiPlayerControl;
                                break;
                            }
                            else if(ctrl == m_activeMPC)
                                find = true;
                        }
                    }
                }
            }
            mpc.Holder = h;
            if (currentWindowNumber != WindowNumber.N1T1)
                mpc.MakeActive();
            m_activeMPC = mpc;
            activeChangedByUser = false;
            return mpc;
        }

        public enum WindowNumber
        {
            N1T1,
            N2T1,
            N4T1,
            N4T2,
            N6T1,
            N6T2,
            N8T1,
            N9T1,
            N9T2,
            N10T1,
            N12T1,
            N13T1
        }

        static WindowNumber currentWindowNumber = WindowNumber.N1T1;

        public WindowNumber CurrentWindowNumber
        {
            get { return currentWindowNumber; }
            private set { currentWindowNumber = value; OnCurrentWindowNumberChanged(); } // set如果是public，vs2015会给你来一拳重击
        }

        protected virtual void OnCurrentWindowNumberChanged()
        {
            int lastMCCount = 0;
            for (int i = 0; i < m_activeTLP.Controls.Count; ++i)
            {
                Control c = m_activeTLP.Controls[i];
                if (c is MultiPlayerControl)
                {
                    (c as MultiPlayerControl).MPCPause();
                    lastMCCount++;
                }
            }
            panel2.Controls.Remove(m_activeTLP);
            if (currentWindowNumber != WindowNumber.N1T1)
            {
                btn_wn_n1t1.Image = Resources.n1t1s1;
            }
            else
            {
                m_activeTLP = get_tlp_n1t1();
            }
            if (currentWindowNumber != WindowNumber.N2T1)
            {
                btn_wn_n2t1.Image = Resources.n2t1s1;
            }
            else
            {
                m_activeTLP = get_tlp_n2t1();
            }
            if (currentWindowNumber != WindowNumber.N4T1)
            {
                btn_wn_n4t1.Image = Resources.n4t1s1;
            }
            else
            {
                m_activeTLP = get_tlp_n4t1();
            }
            if (currentWindowNumber != WindowNumber.N4T2)
            {
                btn_wn_n4t2.Image = Resources.n4t2s1;
            }
            else
            {
                m_activeTLP = get_tlp_n4t2();
            }
            if (currentWindowNumber != WindowNumber.N6T1)
            {
                btn_wn_n6t1.Image = Resources.n6t1s1;
            }
            else
            {
                m_activeTLP = get_tlp_n6t1();
            }
            if (currentWindowNumber != WindowNumber.N6T2)
            {
                btn_wn_n6t2.Image = Resources.n6t2s1;
            }
            else
            {
                m_activeTLP = get_tlp_n6t2();
            }
            if (currentWindowNumber != WindowNumber.N8T1)
            {
                btn_wn_n8t1.Image = Resources.n8t1s1;
            }
            else
            {
                m_activeTLP = get_tlp_n8t1();
            }
            if (currentWindowNumber != WindowNumber.N9T1)
            {
                btn_wn_n9t1.Image = Resources.n9t1s1;
            }
            else
            {
                m_activeTLP = get_tlp_n9t1();
            }
            if (currentWindowNumber != WindowNumber.N9T2)
            {
                btn_wn_n9t2.Image = Resources.n9t2s1;
            }
            else
            {
                m_activeTLP = get_tlp_n9t2();
            }
            if (currentWindowNumber != WindowNumber.N10T1)
            {
                btn_wn_n10t1.Image = Resources.n10t1s1;
            }
            else
            {
                m_activeTLP = get_tlp_n10t1();
            }
            if (currentWindowNumber != WindowNumber.N12T1)
            {
                btn_wn_n12t1.Image = Resources.n12t1s1;
            }
            else
            {
                m_activeTLP = get_tlp_n12t1();
            }
            if (currentWindowNumber != WindowNumber.N13T1)
            {
                btn_wn_n13t1.Image = Resources.n13t1s1;
            }
            else
            {
                m_activeTLP = get_tlp_n13t1();
            }
            panel2.Controls.Add(m_activeTLP);
            int thisMCCount = 0;
            if (m_activeTLP.Controls.Contains(mpc1))
            {
                mpc1.MPCResume();
                thisMCCount++;
            }
            else mpc1.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc2))
            {
                mpc2.MPCResume();
                thisMCCount++;
            }
            else mpc2.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc3))
            {
                mpc3.MPCResume();
                thisMCCount++;
            }
            else mpc3.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc4))
            {
                mpc4.MPCResume();
                thisMCCount++;
            }
            else mpc4.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc5))
            {
                mpc5.MPCResume();
                thisMCCount++;
            }
            else mpc5.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc6))
            {
                mpc6.MPCResume();
                thisMCCount++;
            }
            else mpc6.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc7))
            {
                mpc7.MPCResume();
                thisMCCount++;
            }
            else mpc7.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc8))
            {
                mpc8.MPCResume();
                thisMCCount++;
            }
            else mpc8.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc9))
            {
                mpc9.MPCResume();
                thisMCCount++;
            }
            else mpc9.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc10))
            {
                mpc10.MPCResume();
                thisMCCount++;
            }
            else mpc10.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc11))
            {
                mpc11.MPCResume();
                thisMCCount++;
            }
            else mpc11.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc12))
            {
                mpc12.MPCResume();
                thisMCCount++;
            }
            else mpc12.MPCStop();
            if (m_activeTLP.Controls.Contains(mpc13))
            {
                mpc13.MPCResume();
                thisMCCount++;
            }
            else mpc13.MPCStop();
            if (activeChangedByUser) return;
            // 如果用户没有选择过下次打开的画面窗口，则我们需要智能推荐，哈哈
            if (thisMCCount < lastMCCount)
            {
                // 画面变少
                int acInTLP = 1;
                foreach(var c in m_activeTLP.Controls)
                {
                    if (!(c is MultiPlayerControl)) continue;
                    if (c == m_activeMPC) break;
                    acInTLP++;
                }
                if (acInTLP > thisMCCount) m_activeMPC = null; // 如果上次的激活画面被消除，则默认开始从第一个画面开始播放
            }
        }

        private void btn_wn_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender == btn_wn_n1t1)
                CurrentWindowNumber = WindowNumber.N1T1;
            else if (sender == btn_wn_n2t1)
                CurrentWindowNumber = WindowNumber.N2T1;
            else if (sender == btn_wn_n4t1)
                CurrentWindowNumber = WindowNumber.N4T1;
            else if (sender == btn_wn_n4t2)
                CurrentWindowNumber = WindowNumber.N4T2;
            else if (sender == btn_wn_n6t1)
                CurrentWindowNumber = WindowNumber.N6T1;
            else if (sender == btn_wn_n6t2)
                CurrentWindowNumber = WindowNumber.N6T2;
            else if (sender == btn_wn_n8t1)
                CurrentWindowNumber = WindowNumber.N8T1;
            else if (sender == btn_wn_n9t1)
                CurrentWindowNumber = WindowNumber.N9T1;
            else if (sender == btn_wn_n9t2)
                CurrentWindowNumber = WindowNumber.N9T2;
            else if (sender == btn_wn_n10t1)
                CurrentWindowNumber = WindowNumber.N10T1;
            else if (sender == btn_wn_n12t1)
                CurrentWindowNumber = WindowNumber.N12T1;
            else if (sender == btn_wn_n13t1)
                CurrentWindowNumber = WindowNumber.N13T1;
        }

        private void btn_wn_MouseEnter(object sender, EventArgs e)
        {
            if(sender == btn_wn_n1t1)
                btn_wn_n1t1.Image = Resources.n1t1s2;
            else if (sender == btn_wn_n2t1)
                btn_wn_n2t1.Image = Resources.n2t1s2;
            else if (sender == btn_wn_n4t1)
                btn_wn_n4t1.Image = Resources.n4t1s2;
            else if (sender == btn_wn_n4t2)
                btn_wn_n4t2.Image = Resources.n4t2s2;
            else if (sender == btn_wn_n6t1)
                btn_wn_n6t1.Image = Resources.n6t1s2;
            else if (sender == btn_wn_n6t2)
                btn_wn_n6t2.Image = Resources.n6t2s2;
            else if (sender == btn_wn_n8t1)
                btn_wn_n8t1.Image = Resources.n8t1s2;
            else if (sender == btn_wn_n9t1)
                btn_wn_n9t1.Image = Resources.n9t1s2;
            else if (sender == btn_wn_n9t2)
                btn_wn_n9t2.Image = Resources.n9t2s2;
            else if (sender == btn_wn_n10t1)
                btn_wn_n10t1.Image = Resources.n10t1s2;
            else if (sender == btn_wn_n12t1)
                btn_wn_n12t1.Image = Resources.n12t1s2;
            else if (sender == btn_wn_n13t1)
                btn_wn_n13t1.Image = Resources.n13t1s2;
        }

        private void btn_wn_MouseLeave(object sender, EventArgs e)
        {
            if (sender == btn_wn_n1t1 && CurrentWindowNumber != WindowNumber.N1T1)
                btn_wn_n1t1.Image = Resources.n1t1s1;
            else if (sender == btn_wn_n2t1 && CurrentWindowNumber != WindowNumber.N2T1)
                btn_wn_n2t1.Image = Resources.n2t1s1;
            else if (sender == btn_wn_n4t1 && CurrentWindowNumber != WindowNumber.N4T1)
                btn_wn_n4t1.Image = Resources.n4t1s1;
            else if (sender == btn_wn_n4t2 && CurrentWindowNumber != WindowNumber.N4T2)
                btn_wn_n4t2.Image = Resources.n4t2s1;
            else if (sender == btn_wn_n6t1 && CurrentWindowNumber != WindowNumber.N6T1)
                btn_wn_n6t1.Image = Resources.n6t1s1;
            else if (sender == btn_wn_n6t2 && CurrentWindowNumber != WindowNumber.N6T2)
                btn_wn_n6t2.Image = Resources.n6t2s1;
            else if (sender == btn_wn_n8t1 && CurrentWindowNumber != WindowNumber.N8T1)
                btn_wn_n8t1.Image = Resources.n8t1s1;
            else if (sender == btn_wn_n9t1 && CurrentWindowNumber != WindowNumber.N9T1)
                btn_wn_n9t1.Image = Resources.n9t1s1;
            else if (sender == btn_wn_n9t2 && CurrentWindowNumber != WindowNumber.N9T2)
                btn_wn_n9t2.Image = Resources.n9t2s1;
            else if (sender == btn_wn_n10t1 && CurrentWindowNumber != WindowNumber.N10T1)
                btn_wn_n10t1.Image = Resources.n10t1s1;
            else if (sender == btn_wn_n12t1 && CurrentWindowNumber != WindowNumber.N12T1)
                btn_wn_n12t1.Image = Resources.n12t1s1;
            else if (sender == btn_wn_n13t1 && CurrentWindowNumber != WindowNumber.N13T1)
                btn_wn_n13t1.Image = Resources.n13t1s1;
        }
        
        private TableLayoutPanel tlp_n1t1;
        private TableLayoutPanel tlp_n2t1;
        private TableLayoutPanel tlp_n4t1;
        private TableLayoutPanel tlp_n4t2;
        private TableLayoutPanel tlp_n6t1;
        private TableLayoutPanel tlp_n6t2;
        private TableLayoutPanel tlp_n8t1;
        private TableLayoutPanel tlp_n9t1;
        private TableLayoutPanel tlp_n9t2;
        private TableLayoutPanel tlp_n10t1;
        private TableLayoutPanel tlp_n12t1;
        private TableLayoutPanel tlp_n13t1;

        private static TableLayoutPanel m_activeTLP;

        private static MultiPlayerControl mpc1;
        private MultiPlayerControl mpc2;
        private MultiPlayerControl mpc3;
        private MultiPlayerControl mpc4;
        private MultiPlayerControl mpc5;
        private MultiPlayerControl mpc6;
        private MultiPlayerControl mpc7;
        private MultiPlayerControl mpc8;
        private MultiPlayerControl mpc9;
        private MultiPlayerControl mpc10;
        private MultiPlayerControl mpc11;
        private MultiPlayerControl mpc12;
        private MultiPlayerControl mpc13;

        private static MultiPlayerControl m_activeMPC;

        private static EventHandler mpcClick;

        private static bool activeChangedByUser = false;

        private void onMPCClick(object sender, EventArgs e)
        {
            if (CurrentWindowNumber == WindowNumber.N1T1) return;
            activeChangedByUser = true;
            if(m_activeMPC != null)
                m_activeMPC.MPCActive = false;
            m_activeMPC = sender as MultiPlayerControl;
            m_activeMPC.MPCActive = true;
        }

        private void MultiPlayerPanel_Load(object sender, EventArgs e)
        {
            mpcClick = new EventHandler(onMPCClick);

            prepareMPC(ref mpc1, "mpc1");
            prepareMPC(ref mpc2, "mpc2");
            prepareMPC(ref mpc3, "mpc3");
            prepareMPC(ref mpc4, "mpc4");
            prepareMPC(ref mpc5, "mpc5");
            prepareMPC(ref mpc6, "mpc6");
            prepareMPC(ref mpc7, "mpc7");
            prepareMPC(ref mpc8, "mpc8");
            prepareMPC(ref mpc9, "mpc9");
            prepareMPC(ref mpc10, "mpc10");
            prepareMPC(ref mpc11, "mpc11");
            prepareMPC(ref mpc12, "mpc12");
            prepareMPC(ref mpc13, "mpc13");
            
            m_activeTLP = get_tlp_n1t1();
            panel2.Controls.Add(m_activeTLP);
        }

        private void prepareMPC(ref MultiPlayerControl mpc, String name)
        {
            mpc = new MultiPlayerControl();
            mpc.Name = name;
            mpc.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            mpc.Padding = new System.Windows.Forms.Padding(3);
            mpc.MPCActived += mpcClick;
            mpc.MPCEnterFullScreen += Mpc_MPCEnterFullScreen;
            mpc.MPCLeaveFullScreen += Mpc_MPCLeaveFullScreen;
        }

        private TableLayoutPanel get_tlp_n1t1()
        {
            if (tlp_n1t1 == null)
                prepareTLP(ref tlp_n1t1, "tlp_n1t1", 1, 1);
            else
                tlp_n1t1.Controls.Clear();
            tlp_n1t1.Controls.Add(mpc1, 0, 0);
            prepareMPC(tlp_n1t1, mpc1);
            return tlp_n1t1;
        }

        private TableLayoutPanel get_tlp_n2t1()
        {
            if (tlp_n2t1 == null)
                prepareTLP(ref tlp_n2t1, "tlp_n2t1", 1, 2);
            else
                tlp_n2t1.Controls.Clear();
            tlp_n2t1.Controls.Add(mpc1, 0, 0);
            tlp_n2t1.Controls.Add(mpc2, 1, 0);
            prepareMPC(tlp_n2t1, mpc1, mpc2);
            return tlp_n2t1;
        }

        private TableLayoutPanel get_tlp_n4t1()
        {
            if (tlp_n4t1 == null)
                prepareTLP(ref tlp_n4t1, "tlp_n4t1", 2, 2);
            else
                tlp_n4t1.Controls.Clear();
            tlp_n4t1.Controls.Add(mpc1, 0, 0);
            tlp_n4t1.Controls.Add(mpc2, 1, 0);
            tlp_n4t1.Controls.Add(mpc3, 0, 1);
            tlp_n4t1.Controls.Add(mpc4, 1, 1);
            prepareMPC(tlp_n4t1, mpc1, mpc2, mpc3, mpc4);
            return tlp_n4t1;
        }

        private TableLayoutPanel get_tlp_n4t2()
        {
            if (tlp_n4t2 == null)
                prepareTLP(ref tlp_n4t2, "tlp_n4t2", 3, 3);
            else
                tlp_n4t2.Controls.Clear();
            tlp_n4t2.Controls.Add(mpc1, 0, 0);
            tlp_n4t2.SetRowSpan(mpc1, 3);
            tlp_n4t2.SetColumnSpan(mpc1, 2);
            tlp_n4t2.Controls.Add(mpc2, 2, 0);
            tlp_n4t2.Controls.Add(mpc3, 2, 1);
            tlp_n4t2.Controls.Add(mpc4, 2, 2);
            prepareMPC(tlp_n4t2, mpc2, mpc3, mpc4);
            return tlp_n4t2;
        }

        private TableLayoutPanel get_tlp_n6t1()
        {
            if (tlp_n6t1 == null)
                prepareTLP(ref tlp_n6t1, "tlp_n6t1", 3, 3);
            else
                tlp_n6t1.Controls.Clear();
            tlp_n6t1.Controls.Add(mpc1, 0, 0);
            tlp_n6t1.SetRowSpan(mpc1, 2);
            tlp_n6t1.SetColumnSpan(mpc1, 2);
            tlp_n6t1.Controls.Add(mpc2, 2, 0);
            tlp_n6t1.Controls.Add(mpc3, 2, 1);
            tlp_n6t1.Controls.Add(mpc4, 0, 2);
            tlp_n6t1.Controls.Add(mpc5, 1, 2);
            tlp_n6t1.Controls.Add(mpc6, 2, 2);
            prepareMPC(tlp_n6t1, mpc2, mpc3, mpc4, mpc5, mpc6);
            return tlp_n6t1;
        }

        private TableLayoutPanel get_tlp_n6t2()
        {
            if (tlp_n6t2 == null)
                prepareTLP(ref tlp_n6t2, "tlp_n6t2", 2, 3);
            else
                tlp_n6t2.Controls.Clear();
            tlp_n6t2.Controls.Add(mpc1, 0, 0);
            tlp_n6t2.Controls.Add(mpc2, 0, 1);
            tlp_n6t2.Controls.Add(mpc3, 1, 0);
            tlp_n6t2.Controls.Add(mpc4, 1, 1);
            tlp_n6t2.Controls.Add(mpc5, 2, 0);
            tlp_n6t2.Controls.Add(mpc6, 2, 1);
            prepareMPC(tlp_n6t2, mpc1, mpc2, mpc3, mpc4, mpc5, mpc6);
            return tlp_n6t2;
        }

        private TableLayoutPanel get_tlp_n8t1()
        {
            if (tlp_n8t1 == null)
                prepareTLP(ref tlp_n8t1, "tlp_n8t1", 4, 4);
            else
                tlp_n8t1.Controls.Clear();
            tlp_n8t1.Controls.Add(mpc1, 0, 0);
            tlp_n8t1.SetRowSpan(mpc1, 3);
            tlp_n8t1.SetColumnSpan(mpc1, 3);
            tlp_n8t1.Controls.Add(mpc2, 2, 0);
            tlp_n8t1.Controls.Add(mpc3, 2, 1);
            tlp_n8t1.Controls.Add(mpc4, 2, 2);
            tlp_n8t1.Controls.Add(mpc5, 0, 2);
            tlp_n8t1.Controls.Add(mpc6, 1, 2);
            tlp_n8t1.Controls.Add(mpc7, 2, 2);
            tlp_n8t1.Controls.Add(mpc8, 3, 2);
            prepareMPC(tlp_n8t1, mpc2, mpc3, mpc4, mpc5, mpc6, mpc7, mpc8);
            return tlp_n8t1;
        }

        private TableLayoutPanel get_tlp_n9t1()
        {
            if (tlp_n9t1 == null)
                prepareTLP(ref tlp_n9t1, "tlp_n9t1", 3, 4);
            else
                tlp_n9t1.Controls.Clear();
            tlp_n9t1.Controls.Add(mpc1, 0, 0);
            tlp_n9t1.SetRowSpan(mpc1, 2);
            tlp_n9t1.SetColumnSpan(mpc1, 2);
            tlp_n9t1.Controls.Add(mpc2, 2, 0);
            tlp_n9t1.Controls.Add(mpc3, 3, 0);
            tlp_n9t1.Controls.Add(mpc4, 2, 1);
            tlp_n9t1.Controls.Add(mpc5, 3, 1);
            tlp_n9t1.Controls.Add(mpc6, 0, 2);
            tlp_n9t1.Controls.Add(mpc7, 1, 2);
            tlp_n9t1.Controls.Add(mpc8, 2, 2);
            tlp_n9t1.Controls.Add(mpc9, 3, 2);
            prepareMPC(tlp_n9t1, mpc2, mpc3, mpc4, mpc5, mpc6, mpc7, mpc8, mpc9);
            return tlp_n9t1;
        }

        private TableLayoutPanel get_tlp_n9t2()
        {
            if (tlp_n9t2 == null)
                prepareTLP(ref tlp_n9t2, "tlp_n9t2", 3, 3);
            else
                tlp_n9t2.Controls.Clear();
            tlp_n9t2.Controls.Add(mpc1, 0, 0);
            tlp_n9t2.Controls.Add(mpc2, 1, 0);
            tlp_n9t2.Controls.Add(mpc3, 2, 0);
            tlp_n9t2.Controls.Add(mpc4, 0, 1);
            tlp_n9t2.Controls.Add(mpc5, 1, 1);
            tlp_n9t2.Controls.Add(mpc6, 2, 1);
            tlp_n9t2.Controls.Add(mpc7, 0, 2);
            tlp_n9t2.Controls.Add(mpc8, 1, 2);
            tlp_n9t2.Controls.Add(mpc9, 2, 2);
            prepareMPC(tlp_n9t2, mpc1, mpc2, mpc3, mpc4, mpc5, mpc6, mpc7, mpc8, mpc9);
            return tlp_n9t2;
        }

        private TableLayoutPanel get_tlp_n10t1()
        {
            if (tlp_n10t1 == null)
                prepareTLP(ref tlp_n10t1, "tlp_n10t1", 3, 4);
            else
                tlp_n10t1.Controls.Clear();
            tlp_n10t1.Controls.Add(mpc1, 0, 0);
            tlp_n10t1.SetColumnSpan(mpc1, 2);
            tlp_n10t1.Controls.Add(mpc2, 2, 0);
            tlp_n10t1.SetColumnSpan(mpc2, 2);
            tlp_n10t1.Controls.Add(mpc3, 0, 1);
            tlp_n10t1.Controls.Add(mpc4, 1, 1);
            tlp_n10t1.Controls.Add(mpc5, 2, 1);
            tlp_n10t1.Controls.Add(mpc6, 3, 1);
            tlp_n10t1.Controls.Add(mpc7, 0, 2);
            tlp_n10t1.Controls.Add(mpc8, 1, 2);
            tlp_n10t1.Controls.Add(mpc9, 2, 2);
            tlp_n10t1.Controls.Add(mpc10, 3, 2);
            prepareMPC(tlp_n10t1, mpc3, mpc4, mpc5, mpc6, mpc7, mpc8, mpc9, mpc10);
            return tlp_n10t1;
        }

        private TableLayoutPanel get_tlp_n12t1()
        {
            if (tlp_n12t1 == null)
                prepareTLP(ref tlp_n12t1, "tlp_n12t1", 3, 4);
            else
                tlp_n12t1.Controls.Clear();
            tlp_n12t1.Controls.Add(mpc1, 0, 0);
            tlp_n12t1.Controls.Add(mpc2, 1, 0);
            tlp_n12t1.Controls.Add(mpc3, 2, 0);
            tlp_n12t1.Controls.Add(mpc4, 3, 0);
            tlp_n12t1.Controls.Add(mpc5, 0, 1);
            tlp_n12t1.Controls.Add(mpc6, 1, 1);
            tlp_n12t1.Controls.Add(mpc7, 2, 1);
            tlp_n12t1.Controls.Add(mpc8, 3, 1);
            tlp_n12t1.Controls.Add(mpc9, 0, 2);
            tlp_n12t1.Controls.Add(mpc10, 1, 2);
            tlp_n12t1.Controls.Add(mpc11, 2, 2);
            tlp_n12t1.Controls.Add(mpc12, 3, 2);
            prepareMPC(tlp_n12t1, mpc1, mpc2, mpc3, mpc4, mpc5, mpc6, mpc7, mpc8, mpc9, mpc10, mpc11, mpc12);
            return tlp_n12t1;
        }

        private TableLayoutPanel get_tlp_n13t1()
        {
            if (tlp_n13t1 == null)
                prepareTLP(ref tlp_n13t1, "tlp_n13t1", 4, 4);
            else
                tlp_n13t1.Controls.Clear();
            tlp_n13t1.Controls.Add(mpc1, 0, 0);
            tlp_n13t1.Controls.Add(mpc2, 1, 0);
            tlp_n13t1.Controls.Add(mpc3, 2, 0);
            tlp_n13t1.Controls.Add(mpc4, 3, 0);
            tlp_n13t1.Controls.Add(mpc5, 0, 1);
            tlp_n13t1.Controls.Add(mpc6, 1, 1);
            tlp_n13t1.SetRowSpan(mpc6, 2);
            tlp_n13t1.SetColumnSpan(mpc6, 2);
            tlp_n13t1.Controls.Add(mpc7, 3, 1);
            tlp_n13t1.Controls.Add(mpc8, 0, 2);
            tlp_n13t1.Controls.Add(mpc9, 3, 2);
            tlp_n13t1.Controls.Add(mpc10, 0, 3);
            tlp_n13t1.Controls.Add(mpc11, 1, 3);
            tlp_n13t1.Controls.Add(mpc12, 2, 3);
            tlp_n13t1.Controls.Add(mpc13, 3, 3);
            prepareMPC(tlp_n13t1, mpc1, mpc2, mpc3, mpc4, mpc5, mpc7, mpc8, mpc9, mpc10, mpc11, mpc12);
            return tlp_n13t1;
        }

        private void prepareTLP(ref TableLayoutPanel tlp, String name, int row, int col)
        {
            tlp = new TableLayoutPanel();
            tlp.Name = name;
            tlp.ColumnCount = col;
            tlp.RowCount = row;
            for(int i = 0; i < col; ++i)
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1f / col * 100));
            for (int i = 0; i < row; ++i)
                tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 1f / row * 100));
            tlp.Dock = DockStyle.Fill;
        }

        private void prepareMPC(TableLayoutPanel tlp, params MultiPlayerControl[] mpcs)
        {
            foreach (MultiPlayerControl mc in mpcs)
            {
                tlp.SetRowSpan(mc, 1);
                tlp.SetColumnSpan(mc, 1);
            }
        }
    }
}
