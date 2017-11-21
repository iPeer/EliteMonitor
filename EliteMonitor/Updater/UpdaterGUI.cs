using EliteMonitor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor.Updater
{
    public partial class UpdaterGUI : Form
    {
        public UpdaterGUI()
        {
            InitializeComponent();
            this.Show();
            beginUpdate();
        }

        public void beginUpdate()
        {
            string updatePath = Path.Combine(Utils.getApplicationEXEFolderPath(), @"EMUpdateData\");
            if (!Directory.Exists(updatePath))
            {
                setStatus("Creating update directory...");
                Directory.CreateDirectory(updatePath);
            }
            else
            {
                setStatus("Removing older update files...");
                Directory.Delete(updatePath, true);
                Directory.CreateDirectory(updatePath);
            }
            setStatus("Downloading update...");
            WebClient wc = new WebClient();
            wc.DownloadFileCompleted += OnDownloadComplete;
            wc.DownloadProgressChanged += OnDownloadProgressChanged;
            wc.DownloadFileAsync(new Uri("https://ipeer.auron.co.uk/EliteMonitor/EliteMonitor.zip"), Path.Combine(updatePath, "EliteMonitor.zip"));
            setDownloadStatus("Downloading https://ipeer.auron.co.uk/EliteMonitor/EliteMonitor.zip...");
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            setDownloadStatus(string.Format("Downloading https://ipeer.auron.co.uk/EliteMonitor/EliteMonitor.zip: {0:n0}/{1:n0} bytes", e.BytesReceived, e.TotalBytesToReceive));
        }

        private void OnDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            setStatus("Unpacking updated files...");
            string updatePath = Path.Combine(Utils.getApplicationEXEFolderPath(), @"EMUpdateData\");
            string updateZIPPath = Path.Combine(updatePath, "EliteMonitor.zip");
            string updateStagingPath = Path.Combine(updatePath, @"staging\");
            ZipFile.ExtractToDirectory(updateZIPPath, updateStagingPath);
            // TODO COPY FILES
            setStatus("Update complete!");
            setDownloadStatus(string.Empty);
        }

        public void setStatus(string text)
        {
            this.labelStatus.Text = text;
        }

        public void setDownloadStatus(string text)
        {
            this.labelDownloadStatus.Text = text;
        }
    }
}
