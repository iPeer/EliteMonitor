using EliteMonitor.Elite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor
{
    public partial class DiscoveredBodyList : Form
    {

        public DiscoveredBodyList()
        {
            InitializeComponent();

            List<BodyDiscovery> bodies = new List<BodyDiscovery>();
            bodies = new List<BodyDiscovery>(MainForm.Instance.journalParser.viewedCommander.DiscoveredBodies);
            labelBodiesDiscovered.Text = string.Format("Bodies discovered: {0:n0}", bodies.Count);
            bodies.OrderBy(b => b.DiscoveredTime).Reverse();

            foreach (BodyDiscovery d in bodies)
            {
                this.listViewDiscoveries.Items.Add(new ListViewItem(new string[] { d.DiscoveredTime.ToString(), d.BodyName }));
            }

            foreach (ColumnHeader ch in this.listViewDiscoveries.Columns)
            {

                ch.Width = -2;
            }

        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
