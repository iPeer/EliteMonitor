using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Utilities
{
    public class FileDownloadHandler
    {
        public AsyncCompletedEventHandler OnFileDownloadComplete;
        public DownloadProgressChangedEventHandler OnFileDownloadProgressChanged;

        DownloadableDataItem item { get; set; }

        public FileDownloadHandler(DownloadableDataItem ddi)
        {
            this.item = ddi;
        }

        public FileDownloadHandler(string from, string to) : this(new DownloadableDataItem(from, to)) { }

        public void DownloadFile()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileAsync(new Uri(this.item.DownloadFrom), this.item.DownloadTo);
                wc.DownloadFileCompleted += this.FileDownloadComplete;
                wc.DownloadProgressChanged += this.FileDownloadProgressChanged;
            }
        }

        private void FileDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChangedEventHandler handler = OnFileDownloadProgressChanged;
            handler.Invoke(sender, e);
        }

        private void FileDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            AsyncCompletedEventHandler handler = OnFileDownloadComplete;
            handler.Invoke(sender, e);
        }
    }
}
