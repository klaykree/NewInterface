using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;

namespace NewUI
{
    public partial class AddSiteDialog : Form
    {
        public AddSiteDialog()
        {
            InitializeComponent();
        }

        private void AddSiteDialog_Load(object sender, EventArgs e)
        {
            SiteText.Select();
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (SiteText.Text == string.Empty)
            {
                DialogResult = DialogResult.None;
                return;
            }

            Uri OutURI;
            bool bUriCreated = Uri.TryCreate(SiteText.Text, UriKind.Absolute, out OutURI);
            if (!bUriCreated)
            {
                InfoLabel.Text = "Could not add site. Make sure it starts with http";
                DialogResult = DialogResult.None;
                return;
            }
            else
            {
                using(MyClient oClient = new MyClient())
                {
                    oClient.HeadOnly = true;
                    try
                    {
                        InfoLabel.Text = "Getting favicon. Please wait.";
                        InfoLabel.Update();
                        oClient.DownloadString(SiteText.Text);
                        DialogResult = DialogResult.OK;
                    }
                    catch (WebException error)
                    {
                        InfoLabel.Text = error.Message;
                        DialogResult = DialogResult.None;
                        return;
                    }
                }
            }
        }
    }

    class MyClient : WebClient
    {
        public bool HeadOnly { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest req = base.GetWebRequest(address);
            req.Timeout = 5 * 1000; //5 seconds
            if (HeadOnly && req.Method == "GET")
            {
                req.Method = "HEAD";
            }
            return req;
        }
    }
}
