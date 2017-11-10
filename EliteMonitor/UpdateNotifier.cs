using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor
{
    public partial class UpdateNotifier : Form
    {
        public UpdateNotifier()
        {
            InitializeComponent();
            GrabChangelog();
        }

        public void setVersion(string version)
        {
            this.labelUpdateText.Text = this.labelUpdateText.Text.Replace("{VERSION}", version);
        }

        public void GrabChangelog()
        {
            WebClient w = new WebClient();
            w.DownloadStringCompleted += OnStringDownloadComplete;
#if DEBUG
            w.DownloadStringAsync(new Uri("https://raw.githubusercontent.com/iPeer/EliteMonitor/master/EliteMonitor/ChangeLog.txt"));
#else
            w.DownloadStringAsync(new Uri("https://ipeer.auron.co.uk/EliteMonitor/changelog"));
#endif
        }

        private void OnStringDownloadComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            this.richTextBox1.Text = e.Result;
            ((WebClient)sender).Dispose();
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            Process.Start("https://ipeer.auron.co.uk/EliteMonitor/EliteMonitor.zip");
            this.Close();
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
