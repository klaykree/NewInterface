using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Microsoft.Win32;

namespace NewUI
{
    public partial class UI : Form
    {
        // Structure contain information about low-level keyboard input event
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            /// <summary>
            /// Specifies the x-coordinate of the point. 
            /// </summary>
            public int x;
            /// <summary>
            /// Specifies the y-coordinate of the point. 
            /// </summary>
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public int mouseData; // be careful, this must be ints, not uints (was wrong before I changed it...). regards, cmew.
            public int flags;
            public int time;
            public UIntPtr dwExtraInfo;
        }

        struct MouseButtonData
        {
            //The button that was pressed or released
            public MouseButtons oButton;
            //True if the button went down, false if it went up
            public bool bDown;
        }

        //System level functions to be used for hook and unhook keyboard input
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        /*[DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);
        [DllImport("msvcrt.dll", SetLastError = false)]
        static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);*/

        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. 
        /// </summary>
        private const int WM_MOUSEMOVE = 0x200;
        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button 
        /// </summary>
        private const int WM_LBUTTONDOWN = 0x201;
        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        private const int WM_RBUTTONDOWN = 0x204;
        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button 
        /// </summary>
        private const int WM_MBUTTONDOWN = 0x207;
        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button 
        /// </summary>
        private const int WM_LBUTTONUP = 0x202;
        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
        /// </summary>
        private const int WM_RBUTTONUP = 0x205;
        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button 
        /// </summary>
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_XBUTTONDOWN = 0x020B;
        private const int WM_XBUTTONUP = 0x020C;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        //Declaring Global objects
        private IntPtr ptrKbdHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        private IntPtr ptrMouseHook;
        private LowLevelKeyboardProc objMouseProcess;

        List<Clickable> m_oQuickSlots;
        List<Point> m_oPoints;

        List<Point> m_oCurrentCircle = new List<Point>();

        int m_iCirclesMade = 0;

        bool m_bShowingApps = false;
        bool m_bShowingClock = false;

        Timer m_kTime = new Timer();
        float m_fDeltaTime = 0f;

        ContextMenu m_oTrayMenu;
        NotifyIcon m_oTrayIcon;

        XDocument m_XDoc;

        string m_sFileDir;

        bool m_bCaptureAppkey = false;
        bool m_bCaptureTimekey = false;

        int m_oAppKey;
        int m_oTimeKey;
        bool m_bDisableAppKey;
        bool m_bDisableTimeKey;

        bool m_bDisabled = false;
        bool m_bAllowDisableChange = true;

        MenuItem m_oDisabledMenuItem;

        bool m_bAllowStartupChange = true;
        bool m_bAllowDisableKeyChange = true;
        bool m_bAllowRadialChange = true;
        
        Clock m_oClock;

        double m_dX = 0;
        double m_dY = 0;

