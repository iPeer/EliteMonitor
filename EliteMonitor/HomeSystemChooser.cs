using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EliteMonitor.Extensions;
using System.Threading;
using EliteMonitor.Utilities;
using EliteMonitor.Elite;

namespace EliteMonitor
{
    public partial class HomeSystemChooser : Form
    {
        public HomeSystemChooser()
        {
            InitializeComponent();
            if (MainForm.Instance.journalParser.viewedCommander.HasHomeSystem)
            {
                this.textBoxSystemInput.Text = MainForm.Instance.journalParser.viewedCommander.HomeSystem.Name;
            }
        }

        private void buttonSetHomeSystem_Click(object sender, EventArgs e)
        {
            string text = this.textBoxSystemInput.Text;
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("You must enter a system name to search for");
                return;
            }
            this.InvokeIfRequired(() => this.progressBar1.Visible = true);
            Thread t = new Thread(new ThreadStart(() =>
            {
                BasicSystem bs = null;
                try
                {
                    bs = MainForm.Instance.Database.getSystemDataFromEDSMAPI(text);
                    this.InvokeIfRequired(() => this.progressBar1.Visible = false);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("No system with that name was found in the EDSM database. Please check your spelling and try again.");
                    return;
                }
                MainForm.Instance.journalParser.viewedCommander.setHomeSystem(bs);
                MessageBox.Show(string.Format("Home system set to '{0}'", bs.Name));
                this.InvokeIfRequired(() => this.Close());
            }));
            t.IsBackground = false;
            t.Start();
        }
    }
}
