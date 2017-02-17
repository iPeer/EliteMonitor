using EliteMonitor.Caching;
using EliteMonitor.Elite;
using EliteMonitor.Extensions;
using EliteMonitor.Journal;
using EliteMonitor.Logging;
using EliteMonitor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor
{
    public partial class MainForm : Form
    {

        private ListViewItem.ListViewSubItem _lastListViewItem;
        private Point _lastToolTipMousePos;
        public static MainForm Instance { get; private set; }
        private Dictionary<string, Color> defaultColours = new Dictionary<string, Color>();
        public JournalParser journalParser;
        public CacheController cacheController;
        public Logger logger;
        private Thread eliteCheckerThread;

        public MainForm()
        {
            this.logger = new Logger("Main");
            InitializeComponent();
            appVersionStatusLabel.Text = Utils.getApplicationVersion();
            eventFilterDropdown.SelectedIndex = eventFilterDropdown.Items.IndexOf("NONE");
            Instance = this;
            journalParser = new JournalParser(this);
            cacheController = new CacheController(this);
            eliteCheckerThread = new Thread(() =>
            {
                while (true)
                {
                    bool r = EliteUtils.IsEliteRunning();
                    eliteRunningStatus.Text = String.Format("Elite: Dangerous is{0}running", r ? " " : " not ");
                    Thread.Sleep(60000);
                }
            });
            eliteCheckerThread.IsBackground = true;
            eliteCheckerThread.Start();

            /*string[] lines = new string[]
            {
                "{ \"timestamp\":\"2017-01-26T21:23:18Z\", \"event\":\"Fileheader\", \"part\":1, \"language\":\"English\\UK\", \"gameversion\":\"2.2\", \"build\":\"r131487/r0 \" }",
                "{ \"timestamp\":\"2017-01-26T21:24:47Z\", \"event\":\"LoadGame\", \"Commander\":\"iPeer\", \"Ship\":\"Cutter\", \"ShipID\":11, \"GameMode\":\"Group\", \"Group\":\"Mobius\", \"Credits\":153430680, \"Loan\":0 }",
                "{ \"timestamp\":\"2017-01-26T21:24:47Z\", \"event\":\"Rank\", \"Combat\":5, \"Trade\":7, \"Explore\":5, \"Empire\":12, \"Federation\":7, \"CQC\":0 }",
                "{ \"timestamp\":\"2017-01-26T21:24:47Z\", \"event\":\"Progress\", \"Combat\":58, \"Trade\":27, \"Explore\":11, \"Empire\":3, \"Federation\":3, \"CQC\":0 }"
            };
            foreach (string s in lines)
                jp.parseEvent(s.Replace(@"\", ""));*/
            if (!cacheController.cacheExists())
            {
                startNoCacheLoadThread();
            }
            else
            {
                startCacheLoadThread();
            }
        }

        public void startNoCacheLoadThread()
        {
            Thread t = new Thread(() =>
            {
                journalParser.parseAllJournals();
                cacheController.saveAllCaches();
            });
            t.Start();
        }

        public void startCacheLoadThread()
        {
            Thread t = new Thread(() =>
            {
                if (!cacheController.loadCaches())
                {
                    cacheController.clearCaches();
                    startNoCacheLoadThread();
                }
                else
                {
                    cacheController.verifyFileLengths();
                    journalParser.switchViewedCommander(cacheController.switchOnLoad);
                    cacheController.switchOnLoad = null;
                }
            });
            t.Start();
        }

        [Obsolete]
        internal void addEvent(params string[] v)
        {
            addEvent(v[2], v);
        }

        [Obsolete]
        internal void addEvent(string json, params string[] v)
        {
            try
            {
                if (v.Length == 0) { throw new InvalidOperationException("Data array cannot be empty."); }
                ListViewItem lvi = new ListViewItem(v);
                if (v.Length >= 4 && v[3].Equals("UNKNOWN EVENT"))
                {
                    lvi.BackColor = Color.Pink;
                }
                else if (v[1].Equals("LoadGame"))
                    lvi.BackColor = Color.LightGreen;
                lvi.ToolTipText = json;
                this.eventList.InvokeIfRequired(() =>
                {
                    eventList.Items.Insert(0, lvi);
                    foreach (ColumnHeader ch in eventList.Columns)
                    {

                        ch.Width = -2;
                    }
                });
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
        }

        private void eventList_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo ht = eventList.HitTest(e.Location);
            if (ht != null)
            {
                if (/*_lastToolTipMousePos != e.Location || */(ht.SubItem != null && ht.SubItem != _lastListViewItem))
                {
                    _lastListViewItem = ht.SubItem;
                    _lastToolTipMousePos = e.Location;
                    string tipText = /*ht.Item.SubItems[2].Text;*/ht.Item.ToolTipText;
                    toolTip.Show(tipText, (ListView)sender, e.Location.X + 10, e.Location.Y + 10, 5000);
                }
            }
        }

        private void MainForm_MouseEnter(object sender, EventArgs e)
        {
            if (toolTip.Active && !toolTip.ShowAlways)
                toolTip.Hide(eventList);
        }

        private void eventFilterDropdown_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selected = eventFilterDropdown.SelectedItem.ToString();
            foreach (ListViewItem i in eventList.Items)
            {
                if (i.SubItems[1].Text.Equals(selected))
                {
                    if (!defaultColours.ContainsKey(selected))
                    {
                        defaultColours.Add(selected, i.BackColor);

                    }
                    i.BackColor = Color.LightBlue;
                }
                else
                {
                    if (defaultColours.ContainsKey(i.SubItems[1].Text))
                        i.BackColor = defaultColours[i.SubItems[1].Text];
                }
            }
        }

        delegate void ThreadSafeAppStatusLabelTextChange(string text);

        public void setAppStatusText(string text)
        {
            if (this.InvokeRequired)
            {
                ThreadSafeAppStatusLabelTextChange tsasltc = new ThreadSafeAppStatusLabelTextChange(setAppStatusText);
                this.Invoke(tsasltc, new object[] { text });
            }
            else
            {
                this.appStatus.Text = text;
            }
        }

        private void openCacheDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Utils.getApplicationEXEFolderPath(), "cache/"));
        }

        private void clearCachesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all caches?\nThis may result in missing information if Journal files have been deleted.\n\nTHIS PROCESS CANNOT BE UNDONE.", "Confirm Cache Clear", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                cacheController.clearCaches();
                this.startNoCacheLoadThread();
            }
        }

        private void realoadJournalEntriesFromCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reload all Journal entries? This might take a while.", "Confirm reload", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                eventList.Items.Clear();
                cacheController.loadCaches();
            }
        }

        private void debugRefreshJournal_Click(object sender, EventArgs e)
        {
            string newCommander = this.comboCommanderList.SelectedItem.ToString();
            Thread t = new Thread(() =>
            {
                this.journalParser.switchViewedCommander(newCommander);
            });
            t.Start();

        }

        private void comboCommanderList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string newCommander = this.comboCommanderList.SelectedItem.ToString();
            Thread t = new Thread(() =>
            {
                this.journalParser.switchViewedCommander(newCommander);
            });
            t.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cacheController.saveAllCaches();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void hUDEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
                HUDEditor hud = new HUDEditor();
                hud.Show(this);
        }
    }
}
