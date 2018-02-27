using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor.Extensions
{
    public static class MenuStripExtensions
    {

        public static List<ToolStripMenuItem> GetAllSubItemsInMenuStrip(this MenuStrip me)
        {
            List<ToolStripMenuItem> controls = new List<ToolStripMenuItem>();
            foreach (ToolStripMenuItem c in me.Items)
            {
                GetAllSubItems(c, controls);
            }
            return controls;
        }

        public static List<ToolStripMenuItem> GetAllSubItems(this ToolStripMenuItem me)
        {
            List<ToolStripMenuItem> controls = new List<ToolStripMenuItem>();
            GetAllSubItems(me, controls);
            return controls;
        }
        private static void GetAllSubItems(ToolStripMenuItem parent, List<ToolStripMenuItem> listToPopulate)
        {
            foreach (ToolStripMenuItem c in parent.DropDownItems)
            {
                listToPopulate.Add(c);
                GetAllSubItems(c, listToPopulate);
            }
        }

    }
}
