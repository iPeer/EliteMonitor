using EliteMonitor.Extensions;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor.Exploration
{
    public partial class ExpeditionViewer : Form
    {

        public static ExpeditionViewer Instance;
        public Expedition LoadedExpedition { get; private set; }
        private bool DontUpdateOnChangedEvent = false;
        public long CurrentEstimatedValue = 0;
        public EventHandler OnExpeditionViewerClosed;
        public List<string> landmarkSystems = new List<string>()
        {
            "Sagittarius A*",
            "Colonia",
            "Beagle Point",
            "Great Annihilator"
        };

        public ExpeditionViewer()
        {
            InitializeComponent();
            Instance = this;
            loadExpeditions();
        }

        private void loadExpeditions()
        {
            this.comboBoxExpeditionPicker.Items.Clear();
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

        public void SetActiveExpedition(Expedition expedition)
        {
            this.CurrentEstimatedValue = 0;
            if (this.LoadedExpedition != null)
                this.LoadedExpedition.IsExpeditionLoaded = false;
            this.LoadedExpedition = expedition;
            this.LoadedExpedition.OnLoad();
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
            this.CurrentEstimatedValue = 0;
            this.listViewScanCounts.InvokeIfRequired(() =>
            {
                this.listViewScanCounts.BeginUpdate();
                this.listViewScanCounts.Items.Clear();
                foreach (KeyValuePair<string, long> kvp in ordered)
                {
                    string lookupName = kvp.Key;
                    bool isStar = false;
                    if (lookupName.EndsWithIgnoreCase("class star"))
                    {
                        lookupName = kvp.Key.Split(' ')[0];
                        isStar = true;
                    }
                    else
                    {
                        try
                        {
                            lookupName = MainForm.Instance.Database.StarClassNames.First(a => a.Value.Equals(kvp.Key)).Key;
                            isStar = true;
                        }
                        catch { }
                    }
                    int initialValue = MainForm.Instance.Database.getBodyPayout(lookupName, isStar);
                    long finalValue = initialValue * kvp.Value;
                    this.CurrentEstimatedValue += finalValue;
                    this.listViewScanCounts.Items.Add(new ListViewItem(new string[] { string.Format("{0:n0}", kvp.Value), kvp.Key, string.Format("{0:n0}", finalValue) }));
                }
                //int icolumn = 0;
                foreach (ColumnHeader column in this.listViewScanCounts.Columns)
                {

                    column.Width = -2;
                    /*if (icolumn++ == 0)
                        column.TextAlign = HorizontalAlignment.Right;*/
                }
                this.labelDataWorth.Text = string.Format("{0:n0} Cr", this.CurrentEstimatedValue);
                this.listViewScanCounts.EndUpdate();
            });
        }

        public void updateSystems()
        {
            this.listViewSystemList.InvokeIfRequired(() =>
            {
                this.listViewSystemList.BeginUpdate();
                this.listViewSystemList.Items.Clear();
                /*List<string> systemNames = this.LoadedExpedition.SystemNames.Select(a => a.SystemName).ToList<string>();
                foreach (string s in systemNames)
                    this.listViewSystemList.Items.Add(s);*/
                foreach (ExpeditionSystemData s in this.LoadedExpedition.SystemNames)
                {
                    ListViewItem lvi = new ListViewItem(new string[] { s.SystemName, s.Timestamp });
                    if (this.landmarkSystems.Contains(s.SystemName) || this.LoadedExpedition.AutoCompleteSystemName.Equals(s.SystemName))
                        lvi.BackColor = Color.LightGreen;
                    this.listViewSystemList.Items.Add(lvi);
                }
                foreach (ColumnHeader column in this.listViewSystemList.Columns)
                    column.Width = -2;
                this.listViewSystemList.EndUpdate();
                this.listViewSystemList.Items[this.listViewSystemList.Items.Count - 1].EnsureVisible();
            });
        }

        public void updateData()
        {

            this.labelExpeditionStatus.InvokeIfRequired(() =>
            {
                string tooltipString = string.Empty;
                if (this.LoadedExpedition.IsCompleted)
                    tooltipString = "This Expedition is complete!";
                else if (this.LoadedExpedition.AutoComplete)
                    tooltipString = string.Format("This Expedition will automatically end when you {0} {1}.", this.LoadedExpedition.StartingSystemName.Equals(this.LoadedExpedition.AutoCompleteSystemName) ? "return to" : "reach", this.LoadedExpedition.AutoCompleteSystemName);
                else
                    tooltipString = "This Expedtiion will only end when it manually marked as completed.";
                this.toolTip1.SetToolTip(this.labelExpeditionStatus, tooltipString);
            });

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
                Int64 totalJournalEntries = (this.LoadedExpedition.LastJournalEntryId > 0 ? this.LoadedExpedition.LastJournalEntryId - this.LoadedExpedition.ExpeditionStartingJournalEntryId : this.LoadedExpedition.TotalJournalEntries);
                this.listViewExpeditionStats.Items.Add(new ListViewItem(new string[] { "Total Journal Entry Count", string.Format("{0:n0}", totalJournalEntries) }));
                int icolumn = 0;
                foreach (ColumnHeader column in this.listViewExpeditionStats.Columns)
                {

                    column.Width = -2;
                    if (icolumn++ == this.listViewExpeditionStats.Columns.Count - 1)
                        column.TextAlign = HorizontalAlignment.Right;
                }
                this.listViewExpeditionStats.EndUpdate();
                this.buttonCompleted.Enabled = !this.LoadedExpedition.IsCompleted;
                this.labelExpeditionStatus.Text = this.LoadedExpedition.IsCompleted ? "COMPLETED" : "ACTIVE";
            });
        }

        private void comboBoxExpeditionPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.DontUpdateOnChangedEvent)
            {
                string SelectedName = this.comboBoxExpeditionPicker.Items[this.comboBoxExpeditionPicker.SelectedIndex].ToString();
                Expedition selectedExpedition = MainForm.Instance.journalParser.viewedCommander.Expeditions.First(a => a.Value.ExpeditionName.Equals(SelectedName)).Value;
                this.SetActiveExpedition(selectedExpedition);
            }
        }

        private void ExpeditionViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.LoadedExpedition.OnSave();
            this.LoadedExpedition.IsExpeditionLoaded = false;
            this.OnExpeditionViewerClosed?.Invoke(this, EventArgs.Empty);
        }

        private void buttonExportSystems_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".txt";
            sfd.InitialDirectory = Utils.getApplicationEXEFolderPath();
            sfd.FileName = Utils.CreateSafeFilename(string.Format("{0} Systems.txt", this.LoadedExpedition.ExpeditionName));
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string filePath = sfd.FileName;
                Console.WriteLine(filePath);
                using (StreamWriter sr = new StreamWriter(filePath))
                    foreach (string s in this.LoadedExpedition.SystemNames.Select(a => a.SystemName))
                        sr.WriteLine(s);
                if (MessageBox.Show(string.Format("Would you like to open this file now?", filePath), "Export complete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    Process.Start(filePath);
            }
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            string newName = Utils.Prompt("Enter a new name for this expedition", "Rename Expedition");
            if (MessageBox.Show($"Are you sure you want to rename the expedition '{this.LoadedExpedition.ExpeditionName}' to '{newName}'?", "Confirm Rename", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int index = this.comboBoxExpeditionPicker.SelectedIndex;
                this.LoadedExpedition.ExpeditionName = newName;
                this.loadExpeditions();
                this.comboBoxExpeditionPicker.SelectedIndex = index;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Once deleted, '{this.LoadedExpedition.ExpeditionName}' will be gone forever, OK?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (ModifierKeys == Keys.Shift || MessageBox.Show($"Are you ABSOLUTELY sure you want to delete the expedition '{this.LoadedExpedition.ExpeditionName}'?\nTip: You can hold shift to skip this confirmation in the future.", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int chance = new Random((int)TimeSpan.Parse(DateTime.Now.ToShortTimeString()).TotalSeconds).Next(0, 100);
#if DEBUG 
                    MainForm.Instance.logger.Log("Random chance: {0}", LogLevel.DEBUG, chance);
#endif
                    if (chance < 5)
                        MessageBox.Show(string.Format("{0} was released outside. Bye {0}", this.LoadedExpedition.ExpeditionName), ":(");
                    Elite.Commander c = MainForm.Instance.journalParser.viewedCommander;
                    this.comboBoxExpeditionPicker.Items.Remove(this.LoadedExpedition.ExpeditionName);
                    c.Expeditions.Remove(this.LoadedExpedition.ExpeditionID);
                    c.setSaveRequired();
                    if (this.comboBoxExpeditionPicker.Items.Count > 0)
                        this.comboBoxExpeditionPicker.SelectedIndex = this.comboBoxExpeditionPicker.Items.Count - 1;
                    else
                        this.Close();
                }
            }
        }

        private void buttonCompleted_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to mark this expedition as complete?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //this.LoadedExpedition.IsCompleted = true;
                this.LoadedExpedition.MarkCompleted();
                this.updateData();
            }
        }
    }
}
