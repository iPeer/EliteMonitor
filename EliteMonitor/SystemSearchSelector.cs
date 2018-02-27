using EliteMonitor.Elite;
using EliteMonitor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using EliteMonitor.Extensions;
using System.Diagnostics;

namespace EliteMonitor
{

    // FIXME: Memory leak (somewhere)!
    // Memory leak may be fixed, requires further testing

    public partial class SystemSearchSelector : Form
    {

        public event EventHandler<BasicSystem> OnSystemSelected;
        public List<BasicSystem> searchResults;
        Thread searchThread;

        public SystemSearchSelector()
        {
            InitializeComponent();
            if (Properties.Settings.Default.darkModeEnabled)
                Utils.toggleNightModeForForm(this);
        }

        public void SetCustomTitle(string title)
        {
            this.Text = title;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (this.searchResults != null)
                this.searchResults.Clear();
            if (string.IsNullOrWhiteSpace(textBoxSearchText.Text))
            {
                MessageBox.Show("You must enter some text to search for!");
                return;
            }
            labelInstructions.Visible = false;
            progressBar1.Visible = labelProgress.Visible = true;
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;
            labelProgress.Text = "Waiting for EDSM to supply data...";
            progressBar1.Style = ProgressBarStyle.Marquee;
            listView1.Items.Clear();
            if (searchThread != null && searchThread.IsAlive)
                searchThread.Abort();
            if (searchThread == null || !searchThread.IsAlive)
            {
                this.searchThread = null;
                this.searchThread = new Thread(() =>
                {
                    EliteDatabase.Instance.getSystemSearchResultsFromEDSMAPI(textBoxSearchText.Text, EDSMDataDownloadCompleted, EDSMDataDownloadProgress, EDSMSystemParseProgress);
                });
            }
            /*searchThread.IsBackground = true;*/
            searchThread.Start();

        }

        private void EDSMDataDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            if (progressBar1.Value == progressBar1.Maximum || e.ProgressPercentage == 100)
            {
                //progressBar1.InvokeIfRequired(() => progressBar1.Style = ProgressBarStyle.Marquee);
                labelProgress.InvokeIfRequired(() => labelProgress.Text = "Parsing search results... (this may take a while for large numbers of systems)");
            }
            else
            {
                //progressBar1.InvokeIfRequired(() => progressBar1.Value = e.ProgressPercentage);
                labelProgress.InvokeIfRequired(() => labelProgress.Text = string.Format("{0:n0} bytes received", e.BytesReceived));
            }
        }

        private void EDSMSystemParseProgress(object sender, Int64[] progress)
        {
            progressBar1.InvokeIfRequired(() => {

                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = (int)progress[0];
                progressBar1.Maximum = (int)progress[1];

            });
            labelProgress.InvokeIfRequired(() => labelProgress.Text = $"Parsing results... {progress[0]} / {progress[1]}");
        }

        private void EDSMDataDownloadCompleted(object sender, List<BasicSystem> e)
        {
            labelInstructions.InvokeIfRequired(() => labelInstructions.Visible = true);
            progressBar1.InvokeIfRequired(() => progressBar1.Visible = labelProgress.Visible = false);
            if (this.listView1.Items.Count > 0)
                this.listView1.InvokeIfRequired(() => this.listView1.Items.Clear());
            if (this.searchResults != null && this.searchResults.Count > 0)
                this.searchResults.Clear();
            if (this.searchResults == null)
                this.searchResults = new List<BasicSystem>();
            /*e.Clear();
            this.searchResults.OrderBy(a => a.DistanceFromSol);*/
            e.OrderBy(a => a.DistanceFromSol);
            this.searchResults.Clear();
            this.searchResults.AddRange(e);
            /*this.searchResults.OrderBy(a => a.DistanceFromSol);*/
            listView1.InvokeIfRequired(() => listView1.BeginUpdate());
#if DEBUG
            Debug.WriteLine(e.Count);
#endif
            List<ListViewItem> items = new List<ListViewItem>();
            foreach (BasicSystem s in e)
            {
                ListViewItem lvi = new ListViewItem(new string[] { string.Format("{0:n2}", s.DistanceFromSol), s.Name, s.Coordinates.ToString() });
                //listView1.InvokeIfRequired(() => listView1.Items.Add(lvi));
                items.Add(lvi);
            }
            e.Clear();
            listView1.InvokeIfRequired(() => listView1.Items.AddRange(items.ToArray()));
            items.Clear();
            listView1.InvokeIfRequired(() =>
            {
                foreach (ColumnHeader c in listView1.Columns)
                    c.Width = -2;
                listView1.ListViewItemSorter = new ListViewItemDoubleComparer(0);
                listView1.Sort();
            });
            listView1.InvokeIfRequired(() => listView1.EndUpdate());

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("You must select a system to use before pressing OK. If you've changed your mind, press cancel instead.");
                return;
            }
            ListViewItem lvi = listView1.SelectedItems[0]; // We can only have one selected, and we already have sanity checking for it none are selected.
            string coords = lvi.SubItems[2].Text;

            BasicSystem s = this.searchResults.First(a => a.Coordinates.ToString().Equals(coords));
            if (s == null)
            {
                MessageBox.Show("Well this is embarassing... It seems that something went very wrong when attempting to select this system! Please try again, and, if it persists, report it as a bug!");
                return;
            }
#if DEBUG
            MessageBox.Show($"SYSTEM: {s.Name} \nCOORDINTAES: {s.Coordinates.ToString()}\nDISTANCE FROM SOL: {s.DistanceFromSol}\nALLEGIANCE: {s.Allegiance}\nECONOMY: {s.Economy}");
#endif
            OnSystemSelected?.Invoke(this, s);
            this.Close();
        }

        private void textBoxSearchText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.buttonSearch.PerformClick();
        }

        private void SystemSearchSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.searchResults != null)
                this.searchResults.Clear();
            this.listView1.Items.Clear();
            if (this.searchThread != null && this.searchThread.IsAlive)
                this.searchThread.Abort();
            this.searchThread = null;
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            Utils.DrawColumnHeader(sender, e);
        }
    }
}
