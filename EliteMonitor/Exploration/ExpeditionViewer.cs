using EliteMonitor.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor.Exploration
{
    public partial class ExpeditionViewer : Form
    {

        public static ExpeditionViewer Instance;
        public Expedition LoadedExpedition { get; private set; }
        private bool DontUpdateOnChangedEvent = false;

        public ExpeditionViewer()
        {
            InitializeComponent();
            Instance = this;
            foreach (KeyValuePair<Guid, Expedition> e in MainForm.Instance.journalParser.viewedCommander.Expeditions)
            {
                this.comboBoxExpeditionPicker.Items.Add(e.Value.ExpeditionName);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        public void SetActiveExpedition(Guid guid)
        {
            if (this.LoadedExpedition != null)
                this.LoadedExpedition.IsExpeditionLoaded = false;
            this.LoadedExpedition = MainForm.Instance.journalParser.viewedCommander.Expeditions[guid];
            this.DontUpdateOnChangedEvent = true;
            this.comboBoxExpeditionPicker.SelectedIndex = this.comboBoxExpeditionPicker.Items.IndexOf(this.LoadedExpedition.ExpeditionName);
            this.DontUpdateOnChangedEvent = false;
            this.LoadedExpedition.IsExpeditionLoaded = true;
            this.updateData();
            this.updateSystems();
            this.updateScans();
        }

        public void updateScans()
        {
            var ordered = this.LoadedExpedition.ScanCounts.OrderByDescending(a => a.Value);
            this.listViewScanCounts.InvokeIfRequired(() =>
            {
                this.listViewScanCounts.BeginUpdate();
                this.listViewScanCounts.Items.Clear();
                foreach (KeyValuePair<string, long> kvp in ordered)
                    this.listViewScanCounts.Items.Add(new ListViewItem(new string[] { string.Format("{0:n0}", kvp.Value), kvp.Key }));
                //int icolumn = 0;
                foreach (ColumnHeader column in this.listViewScanCounts.Columns)
                {

                    column.Width = -2;
                    /*if (icolumn++ == 0)
                        column.TextAlign = HorizontalAlignment.Right;*/
                }
                this.listViewScanCounts.EndUpdate();
            });
        }

        public void updateSystems()
        {
            this.listViewSystemList.InvokeIfRequired(() =>
            {
                this.listViewSystemList.BeginUpdate();
                this.listViewSystemList.Items.Clear();
                List<string> systemNames = this.LoadedExpedition.SystemNames.Select(a => a.SystemName).ToList<string>();
                foreach (string s in systemNames)
                    this.listViewSystemList.Items.Add(s);
                foreach (ColumnHeader column in this.listViewSystemList.Columns)
                    column.Width = -2;
                this.listViewSystemList.EndUpdate();
                this.listViewSystemList.Items[this.listViewSystemList.Items.Count - 1].EnsureVisible();
            });
        }

        public void updateData()
        {
            this.listViewExpeditionStats.InvokeIfRequired(() =>
            {
                this.listViewExpeditionStats.BeginUpdate();
                this.listViewExpeditionStats.Items.Clear();
                this.listViewExpeditionStats.Items.Add(new ListViewItem(new string[] { "Total Jumps", string.Format("{0:n0}", this.LoadedExpedition.JumpCount) }));
                this.listViewExpeditionStats.Items.Add(new ListViewItem(new string[] { "Total Jump Distance", this.LoadedExpedition.TotalDistance.ToString("0,0.00") }));
                this.listViewExpeditionStats.Items.Add(new ListViewItem(new string[] { "Average Jump Distance", this.LoadedExpedition.AverageJumpDistance.ToString("0,0.00") }));
                this.listViewExpeditionStats.Items.Add(new ListViewItem());
                this.listViewExpeditionStats.Items.Add(new ListViewItem(new string[] { "Fuel Used (t)", this.LoadedExpedition.FuelUsed.ToString("0,0.00") }));
                this.listViewExpeditionStats.Items.Add(new ListViewItem());
                this.listViewExpeditionStats.Items.Add(new ListViewItem(new string[] { "Scanned Body Count", string.Format("{0:n0}", this.LoadedExpedition.BodyScanCount) }));
                this.listViewExpeditionStats.Items.Add(new ListViewItem(new string[] { "Total Journal Entry Count", string.Format("{0:n0}", this.LoadedExpedition.TotalJournalEntries) }));
                int icolumn = 0;
                foreach (ColumnHeader column in this.listViewExpeditionStats.Columns)
                {

                    column.Width = -2;
                    if (icolumn++ == this.listViewExpeditionStats.Columns.Count - 1)
                        column.TextAlign = HorizontalAlignment.Right;
                }
                this.listViewExpeditionStats.EndUpdate();
            });
        }

        private void comboBoxExpeditionPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.DontUpdateOnChangedEvent)
            {
                string SelectedName = this.comboBoxExpeditionPicker.Items[this.comboBoxExpeditionPicker.SelectedIndex].ToString();
                Guid selectedExpeditionGuid = MainForm.Instance.journalParser.viewedCommander.Expeditions.First(a => a.Value.ExpeditionName.Equals(SelectedName)).Key;
                this.SetActiveExpedition(selectedExpeditionGuid);
            }
        }

        private void ExpeditionViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.LoadedExpedition.IsExpeditionLoaded = false;
        }
    }
}
