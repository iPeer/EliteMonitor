using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor.Extensions
{
    public static class FormExtensions
    {

        public static List<Control> GetAllControls(this Form form)
        {
            List<Control> controls = new List<Control>();
            GetAllControls(form, controls);
            return controls;
        }
        private static void GetAllControls(Control parent, List<Control> listToPopulate)
        {
            foreach (Control c in parent.Controls)
            {
                listToPopulate.Add(c);
                GetAllControls(c, listToPopulate);
            }
        }

    }
}
