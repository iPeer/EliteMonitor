using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace EliteMonitor.Extensions
{
    public static class ListViewExtensions
    {

        public static ListViewItem Insert(this ListViewItemCollection lvic, int index, string[] itemData)
        {
            return lvic.Insert(index, new ListViewItem(itemData));
        }

    }
}
