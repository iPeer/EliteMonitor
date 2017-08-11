using EliteMonitor.Elite;
using EliteMonitor.Exploration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            Expedition x = new Expedition();
            x.ExpeditionStartingJournalEntryId = this.StartingJournalEntry.ID;
            x.SystemNames.Add(new ExpeditionSystemData(this.StartingJournalEntry.Timestamp, JObject.Parse(this.StartingJournalEntry.Json).GetValue("StarSystem").ToString()));
            x.ExpeditionName = eName;
            x.AutoComplete = this.checkBoxAutoComplete.Checked;
            x.AutoCompleteSystemName = this.textBoxStartPoint.Text;
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
    }
}