        public UI()
        {
            InitializeComponent();

            //Create the array of buttons that will start applications
            m_oQuickSlots = new List<Clickable>();
            //Create the array of points that the buttons will move to
            m_oPoints = new List<Point>();

            //Create the system tray right click menu
            m_oTrayMenu = new ContextMenu();
            m_oTrayMenu.MenuItems.Add("Show", LeftClickSystemTray);
            m_oDisabledMenuItem = m_oTrayMenu.MenuItems.Add("Disabled", DisableEnable);
            m_oTrayMenu.MenuItems.Add("Exit", ExitNI);
            
            //Makes the first one bold
            m_oTrayMenu.MenuItems[0].DefaultItem = true;

            //Create the system tray icon
            m_oTrayIcon = new NotifyIcon();
            m_oTrayIcon.Text = "NI";
            m_oTrayIcon.Icon = Properties.Resources.NIIconSmall;
            //Add the right click menu to the icon
            m_oTrayIcon.ContextMenu = m_oTrayMenu;
            m_oTrayIcon.Visible = true;

            //Add left click function to system tray
            //m_oTrayIcon.Click += new System.EventHandler(this.LeftClickSystemTray);
            m_oTrayIcon.MouseClick += new MouseEventHandler(this.LeftClickSystemTrayEvent);

            m_fDeltaTime = 1f / 40f;

            m_kTime.Interval = (int)(m_fDeltaTime * 1000f); //40 fps
            m_kTime.Tick += new EventHandler(Tick);
            m_kTime.Start();

            m_oClock = new Clock();

            //Load or create the xml
            CreateOrLoadXML();

            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule; //Get Current Module
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey); //Assign callback function each time keyboard process
            ptrKbdHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0); //Setting Hook of Keyboard Process for current module

            objMouseProcess = new LowLevelKeyboardProc(captureMouse);
            ptrMouseHook = SetWindowsHookEx(14, objMouseProcess, GetModuleHandle(objCurrentModule.ModuleName), 0); //Setting Hook of mouse Process for current module
        }

        void ExitNI(object sender, EventArgs e)
        {
            Show();
            Close();
        }

        void DisableEnable(object sender, EventArgs e)
        {
            if (m_bAllowDisableChange == true)
            {
                m_bAllowDisableChange = false;

                if (m_bDisabled == true)
                {
                    m_bDisabled = false;
                    m_oDisabledMenuItem.Checked = false;
                    DisabledBox.Checked = false;
                }
                else
                {
                    m_bDisabled = true;
                    m_oDisabledMenuItem.Checked = true;
                    DisabledBox.Checked = true;
                }
            }
            else
            {
                m_bAllowDisableChange = true;
            }
        }

        void LeftClickSystemTray(object sender, EventArgs e)
        {
            Show();

            //Makes the window be ontop of everything
            TopMost = true;
            TopMost = false;
        }

        void LeftClickSystemTrayEvent(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                LeftClickSystemTray(sender, e);
            }
        }

        private void UI_Load(object sender, EventArgs e)
        {
        }

        void Tick(object o, EventArgs e)
        {
            if(!m_bDisabled)
            {
                MovingApps();
                MovingClock();
            }
        }

        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                if(m_bCaptureAppkey)
                {
                    if (objKeyInfo.key == Keys.Escape)
                    {
                        objKeyInfo.key = Keys.F24;
                        SetAppKey(objKeyInfo.key);
                        m_oAppKey = 0;
                    }
                    else
                    {
                        SetAppKey(objKeyInfo.key);
                        m_oAppKey = (int)objKeyInfo.key;
                    }

                    //return CallNextHookEx(ptrHook, nCode, wp, lp);
                }

                if (m_bCaptureTimekey)
                {
                    if (objKeyInfo.key == Keys.Escape)
                    {
                        objKeyInfo.key = Keys.F24;
                        SetTimeKey(objKeyInfo.key);
                        m_oTimeKey = 0;
                    }
                    else
                    {
                        SetTimeKey(objKeyInfo.key);
                        m_oTimeKey = (int)objKeyInfo.key;
                    }

                    //return CallNextHookEx(ptrHook, nCode, wp, lp);
                }
                
                bool bPressedAppkey = false;
                bool bPressedTimekey = false;
                
                //if (objKeyInfo.key == m_oAppKey && m_bDisabled == false)
                if (objKeyInfo.key == (Keys)m_oAppKey && m_bDisabled == false)
                {
                    if ((wp == (IntPtr)WM_KEYDOWN || wp == (IntPtr)WM_SYSKEYDOWN) && m_bShowingApps == false)
                    {
                        StartShowing();
                        bPressedAppkey = true;
                    }
                    else if (wp == (IntPtr)WM_KEYUP || wp == (IntPtr)WM_SYSKEYUP)
                    {
                        StopShowing();
                        bPressedAppkey = true;
                    }

                    if (m_bShowingApps)
                    {
                        bPressedAppkey = true;
                    }
                }

                if (objKeyInfo.key == (Keys)m_oTimeKey && m_bDisabled == false)
                {
                    if ((wp == (IntPtr)WM_KEYDOWN || wp == (IntPtr)WM_SYSKEYDOWN) && m_bShowingClock == false)
                    {
                        StartShowingClock();
                        bPressedTimekey = true;
                    }
                    else if (wp == (IntPtr)WM_KEYUP || wp == (IntPtr)WM_SYSKEYUP)
                    {
                        StopShowingClock();
                        bPressedTimekey = true;
                    }

                    if (m_bShowingClock)
                    {
                        bPressedTimekey = true;
                    }
                }

                if ( (m_bDisableAppKey && bPressedAppkey) || (m_bDisableTimeKey && bPressedTimekey) )
                {
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(ptrKbdHook, nCode, wp, lp);
        }

        MouseButtonData AnyMouseButtonPressed(IntPtr wp, int MouseData)
        {
            MouseButtonData MBData = new MouseButtonData();

            switch ((int)wp)
            {
                case WM_LBUTTONDOWN:
                    MBData.oButton = System.Windows.Forms.MouseButtons.None;
                    MBData.bDown = true;
                    break;
                case WM_RBUTTONDOWN:
                    MBData.oButton = System.Windows.Forms.MouseButtons.Right;
                    MBData.bDown = true;
                    break;
                case WM_MBUTTONDOWN:
                    MBData.oButton = System.Windows.Forms.MouseButtons.Middle;
                    MBData.bDown = true;
                    break;
                case WM_XBUTTONDOWN:
                    if (MouseData == 65536)
                    {
                        MBData.oButton = System.Windows.Forms.MouseButtons.XButton1;
                        MBData.bDown = true;
                    }
                    else
                    {
                        MBData.oButton = System.Windows.Forms.MouseButtons.XButton2;
                        MBData.bDown = true;
                    }
                    break;
                case WM_LBUTTONUP:
                    MBData.oButton = System.Windows.Forms.MouseButtons.None;
                    MBData.bDown = false;
                    break;
                case WM_RBUTTONUP:
                    MBData.oButton = System.Windows.Forms.MouseButtons.Right;
                    MBData.bDown = false;
                    break;
                case WM_MBUTTONUP:
                    MBData.oButton = System.Windows.Forms.MouseButtons.Middle;
                    MBData.bDown = false;
                    break;
                case WM_XBUTTONUP:
                    if (MouseData == 65536)
                    {
                        MBData.oButton = System.Windows.Forms.MouseButtons.XButton1;
                        MBData.bDown = false;
                    }
                    else
                    {
                        MBData.oButton = System.Windows.Forms.MouseButtons.XButton2;
                        MBData.bDown = false;
                    }
                    break;
                default:
                    MBData.oButton = System.Windows.Forms.MouseButtons.None;
                    MBData.bDown = false;
                    break;
            }

            return MBData;
        }

        private IntPtr captureMouse(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT objMouseInfo = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(MSLLHOOKSTRUCT));

                MouseButtonData oButtonData = AnyMouseButtonPressed(wp, objMouseInfo.mouseData);

                if (oButtonData.oButton == System.Windows.Forms.MouseButtons.None)
                {
                    return CallNextHookEx(ptrMouseHook, nCode, wp, lp);
                }

                if (m_bCaptureAppkey)
                {
                    SetAppKey(oButtonData.oButton);
                    m_oAppKey = (int)oButtonData.oButton;

                    //return CallNextHookEx(ptrHook, nCode, wp, lp);
                }

                if (m_bCaptureTimekey)
                {
                    SetTimeKey(oButtonData.oButton);
                    m_oTimeKey = (int)oButtonData.oButton;

                    //return CallNextHookEx(ptrHook, nCode, wp, lp);
                }

                bool bPressedAppkey = false;
                bool bPressedTimekey = false;

                if ((int)oButtonData.oButton == m_oAppKey && m_bDisabled == false)
                {
                    if (oButtonData.bDown == true && m_bShowingApps == false)
                    {
                        StartShowing();
                        bPressedAppkey = true;
                    }
                    else if (oButtonData.bDown == false)
                    {
                        StopShowing();
                        bPressedAppkey = true;
                    }

                    if (m_bShowingApps)
                    {
                        bPressedAppkey = true;
                    }
                }

                if ((int)oButtonData.oButton == m_oTimeKey && m_bDisabled == false)
                {
                    if (oButtonData.bDown == true && m_bShowingClock == false)
                    {
                        StartShowingClock();
                        bPressedTimekey = true;
                    }
                    else if (oButtonData.bDown == false)
                    {
                        StopShowingClock();
                        bPressedTimekey = true;
                    }

                    if (m_bShowingClock)
                    {
                        bPressedTimekey = true;
                    }
                }

                if ((m_bDisableAppKey && bPressedAppkey) || (m_bDisableTimeKey && bPressedTimekey))
                {
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(ptrMouseHook, nCode, wp, lp);
        }

        void SetAppKey(Keys a_oKey)
        {
            m_XDoc.Element("NI").Element("Options").Element("AppHotkey").Value = "K" + ((int)a_oKey).ToString();
            //m_XDoc["NI"]["Options"]["AppHotkey"].InnerText = ((int)a_oKeys).ToString();

            m_XDoc.Save(m_sFileDir);

            if (a_oKey == Keys.F24)
                AppHotKey.Text = "None";
            else
                AppHotKey.Text = a_oKey.ToString();

            m_bCaptureAppkey = false;
        }

        void SetAppKey(MouseButtons a_oButton)
        {
            m_XDoc.Element("NI").Element("Options").Element("AppHotkey").Value = "M" + ((int)a_oButton).ToString();
            //m_XDoc["NI"]["Options"]["AppHotkey"].InnerText = ((int)a_oKeys).ToString();

            m_XDoc.Save(m_sFileDir);

            AppHotKey.Text = a_oButton.ToString();

            m_bCaptureAppkey = false;
        }

        void SetTimeKey(Keys a_oKey)
        {
            m_XDoc.Element("NI").Element("Options").Element("TimeHotkey").Value = "K" + ((int)a_oKey).ToString();
            //m_XDoc["NI"]["Options"]["TimeHotkey"].InnerText = ((int)a_oKeys).ToString();

            m_XDoc.Save(m_sFileDir);

            if (a_oKey == Keys.F24)
                TimeHotKey.Text = "None";
            else
                TimeHotKey.Text = a_oKey.ToString();

            m_bCaptureTimekey = false;
        }

        void SetTimeKey(MouseButtons a_oButton)
        {
            m_XDoc.Element("NI").Element("Options").Element("TimeHotkey").Value = "M" + ((int)a_oButton).ToString();
            //m_XDoc["NI"]["Options"]["TimeHotkey"].InnerText = ((int)a_oKeys).ToString();

            m_XDoc.Save(m_sFileDir);

            TimeHotKey.Text = a_oButton.ToString();

            m_bCaptureTimekey = false;
        }

        void StartShowing()
        {
            m_bShowingApps = true;

            for (int i = 0 ; i < m_oQuickSlots.Count ; ++i)
            {
                m_oQuickSlots[i].Show();
                m_oQuickSlots[i].TopMost = true;
                //Place the apps at the center of the cursor
                Point StartPos = new Point(MousePosition.X - (m_oQuickSlots[i].Width >> 1), MousePosition.Y - (m_oQuickSlots[i].Height >> 1));
                m_oQuickSlots[i].Location = StartPos;
                m_oQuickSlots[i].m_XPos = StartPos.X;
                m_oQuickSlots[i].m_YPos = StartPos.Y;
                m_oQuickSlots[i].Opacity = 0.0;
            }

            //The points the apps will move to in the update
            m_oPoints = new List<Point>();
            //The currently created circle (finished or not)
            m_oCurrentCircle = new List<Point>();

            //The number of points that have been made
            int iSlotCount = 0;

            //The distance from the mouse to the first circle of icons
            int iDistance = Distance.Value;

            m_iCirclesMade = 0;

            //Create 4 points around the cursor
            //Top
            m_oCurrentCircle.Add(new Point(MousePosition.X, MousePosition.Y - iDistance));
            ++iSlotCount;
            //Right
            m_oCurrentCircle.Add(new Point(MousePosition.X + iDistance, MousePosition.Y));
            ++iSlotCount;
            //Bottom
            m_oCurrentCircle.Add(new Point(MousePosition.X, MousePosition.Y + iDistance));
            ++iSlotCount;
            //Left
            m_oCurrentCircle.Add(new Point(MousePosition.X - iDistance, MousePosition.Y));
            ++iSlotCount;

            FinishCircle(ref m_oCurrentCircle, ref iSlotCount, iDistance);
            AddPointsToMemberList(ref m_oCurrentCircle);

            //Check if enough points have been created
            while(iSlotCount < m_oQuickSlots.Count)
            {
                MakeNextCircle(ref m_oCurrentCircle, ref iSlotCount, iDistance);
                FinishCircle(ref m_oCurrentCircle, ref iSlotCount, iDistance);

                AddPointsToMemberList(ref m_oCurrentCircle);
            }
        }

        void StopShowing()
        {
            m_bShowingApps = false;

            for (int i = 0; i < m_oQuickSlots.Count; ++i)
            {
                m_oQuickSlots[i].Hide();
                m_oQuickSlots[i].Opacity = 0;
            }
        }

        void StartShowingClock()
        {
            m_bShowingClock = true;

            m_oClock.Show();
            m_oClock.TopMost = true;
            m_oClock.Opacity = 0.0;
            m_oClock.ResetDate();
        }

        void StopShowingClock()
        {
            m_bShowingClock = false;

            m_oClock.Opacity = 0.0;
            m_oClock.Hide();
        }

        void MovingClock()
        {
            if(m_bShowingClock)
            {
                Point NewLoc = new Point(MousePosition.X, MousePosition.Y);

                if (m_dX < -6.28318531)
                {
                    m_dX = 0;
                }
                if (m_dY < -6.28318531)
                {
                    m_dY = 0;
                }

                //NewLoc.X += (int)(Math.Cos((double)Runtime.Milliseconds) * 50);
                //NewLoc.Y += (int)(Math.Sin((double)Runtime.Milliseconds) * 50);

                m_dX -= m_fDeltaTime;
                m_dY -= m_fDeltaTime;

                NewLoc.X += (int)(Math.Cos(m_dX) * 120);
                NewLoc.Y += (int)(Math.Sin(m_dY) * 80);

                NewLoc.X -= m_oClock.Width / 2;
                NewLoc.Y -= m_oClock.Height / 2;

                m_oClock.Location = NewLoc;

                m_oClock.Opacity += 0.06;
            }
        }

        void MovingApps()
        {
            if (m_bShowingApps)
            {
                Point oDir = new Point();
                Point oNewLoc;

                bool bFinished = true;

                for (int i = 0 ; i < m_oQuickSlots.Count ; ++i)
                {
                    //Get the direction from the current position to the desired position
                    /*oDir.X = m_oPoints[i].X - (m_oQuickSlots[i].Location.X + (m_oQuickSlots[i].Width >> 1));
                    oDir.Y = m_oPoints[i].Y - (m_oQuickSlots[i].Location.Y + (m_oQuickSlots[i].Height >> 1));

                    int iXDisToMove;
                    int iYDisToMove;

                    if(oDir.X > Speed.Value || oDir.X < -Speed.Value)
                    {
                        iXDisToMove = oDir.X / Speed.Value;
                    }
                    else
                    {
                        if (oDir.X > 0)
                        {
                            iXDisToMove = 1;
                        }
                        else if (oDir.X < 0)
                        {
                            iXDisToMove = -1;
                        }
                        else
                        {
                            iXDisToMove = 0;
                        }
                    }

                    if(oDir.Y > Speed.Value || oDir.Y < -Speed.Value)
                    {
                        iYDisToMove = oDir.Y / Speed.Value;
                    }
                    else
                    {
                        if (oDir.Y > 0)
                        {
                            iYDisToMove = 1;
                        }
                        else if (oDir.Y < 0)
                        {
                            iYDisToMove = -1;
                        }
                        else
                        {
                            iYDisToMove = 0;
                        }
                    }

                    oNewLoc = m_oQuickSlots[i].Location;
                    oNewLoc.X += iXDisToMove;
                    oNewLoc.Y += iYDisToMove;
                    m_oQuickSlots[i].Location = oNewLoc;*/

                    //m_oQuickSlots[i].m_XPos = Lerp(m_oQuickSlots[i].m_XPos, m_oPoints[i].X - (m_oQuickSlots[i].Width >> 1), Speed.Value / 20.0f);
                    //m_oQuickSlots[i].m_YPos = Lerp(m_oQuickSlots[i].m_YPos, m_oPoints[i].Y - (m_oQuickSlots[i].Height >> 1), Speed.Value / 20.0f);
                    m_oQuickSlots[i].m_XPos = Lerp(m_oQuickSlots[i].m_XPos, m_oPoints[i].X - (m_oQuickSlots[i].Width >> 1), m_fDeltaTime * Speed.Value);
                    m_oQuickSlots[i].m_YPos = Lerp(m_oQuickSlots[i].m_YPos, m_oPoints[i].Y - (m_oQuickSlots[i].Height >> 1), m_fDeltaTime * Speed.Value);
                    m_oQuickSlots[i].Location = new Point(Convert.ToInt32(m_oQuickSlots[i].m_XPos), Convert.ToInt32(m_oQuickSlots[i].m_YPos));

                    m_oQuickSlots[i].Opacity += 0.09;

                    if (m_oQuickSlots[i].Location == m_oPoints[i])
                    {
                        m_oQuickSlots[i].Opacity = 1.0;
                    }
                    else
                    {
                        bFinished = false;
                    }
                }

                if (bFinished)
                {
                    m_bShowingApps = false;
                }
            }
        }

        float Lerp(float v1, float v2, float t)
        {
            return v1 + (v2 - v1) * t;
        }

        private void StartBtn_Click(object sender, EventArgs e)
        {
            Hide();
        }

        int Clamp(int value, int iClampValue)
        {
            int result = value;
            if (value > 0)
                result = iClampValue;
            else if (value < 0)
                result = -iClampValue;
            else if (value == 0)
                result = 0;

            return result;
        }

        void AddApp(string a_sFilePath, bool a_bAddToXml)
        {
            //if (Directory.Exists(a_sFilePath) || File.Exists(a_sFilePath))
            {
                //Create a new clickable item
                Clickable oNewQuickslot = new Clickable();
                //Add it to the list
                m_oQuickSlots.Add(oNewQuickslot);

                if(Directory.Exists(a_sFilePath) || File.Exists(a_sFilePath))
                {
                    FileAttributes oAttributes = File.GetAttributes(a_sFilePath);

                    if(oAttributes == FileAttributes.Directory || oAttributes == (FileAttributes.Directory | FileAttributes.ReadOnly))
                    {
                        Bitmap newImage = new Bitmap(Properties.Resources.Folder, new System.Drawing.Size(32, 32));

                        oNewQuickslot.SetImage(ref newImage);
                    }
                    else
                    {
                        Bitmap BGImage = Icon.ExtractAssociatedIcon(a_sFilePath).ToBitmap();

                        oNewQuickslot.SetImage(ref BGImage);
                    }
                }
                else
                {
                    Bitmap newImage = new Bitmap(Properties.Resources.NotFound, new System.Drawing.Size(32, 32));

                    oNewQuickslot.SetImage(ref newImage);

                    MessageBox.Show(a_sFilePath + " not found", "File or folder not found");
                }

                string sAppName = a_sFilePath.Remove(0, a_sFilePath.LastIndexOf("\\") + 1);

                oNewQuickslot.m_oAppName = sAppName;

                oNewQuickslot.SetMouseover();

                a_sFilePath = a_sFilePath.Remove(a_sFilePath.LastIndexOf("\\") + 1);
                //Set the directory for the quick slot
                oNewQuickslot.m_oAppDir = a_sFilePath;

                ApplicationList.DataSource = null;
                ApplicationList.DataSource = m_oQuickSlots;
                ApplicationList.DisplayMember = "AppName";

                if (a_bAddToXml)
                {
                    //Get the root element from the xml file
                    XElement Root = m_XDoc.Element("NI");

                    //Get the quickslot element from the xml file
                    Root = Root.Element("QuickSlots");

                    XElement NewElement = new XElement("Slot",
                        new XAttribute("Name", sAppName));

                    Root.Add(NewElement);

                    NewElement.Add(new XAttribute("Path", a_sFilePath));

                    m_XDoc.Save(m_sFileDir);
                }
            }
        }

        void AddWebsite(string a_sURL, bool a_bAddToXml)
        {
            Uri url = new Uri(a_sURL);
            string html = string.Empty;

            using (MyClient oWebClient = new MyClient())
            {
                try
                {
                    html = oWebClient.DownloadString(url);
                }
                catch//(System.Net.WebException e)
                {
                }
            }

            bool bIconFound = false;
            string sFavIconURL = string.Empty;

            HtmlAgilityPack.HtmlDocument oHtmlDoc = new HtmlAgilityPack.HtmlDocument();
            oHtmlDoc.LoadHtml(html);
            if (oHtmlDoc.DocumentNode != null)
            {
                HtmlAgilityPack.HtmlNodeCollection HtmlNodes = oHtmlDoc.DocumentNode.SelectNodes("//link[@href]");
                if (HtmlNodes != null)
                {
                    foreach (HtmlAgilityPack.HtmlNode link in HtmlNodes)
                    {
                        HtmlAgilityPack.HtmlAttribute att = link.Attributes["href"];
                        if (att.Value.EndsWith(".ico") || att.Value.EndsWith(".png"))
                        {
                            bIconFound = true;
                            sFavIconURL = att.Value;
                            if (sFavIconURL.Length > 2)
                            {
                                if (sFavIconURL[0] == '/' && sFavIconURL[1] == '/')
                                {
                                    sFavIconURL = @"http:" + sFavIconURL;
                                }
                                else if (sFavIconURL[0] == '/')
                                {
                                    sFavIconURL = @"http://" + url.Host + sFavIconURL;
                                }
                            }

                            if (sFavIconURL == "favicon.ico")
                                bIconFound = false;

                            break;
                        }
                    }
                }
            }

            if (!bIconFound)
            {
                sFavIconURL = @"http://" + url.Host + @"/favicon.ico";
                using (MyClient oClient = new MyClient())
                {
                    oClient.HeadOnly = true;
                    try
                    {
                        oClient.DownloadString(sFavIconURL);
                        bIconFound = true;
                    }
                    catch//(System.Net.WebException error)
                    {
                    }
                }
            }

            byte[] oIconData = new byte[0];

            if(oIconData.Length == 0)
            {
                using(System.Net.WebClient oClient = new System.Net.WebClient())
                {
                    try
                    {
                        oIconData = oClient.DownloadData(sFavIconURL);
                    }
                    catch
                    {
                    }
                }
            }

            if(oIconData.Length == 0)
            {
                bIconFound = false;
            }

            if(!bIconFound)
            {
                using (MyClient oWebClient = new MyClient())
                {
                    try
                    {
                        oIconData = oWebClient.DownloadData(@"http://getfavicon.appspot.com/" + @"http://" + url.Host + "?defaulticon=none");
                        bIconFound = true;
                    }
                    catch
                    {
                    }
                }
            }

            Bitmap NewImage = null;
            if(bIconFound && oIconData.Length > 0)
            {
                Icon oFavIcon;
                using(MemoryStream ms = new MemoryStream(oIconData))
                {
                    Bitmap TempBmp = new Bitmap(ms);
                    TempBmp = new Bitmap(TempBmp, new Size(32, 32));
                    oFavIcon = Icon.FromHandle(TempBmp.GetHicon());
                }

                NewImage = oFavIcon.ToBitmap();
            }
            else
            {
                NewImage = new Bitmap(Properties.Resources.Internet, new System.Drawing.Size(32, 32));
            }

            //Create a new clickable item
            Clickable oNewQuickslot = new Clickable();
            //Add it to the list
            m_oQuickSlots.Add(oNewQuickslot);

            oNewQuickslot.SetImage(ref NewImage);

            oNewQuickslot.m_oAppName = a_sURL;
            oNewQuickslot.SetMouseover();

            char cHtmlLink = (char)0x7F;
            oNewQuickslot.m_oAppDir = cHtmlLink + a_sURL;

            ApplicationList.DataSource = null;
            ApplicationList.DataSource = m_oQuickSlots;
            ApplicationList.DisplayMember = "AppName";

            if (a_bAddToXml)
            {
                //Get the root element from the xml file
                XElement Root = m_XDoc.Element("NI");
                //Get the quickslot element from the xml file
                Root = Root.Element("QuickSlots");
                XElement NewElement = new XElement("Slot",
                    new XAttribute("Name", a_sURL));
                Root.Add(NewElement);
                NewElement.Add(new XAttribute("Path", cHtmlLink + a_sURL));
                m_XDoc.Save(m_sFileDir);
            }
        }

        void CreateOrLoadXML()
        {
            m_sFileDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            m_sFileDir += @"\NI\NI.xml";

            m_XDoc = new XDocument();

            //If the file exists, load it in, else create it
            if (System.IO.File.Exists(m_sFileDir))
            {
                m_XDoc = XDocument.Load(m_sFileDir);
                XElement Root = m_XDoc.Element("NI");

                XElement oQuickSlots = Root.Element("QuickSlots");

                IEnumerable<XElement> oElements = oQuickSlots.Elements();
                foreach (XElement oElement in oElements)
                {
                    string sPath = oElement.Attribute("Path").Value;
                    if (sPath[0] == (char)0x7F)
                        AddWebsite(sPath.Remove(0, 1), false);
                    else
                        AddApp((sPath + oElement.Attribute("Name").Value), false);
                }
                
                string sAppKeyValue = Root.Element("Options").Element("AppHotkey").Value;
                if (sAppKeyValue[0] == 'K')
                {
                    m_oAppKey = int.Parse(sAppKeyValue.Remove(0, 1));
                    if (m_oAppKey != 135)
                        AppHotKey.Text = ((Keys)m_oAppKey).ToString();
                    else
                        AppHotKey.Text = "None";
                }
                else
                {
                    m_oAppKey = int.Parse(sAppKeyValue.Remove(0, 1));
                    AppHotKey.Text = ((MouseButtons)m_oAppKey).ToString();
                }

                if (Root.Element("Options").Element("DisableAppKey").Value == "0")
                {
                    m_bDisableAppKey = false;
                }
                else
                {
                    m_bAllowDisableKeyChange = false;
                    m_bDisableAppKey = true;
                    DisableAppKey.Checked = true;
                }

                string sTimeKeyValue = Root.Element("Options").Element("TimeHotkey").Value;
                if (sTimeKeyValue[0] == 'K')
                {
                    m_oTimeKey = int.Parse(sTimeKeyValue.Remove(0, 1));
                    if (m_oTimeKey != 135)
                        TimeHotKey.Text = ((Keys)m_oTimeKey).ToString();
                    else
                        TimeHotKey.Text = "None";
                }
                else
                {
                    m_oTimeKey = int.Parse(sTimeKeyValue.Remove(0, 1));
                    TimeHotKey.Text = ((MouseButtons)m_oTimeKey).ToString();
                }

                if (Root.Element("Options").Element("DisableTimeKey").Value == "0")
                {
                    m_bDisableTimeKey = false;
                }
                else
                {
                    m_bAllowDisableKeyChange = false;
                    m_bDisableTimeKey = true;
                    DisableTimeKey.Checked = true;
                }

                if (Root.Element("Options").Element("StartUp").Value == "1")
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                    string sValue = (string)key.GetValue("NI");

                    //If the key is there but it is the wrong path, reset it
                    //else, set it
                    if (!String.IsNullOrEmpty(sValue))
                    {
                        if (sValue != "\"" + Application.ExecutablePath + "\"")
                        {
                            key.DeleteValue("NI");
                            key.SetValue("NI", "\"" + Application.ExecutablePath + "\"");
                        }
                    }
                    else
                    {
                        key.SetValue("NI", "\"" + Application.ExecutablePath + "\"");
                    }

                    key.Close();

                    m_bAllowStartupChange = false;
                    Startup.Checked = true;
                }

                string sFont = Root.Element("Options").Element("ClockFont").Value;

                FontConverter oFontConverter = new FontConverter();
                Font oFont = (Font)oFontConverter.ConvertFromString(sFont);

                m_oClock.ClockLabel.Font = oFont;
                m_oClock.ClockLabel.ForeColor = Color.FromName(Root.Element("Options").Element("ClockColor").Value);

                if (Root.Element("Options").Element("Diamond").Value == "0")
                {
                    m_bAllowRadialChange = false;
                    Diamond.Checked = false;
                    Circle.Checked = true;
                    m_bAllowRadialChange = true;
                }

                Speed.Value = int.Parse(Root.Element("Options").Element("Speed").Value);
                SpeedLabel.Text = "Speed: " + Speed.Value;

                Distance.Value = int.Parse(Root.Element("Options").Element("Distance").Value);
                DistanceLabel.Text = "Distance: " + Distance.Value;
            }
            else
            {
                string Dir = m_sFileDir.Remove(m_sFileDir.LastIndexOf("\\"));
                //If the directory for the application doesn't exist, create it
                if (!Directory.Exists(Dir))
                {
                    Directory.CreateDirectory(Dir);
                }

                FontConverter oFontConverter = new FontConverter();
                string sFont = (string)oFontConverter.ConvertToString(m_oClock.ClockLabel.Font);

                //Create a new xml
                XDocument XDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    
                    new XElement("NI",

                    new XElement("QuickSlots"),

                    new XElement("Options",
                        new XElement("AppHotkey", "K" + 0),
                        new XElement("DisableAppKey", 0),
                        new XElement("TimeHotkey", "K" + 0),
                        new XElement("DisableTimeKey", 0),
                        new XElement("StartUp", 0),
                        new XElement("ClockFont", "Microsoft Sans Serif, 20.25pt"),
                        new XElement("ClockColor", "[Color [Black]]"),
                        new XElement("Diamond", 1),
                        new XElement("Speed", 10),
                        new XElement("Distance", 50)
                                ))
                    );

                XDoc.Save(m_sFileDir);

                m_XDoc = XDocument.Load(m_sFileDir);
            }
        }

        private void AppHotKey_Click(object sender, EventArgs e)
        {
            m_bCaptureAppkey = true;

            AppHotKey.Text = "Choose key...";
        }

        private void TimeHotkey_Click(object sender, EventArgs e)
        {
            m_bCaptureTimekey = true;

            TimeHotKey.Text = "Choose key...";
        }

        private void DisableAppKey_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bAllowDisableKeyChange == true)
            {
                if (DisableAppKey.Checked)
                {
                    m_XDoc.Element("NI").Element("Options").Element("DisableAppKey").Value = "1";
                    m_bDisableAppKey = true;
                }
                else
                {
                    m_XDoc.Element("NI").Element("Options").Element("DisableAppKey").Value = "0";
                    m_bDisableAppKey = false;
                }

                m_XDoc.Save(m_sFileDir);
            }
            else
            {
                m_bAllowDisableKeyChange = true;
            }
        }
        
        private void DisableTimeKey_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bAllowDisableKeyChange == true)
            {
                if (DisableTimeKey.Checked)
                {
                    m_XDoc.Element("NI").Element("Options").Element("DisableTimeKey").Value = "1";
                    m_bDisableTimeKey = true;
                }
                else
                {
                    m_XDoc.Element("NI").Element("Options").Element("DisableTimeKey").Value = "0";
                    m_bDisableTimeKey = false;
                }

                m_XDoc.Save(m_sFileDir);
            }
            else
            {
                m_bAllowDisableKeyChange = true;
            }
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete(m_sFileDir);
            }
            catch
            {
            }

            CreateOrLoadXML();
            CreateOrLoadXML();

            Startup.Checked = false;

            DisableAppKey.Checked = false;
            DisableTimeKey.Checked = false;

            m_bDisableAppKey = false;
            m_bDisableTimeKey = false;

            m_bAllowRadialChange = false;
            Diamond.Checked = true;
            m_bAllowRadialChange = true;

            StopShowing();
            StopShowingClock();

            m_oQuickSlots = new List<Clickable>();
            m_oPoints = new List<Point>();

            ApplicationList.DataSource = null;
            ApplicationList.DataSource = m_oQuickSlots;
            ApplicationList.DisplayMember = "AppName";
        }

        private void HelpBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Welcome to New Interface (NI)\n\n"
                + "Hotkeys - Click the buttons next to the help button (question mark) to set your hotkeys.\n\n"
                + "Disable key - These checkboxes will attempt to block your selected keys from working outside of this program.\n\n"
                + "Applications - Click the add button and choose a file to be part of your chosen apps.\n"
                + "Files and folders can also be dragged into the list of items (folders can not be added via the add button).\n\n"
                + "Websites can be added with the Add Site button, it will attempt to get the favicon of the site (this takes a few seconds) and when clicked it will open in your default browser.\n\n"
                + "Click the remove button to remove your selected file from the list.\n\n"
                + "The Diamond and Circle options will change the pattern in which your icons will appear.\n\n"
                + "The speed slider will alter how fast the icons move out from the cursor.\n"
                + "The distance slider will change how far the icons move out from the cursor.\n\n"
                + "Usage - Press and hold your chosen hotkey to make your selected programs to appear around your cursor.\n"
                + "Left click your desired app/folder/site to open it.\nRight click an app/folder to open its containing folder.\n\n"
                + "Recommended - The windows key makes an excellent hotkey, especially when disable key is ticked. Ctrl and Alt are also recommended.\n"
                + "Your chosen hotkeys may interfere with fullscreen programs, please make use of the disable checkbox.\n\n"
                + "The start at startup checkbox will cause this application to open when Windows is started."
                , "Help", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void DisabledBox_CheckedChanged(object sender, EventArgs e)
        {
            DisableEnable(sender, e);
            m_bAllowDisableChange = true;
        }

        private void UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("test");
            m_oTrayIcon.Dispose();
            m_oTrayMenu.Dispose();

            UnhookWindowsHookEx(ptrKbdHook);
            ptrKbdHook = IntPtr.Zero;

            UnhookWindowsHookEx(ptrMouseHook);
            ptrMouseHook = IntPtr.Zero;

            Application.Exit();
        }

        private void Startup_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bAllowStartupChange)
            {
                if (Startup.Checked)
                {
                    m_XDoc.Element("NI").Element("Options").Element("StartUp").Value = "1";
                    CreateRemoveStartup(true);
                }
                else
                {
                    m_XDoc.Element("NI").Element("Options").Element("StartUp").Value = "0";
                    CreateRemoveStartup(false);
                }

                m_XDoc.Save(m_sFileDir);
            }
            else
            {
                m_bAllowStartupChange = true;
            }
        }

        void CreateRemoveStartup(bool a_bCreate)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (a_bCreate)
            {
                //Surround path with " to make sure that there are no problems if the path contains spaces.
                key.SetValue("NI", "\"" + Application.ExecutablePath + "\"");
            }
            else
            {
                key.DeleteValue("NI");
            }

            key.Close();
        }

        private void AddAppBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();

            fDialog.Title = "Choose file";

            fDialog.Filter = "All files|*.*";

            fDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            if (fDialog.ShowDialog() == DialogResult.OK)
            {
                AddApp(fDialog.FileName, true);
            }
        }

        private void UI_MouseClick(object sender, MouseEventArgs e)
        {
            ApplicationList.SelectedItem = null;
        }

        private void RemoveAppBtn_Click(object sender, EventArgs e)
        {
            int iSelectedIndex = ApplicationList.SelectedIndex;

            //Checks if an item is selected
            if(iSelectedIndex != -1)
            {
                IEnumerable<XElement> oElements = m_XDoc.Element("NI").Element("QuickSlots").Elements("Slot");
                int iIndex = 0;
                foreach (XElement oElement in oElements)
                {
                    if(iIndex == iSelectedIndex)
                    {
                        oElement.Remove();
                        break;
                    }

                    ++iIndex;
                }

                m_XDoc.Save(m_sFileDir);
                // Remove the item in the List.
                m_oQuickSlots.RemoveAt(iSelectedIndex);

                ApplicationList.DataSource = null;
                ApplicationList.DataSource = m_oQuickSlots;
                ApplicationList.DisplayMember = "AppName";
            }
        }

        private void ApplicationList_DragDrop(object sender, DragEventArgs e)
        {
            string[] sFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            for (int i = 0 ; i < sFilePaths.Length ; ++i)
            {
                AddApp(sFilePaths[i], true);
            }
        }

        private void ApplicationList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        //Return true if the function returned early
        void FinishCircle(ref List<Point> a_oCurrentCircle, ref int a_iSlotCount, int a_iDistance)
        {
            List<Point> oFinishedCircle = new List<Point>();
            
            for (int i = 0 ; i < a_oCurrentCircle.Count ; ++i)
            {
                if(Diamond.Checked)
                {
                    oFinishedCircle.Add(a_oCurrentCircle[i]);

                    //Calculate the points between the last and first points
                    //else calculate between the current and next point
                    if(i == a_oCurrentCircle.Count - 1)
                    {
                        oFinishedCircle.Add(BetweenTwoPoints(a_oCurrentCircle[i], a_oCurrentCircle[0]));
                    }
                    else
                    {
                        oFinishedCircle.Add(BetweenTwoPoints(a_oCurrentCircle[i], a_oCurrentCircle[i + 1]));
                    }
                }
                else
                {
                    Point MovedCurrentPoint = a_oCurrentCircle[i];
                    double fXToMidPoint = MovedCurrentPoint.X - MousePosition.X;
                    double fYToMidPoint = MovedCurrentPoint.Y - MousePosition.Y;
                    double Distance = Math.Sqrt(Math.Pow(fXToMidPoint, 2) + Math.Pow(fYToMidPoint, 2));
                    fXToMidPoint /= Distance;
                    fYToMidPoint /= Distance;
                    MovedCurrentPoint.X = Convert.ToInt32(MousePosition.X + fXToMidPoint * ((double)a_iDistance * (m_iCirclesMade + 1.0)));
                    MovedCurrentPoint.Y = Convert.ToInt32(MousePosition.Y + fYToMidPoint * ((double)a_iDistance * (m_iCirclesMade + 1.0)));

                    oFinishedCircle.Add(MovedCurrentPoint);

                    Point MidPoint = new Point();

                    if(i == a_oCurrentCircle.Count - 1)
                    {
                        MidPoint = BetweenTwoPoints(a_oCurrentCircle[i], a_oCurrentCircle[0]);
                    }
                    else
                    {
                        MidPoint = BetweenTwoPoints(a_oCurrentCircle[i], a_oCurrentCircle[i + 1]);
                    }

                    fXToMidPoint = MidPoint.X - MousePosition.X;
                    fYToMidPoint = MidPoint.Y - MousePosition.Y;
                    Distance = Math.Sqrt(Math.Pow(fXToMidPoint, 2) + Math.Pow(fYToMidPoint, 2));
                    fXToMidPoint /= Distance;
                    fYToMidPoint /= Distance;
                    MidPoint.X = Convert.ToInt32(MousePosition.X + fXToMidPoint * ((double)a_iDistance * (m_iCirclesMade + 1.0)));
                    MidPoint.Y = Convert.ToInt32(MousePosition.Y + fYToMidPoint * ((double)a_iDistance * (m_iCirclesMade + 1.0)));

                    oFinishedCircle.Add(MidPoint);
                }

                ++a_iSlotCount;
            }

            ++m_iCirclesMade;

            a_oCurrentCircle = oFinishedCircle;
        }

        Point BetweenTwoPoints(Point a_oFirst, Point a_oSecond)
        {
            Point oReturn = new Point(a_oFirst.X - a_oSecond.X, a_oFirst.Y - a_oSecond.Y);
            oReturn.X = oReturn.X >> 1;
            //oReturn.X = oReturn.X / 2;
            oReturn.Y = oReturn.Y >> 1;
            //oReturn.Y = oReturn.Y / 2;
            oReturn.X = a_oSecond.X + oReturn.X;
            oReturn.Y = a_oSecond.Y + oReturn.Y;
            return oReturn;
        }

        //Return true if the function returned early
        void MakeNextCircle(ref List<Point> a_oCurrentCircle, ref int a_iSlotCount, int a_iDistance)
        {
            List<Point> oNextCircle = new List<Point>();
            
            if (a_iSlotCount >= m_oQuickSlots.Count)
            {
                return;
            }

            //The direction from the mouse to a point
            Point oDirFromMouse = new Point();

            for (int i = 0; i < a_oCurrentCircle.Count; ++i)
            {
                //Get the direction from the mouse to the current point
                oDirFromMouse.X = a_oCurrentCircle[i].X - MousePosition.X;
                oDirFromMouse.Y = a_oCurrentCircle[i].Y - MousePosition.Y;

                if(Diamond.Checked)
                {
                    //Increase the distance to make the icons not overlap the previous circle icons
                    oDirFromMouse.X += (int)((double)oDirFromMouse.X - (double)oDirFromMouse.X / 1.3);
                    oDirFromMouse.Y += (int)((double)oDirFromMouse.Y - (double)oDirFromMouse.Y / 1.3);

                    oDirFromMouse.X /= m_iCirclesMade;
                    oDirFromMouse.Y /= m_iCirclesMade;
                }
                else
                {
                    if(oDirFromMouse.X < 0)
                        oDirFromMouse.X = -1;
                    else if(oDirFromMouse.X > 0)
                        oDirFromMouse.X = 1;

                    if(oDirFromMouse.Y < 0)
                        oDirFromMouse.Y = -1;
                    else if(oDirFromMouse.Y > 0)
                        oDirFromMouse.Y = 1;

                    oDirFromMouse.X *= a_iDistance;
                    oDirFromMouse.Y *= a_iDistance;
                }

                //Create a new point further away from the mouse
                oNextCircle.Add(new Point(a_oCurrentCircle[i].X + oDirFromMouse.X, a_oCurrentCircle[i].Y + oDirFromMouse.Y));

                ++a_iSlotCount;
            }

            a_oCurrentCircle = oNextCircle;
        }

        void AddPointsToMemberList(ref List<Point> a_oAdditionalPoints)
        {
            for (int i = 0 ; i < a_oAdditionalPoints.Count ; ++i)
            {
                m_oPoints.Add(a_oAdditionalPoints[i]);
            }
        }

        private void TimeFont_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowColor = true;
            fontDialog1.ShowEffects = false;

            fontDialog1.Font = m_oClock.ClockLabel.Font;
            fontDialog1.Color = m_oClock.ClockLabel.ForeColor;

            if (fontDialog1.ShowDialog() != DialogResult.Cancel)
            {
                m_oClock.ClockLabel.Font = fontDialog1.Font;
                m_oClock.ClockLabel.ForeColor = fontDialog1.Color;
                m_oClock.UpdateClock(sender);

                FontConverter oFontConverter = new FontConverter();
                string sFont = (string)oFontConverter.ConvertToString(m_oClock.ClockLabel.Font);

                m_XDoc.Element("NI").Element("Options").Element("ClockFont").Value = sFont;
                m_XDoc.Element("NI").Element("Options").Element("ClockColor").Value = m_oClock.ClockLabel.ForeColor.Name.ToString();
                m_XDoc.Save(m_sFileDir);
            }
        }

        private void AddSiteBtn_Click(object sender, EventArgs e)
        {
            AddSiteDialog oAddSiteDialog = new AddSiteDialog();

            if (oAddSiteDialog.ShowDialog(this) == DialogResult.OK)
            {
                AddWebsite(oAddSiteDialog.SiteText.Text, true);
            }
        }

        private void SettingsFolderBtn_Click(object sender, EventArgs e)
        {
            string FolderDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\NI";
            if(Directory.Exists(FolderDir))
            {
                System.Diagnostics.Process.Start(FolderDir);
            }
            else
            {
                MessageBox.Show("\"" + FolderDir + "\" path not found");
            }
        }

        private void Diamond_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bAllowRadialChange)
            {
                if(Diamond.Checked)
                {
                    m_XDoc.Element("NI").Element("Options").Element("Diamond").Value = "1";
                    m_XDoc.Save(m_sFileDir);
                }
            }
        }

        private void Circle_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bAllowRadialChange)
            {
                if(Circle.Checked)
                {
                    m_XDoc.Element("NI").Element("Options").Element("Diamond").Value = "0";
                    m_XDoc.Save(m_sFileDir);
                }
            }
        }

        private void SpeedScale_Scroll(object sender, EventArgs e)
        {
            SpeedLabel.Text = "Speed: " + Speed.Value;
            m_XDoc.Element("NI").Element("Options").Element("Speed").Value = Speed.Value.ToString();
            m_XDoc.Save(m_sFileDir);
        }

        private void Distance_Scroll(object sender, EventArgs e)
        {
            DistanceLabel.Text = "Distance: " + Distance.Value;
            m_XDoc.Element("NI").Element("Options").Element("Distance").Value = Distance.Value.ToString();
            m_XDoc.Save(m_sFileDir);
        }
    }

    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };
}