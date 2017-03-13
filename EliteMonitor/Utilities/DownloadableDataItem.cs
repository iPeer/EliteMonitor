using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Utilities
{
    public class DownloadableDataItem
    {

        public string DownloadFrom  { get; private set; }
        public string DownloadTo    { get; private set; }

        public DownloadableDataItem(string from, string to)
        {
            this.DownloadFrom = from;
            this.DownloadTo = to;
        }

    }
}
