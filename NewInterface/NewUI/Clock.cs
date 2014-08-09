using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NewUI
{
    public partial class Clock : Form
    {
        //System.Threading.Timer m_oClockTimer;

        public Clock()
        {
            InitializeComponent();

            ClockLabel.Location = new Point(0, 0);
            
            //m_oClockTimer = new System.Threading.Timer(UpdateClock, null, 60 - DateTime.Now.Second, 1000);

            //Shows the time
            //ClockLabel.Text = DateTime.Now.ToString("hh:mm tt");
            
            //Shows the date
            //ClockLabel.Text = DateTime.Now.ToString("MMM (M) ddd d");
            
            Width = ClockLabel.Width;
            Height = ClockLabel.Height;

            Opacity = 0;
        }

        public void ResetDate()
        {
            //Get the day and the day of the month
            string sDay = DateTime.Now.ToString("ddd d");
            //Add on to the day of the month (1st, 2nd, 3rd, 4th etc.)
            SetEnding(ref sDay);
            //Get the month and the month number
            string sMonth = DateTime.Now.ToString("MMM(M)");
            ClockLabel.Text = sDay + " " + sMonth;
            Width = ClockLabel.Width;
            Height = ClockLabel.Height;
        }

        void SetEnding(ref string a_sDay)
        {
            //Get the last character in the date which represents the day of the month
            char cEnd = a_sDay[a_sDay.Length - 1];

            if (cEnd == '1')
            {
                a_sDay += "st";
            }
            else if (cEnd == '2')
            {
                a_sDay += "nd";
            }
            else if (cEnd == '3')
            {
                a_sDay += "rd";
            }
            else
            {
                a_sDay += "th";
            }
        }

        //Updates every minute (synced with system time)
        public void UpdateClock(object state)
        {
            if (ClockLabel.InvokeRequired)
            {
                ClockLabel.Invoke(new MethodInvoker(delegate { ClockLabel.Text = DateTime.Now.ToString("hh:mm tt"); }));
                ClockLabel.Invoke(new MethodInvoker(delegate { Width = ClockLabel.Width; }));
                ClockLabel.Invoke(new MethodInvoker(delegate { Height = ClockLabel.Height; }));
            }
            else
            {
                ClockLabel.Text = DateTime.Now.ToString("hh:mm tt");
                Width = ClockLabel.Width;
                Height = ClockLabel.Height;
            }
        }
    }
}
