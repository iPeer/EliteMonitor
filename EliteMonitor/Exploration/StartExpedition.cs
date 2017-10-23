using EliteMonitor.Elite;
using EliteMonitor.Exploration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EliteMonitor
{
    public partial class StartExpedition : Form
    {

        public JournalEntry StartingJournalEntry;

        public StartExpedition()
        {
            InitializeComponent();
        }

        public void SetUpData()
        {
            if (this.StartingJournalEntry != null)
            {
                JObject json = JObject.Parse(this.StartingJournalEntry.Json);
                this.textBoxStartPoint.Text = json.GetValue("StarSystem").ToString();
                this.textBoxStartPoint.Enabled = false;
                this.checkBoxAutoComplete.Checked = true;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            string eName = this.textBoxExpeditionName.Text;
            if (string.IsNullOrEmpty(eName) || string.IsNullOrWhiteSpace(eName)) { MessageBox.Show("You must enter a valid name for this expedition.", "Name Required"); return; }
            if (this.radioButtonEndOnSpecific.Checked && string.IsNullOrWhiteSpace(this.linkLabelEndingSystem.Text)) { MessageBox.Show("You must providing an end point for this expedition!", "End point required"); return; }
            Expedition x = new Expedition();
            x.ExpeditionStartingJournalEntryId = this.StartingJournalEntry.ID;
            x.SystemNames.Add(new ExpeditionSystemData(this.StartingJournalEntry.Timestamp, JObject.Parse(this.StartingJournalEntry.Json).GetValue("StarSystem").ToString()));
            x.ExpeditionName = eName;
            x.StartingSystemName = this.textBoxStartPoint.Text;
            x.AutoComplete = this.checkBoxAutoComplete.Checked;
            if (this.radioButtonEndOnStart.Checked)
                x.AutoCompleteSystemName = this.textBoxStartPoint.Text;
            else
                x.AutoCompleteSystemName = this.linkLabelEndingSystem.Text;
            MainForm.Instance.journalParser.viewedCommander.setSaveRequired();

            this.Close();
            ScanJournalDialog sjd = new ScanJournalDialog();
            sjd.Show(MainForm.Instance);
            Thread t = new Thread(() =>
            {
                sjd.startScan(StartingJournalEntry, x, MainForm.Instance.journalParser.viewedCommander);

            });
            t.IsBackground = false;
            t.Start();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void buttonPickEnd_Click(object sender, EventArgs e)
        {
            SystemSearchSelector sss = new SystemSearchSelector();
            sss.OnSystemSelected += OnEDSMSystemPicked;
            sss.SetCustomTitle("Select Ending System");
            sss.ShowDialog(this);
        }

        public void OnEDSMSystemPicked(object sender, BasicSystem system)
        {
            this.linkLabelEndingSystem.Text = system.Name;
            LinkLabel.Link lnk = new LinkLabel.Link();
            lnk.LinkData = string.Format("https://www.edsm.net/en/system/id/0/name/{0}", system.Name.Replace(" ", "+"));
            linkLabelEndingSystem.Links.Clear();
            linkLabelEndingSystem.Links.Add(lnk);
        }

        private void radioButtonEndOnStart_CheckedChanged(object sender, EventArgs e)
        {
            this.buttonPickEnd.Enabled = false;
        }

        private void radioButtonEndOnSpecific_CheckedChanged(object sender, EventArgs e)
        {
            this.buttonPickEnd.Enabled = true;
        }

        private void linkLabelEndingSystem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.linkLabelEndingSystem.Links.Count > 0)
                Process.Start(this.linkLabelEndingSystem.Links[0].LinkData.ToString());
        }
    }
}
