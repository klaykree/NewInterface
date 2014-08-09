using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace NewUI
{
    public partial class Clickable : Form
    {
        public string m_oAppDir;

        public string m_oAppName;

        public float m_XPos;
        public float m_YPos;

        public Clickable()
        {
            Show();
            InitializeComponent();
            Hide();

            MouseoverName.AutoPopDelay = 5000;
            MouseoverName.InitialDelay = 0;
            MouseoverName.ReshowDelay = 10;
            //Force the ToolTip text to be displayed whether or not the form is active.
            MouseoverName.ShowAlways = true;

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        private void Clickable_MouseClick(object sender, MouseEventArgs e)
        {
            //Check if it is a html link
            if(m_oAppDir[0] == (char)0x7F)
            {
                System.Diagnostics.Process.Start(m_oAppDir.Remove(0, 1));
            }
            else
            {
                try
                {
                    if(e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        //This needs to be in a try catch in case the user cancels the process starting
                        System.Diagnostics.Process.Start(m_oAppDir + m_oAppName);
                    }
                    else if(e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + m_oAppDir + m_oAppName + "\"");
                    }
                }
                catch
                {
                }
            }
        }

        public void SetImage(ref Bitmap a_oBGImage)
        {
            BackgroundImage = a_oBGImage;
            //Width = a_oBGImage.Size.Width;
            //Height = a_oBGImage.Size.Height;
            Width = 32;
            Height = 32;
        }

        public void SetMouseover()
        {
            MouseoverName.SetToolTip(this, AppName);
        }

        public string AppName
        {
            get
            {
                //char cReplace = (char)254;
                //return m_oAppName.Replace(cReplace, ' ');
                return m_oAppName;
            }
        }
    }
}
