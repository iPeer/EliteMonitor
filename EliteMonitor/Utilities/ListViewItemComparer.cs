using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor.Utilities
{
    // https://msdn.microsoft.com/en-us/library/ms996467.aspx
    // I'm lazy so I just copied this (Y)
    class ListViewItemComparer : IComparer
    {
        private int col;
        public ListViewItemComparer()
        {
            col = 0;
        }
        public ListViewItemComparer(int column)
        {
            col = column;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
            ((ListViewItem)y).SubItems[col].Text);
            return returnVal;
        }
    }

    class ListViewItemDoubleComparer : IComparer
    {
        private int col;
        public ListViewItemDoubleComparer()
        {
            col = 0;
        }
        public ListViewItemDoubleComparer(int column)
        {
            col = column;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            double item1 = Convert.ToDouble(((ListViewItem)x).SubItems[col].Text);
            double item2 = Convert.ToDouble(((ListViewItem)y).SubItems[col].Text);
            returnVal = item1.CompareTo(item2);
            return returnVal;
        }
    }

}
